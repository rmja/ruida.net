using System.Text;

namespace Ruida.Core;

/// <summary>
/// Base class for all Ruida laser commands
/// </summary>
public abstract class RuidaCommand
{
    protected readonly RuidaData _data;
    protected readonly List<string> _args = new();
    protected string? _name;

    public int Position { get; }
    public int Length { get; protected set; }
    
    protected RuidaCommand(RuidaData data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        Position = data.Position - 1; // Account for the command byte already consumed
    }

    /// <summary>
    /// Override in derived classes to provide format specification
    /// </summary>
    public abstract object GetFormat();

    /// <summary>
    /// Interpret the command based on its format specification
    /// </summary>
    public virtual void Interpret()
    {
        var format = GetFormat();
        byte? sub = null;

        switch (format)
        {
            case object[] arrayFormat:
                InterpretArray(arrayFormat);
                break;
            case Dictionary<byte, object[]> hashFormat:
                sub = Consume();
                if (hashFormat.TryGetValue(sub.Value, out var subFormat))
                {
                    InterpretArray(subFormat);
                }
                else
                {
                    Error($"Undefined {GetType().Name}:{sub:X2}");
                }
                break;
            case null:
                break;
            default:
                Error($"Unknown format value {format}");
                break;
        }

        Length = _data.Position - Position;
    }

    protected void InterpretArray(object[] format)
    {
        if (format.Length == 0) return;

        _name = format[0].ToString();
        for (int i = 1; i < format.Length; i++)
        {
            var item = format[i];
            switch (item)
            {
                case string methodName:
                    InvokeMethod(methodName);
                    break;
                case int value when value >= -1:
                    var consumed = Consume();
                    if (value >= 0 && consumed != value)
                    {
                        Console.Error.WriteLine($"{Position:X5}: {GetType().Name}{_args}: expected {value:X2}, got {consumed:X2}");
                        Environment.Exit(1);
                    }
                    _args.Add($"{consumed:X2}");
                    break;
                case int negative when negative < -1:
                    var bytes = ConsumeMultiple(-negative);
                    foreach (var b in bytes)
                    {
                        _args.Add($"{b:X2}");
                    }
                    break;
                case CommandParameter.Abs:
                    _args.Add($"{GetAbsoluteCoordinate():F3}mm");
                    break;
                case CommandParameter.Rel:
                    _args.Add($"{GetRelativeCoordinate():F3}mm");
                    break;
                case CommandParameter.Speed:
                    _args.Add($"{GetSpeed():F3}mm/s");
                    break;
                case CommandParameter.Power:
                    _args.Add($"{GetPercent()}%");
                    break;
                case CommandParameter.Layer:
                    _args.Add($"Layer:{GetLayer()}");
                    break;
                case CommandParameter.Ms:
                    _args.Add($"{GetMilliseconds():F3} ms");
                    break;
                case CommandParameter.Sec:
                    _args.Add($"{GetSeconds():F6} s");
                    break;
                case CommandParameter.Percent:
                    _args.Add($"{GetPercent()}%");
                    break;
                case CommandParameter.Bool:
                    _args.Add(GetBool().ToString());
                    break;
                case CommandParameter.Freq:
                    _args.Add($"{GetFrequency():F3}kHz");
                    break;
                case CommandParameter.Laser:
                    _args.Add($"Laser{Consume() + 1}");
                    break;
                case CommandParameter.Meter:
                    _args.Add($"{GetAbsoluteCoordinate() * 1000:F3}mm");
                    break;
                case CommandParameter.Color:
                    _args.Add(GetColor());
                    break;
                case CommandParameter.Priority:
                    _args.Add($"Prio {Consume()}");
                    break;
                case CommandParameter.String:
                    _args.Add($" {GetCString()}");
                    break;
                case CommandParameter.Number:
                    _args.Add($"{GetNumber(2)}");
                    break;
                default:
                    Error($"Can't interpret {item}");
                    break;
            }
        }
    }

    protected virtual void InvokeMethod(string methodName)
    {
        // Override in derived classes if needed
        switch (methodName.ToLowerInvariant())
        {
            default:
                Error($"Unknown method: {methodName}");
                break;
        }
    }

    protected byte Consume()
    {
        return _data.Consume(1, ConsumeMode.Data) ?? throw new InvalidOperationException("Unexpected end of data");
    }

    protected byte[] ConsumeMultiple(int count)
    {
        return _data.ConsumeMultiple(count, ConsumeMode.Data) ?? throw new InvalidOperationException("Unexpected end of data");
    }

    protected byte? Peek()
    {
        return _data.Peek();
    }

    protected bool GetBool()
    {
        return Consume() switch
        {
            0 => false,
            1 => true,
            var x => throw new InvalidOperationException($"Not a bool: {x}")
        };
    }

    protected int GetPercent()
    {
        var p = (int)Math.Round(GetNumber(2) * 0.006103516); // 100/2^14
        if (p > 100)
            Error($"Not percent: {p}");
        return p;
    }

    protected double GetNumber(int n)
    {
        double result = 0;
        double factor = 1;
        byte xor = n > 2 ? (Peek() ?? 0) : (byte)0;
        
        var bytes = ConsumeMultiple(n);
        Array.Reverse(bytes);
        
        foreach (var b in bytes)
        {
            var value = (byte)(b ^ xor);
            result += factor * value;
            factor *= 0x80;
        }
        
        return result;
    }

    protected double GetAbsoluteCoordinate()
    {
        return GetNumber(5) / 1000.0;
    }

    protected double GetRelativeCoordinate()
    {
        var result = GetNumber(2);
        if (result > 8191)
            result -= 16384;
        return result / 1000.0;
    }

    protected double GetSpeed()
    {
        return GetNumber(5) / 1000.0;
    }

    protected byte GetLayer()
    {
        return Consume();
    }

    protected double GetMilliseconds()
    {
        return GetNumber(5) / 1000.0;
    }

    protected double GetSeconds()
    {
        return GetNumber(5) / 1000.0 / 1000.0;
    }

    protected double GetFrequency()
    {
        return GetNumber(5) / 1000.0;
    }

    protected string GetCString()
    {
        var sb = new StringBuilder();
        byte b;
        while ((b = Consume()) != 0)
        {
            sb.Append((char)b);
        }
        return sb.ToString();
    }

    protected string GetColor()
    {
        var rgb = ConsumeMultiple(5);
        Array.Reverse(rgb);
        
        var red = rgb[0] + ((rgb[1] & 0x01) << 7);
        var green = ((rgb[1] & 0x7e) >> 1) + ((rgb[2] & 0x03) << 6);
        var blue = ((rgb[2] & 0x7c) >> 2) + ((rgb[3] & 0x07) << 5);
        
        return $"R {NormalizeColor(red)}%, G {NormalizeColor(green)}%, B {NormalizeColor(blue)}%";
    }

    private static int NormalizeColor(int c)
    {
        return (int)(c * 0.392156863); // * 100/255
    }

    protected void Error(string message)
    {
        var pos = $" @ {_data.Position:X5}";
        Console.Error.WriteLine(message + pos);
        Environment.Exit(1);
    }

    public byte[] GetRaw()
    {
        return _data.GetRaw(Position, Length);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append(_name ?? GetType().Name);
        
        foreach (var arg in _args)
        {
            sb.Append($" {arg}");
        }

        // Add tabs for alignment
        var tabCount = 7 - (sb.Length / 8);
        for (int i = 0; i < tabCount; i++)
        {
            sb.Append('\t');
        }

        // Add hex bytes
        for (int i = Position; i < Position + Length; i++)
        {
            sb.Append($"{_data[i]:X2} ");
        }

        return sb.ToString();
    }
}

public enum CommandParameter
{
    Abs,
    Rel,
    Speed,
    Power,
    Layer,
    Ms,
    Sec,
    Percent,
    Bool,
    Freq,
    Laser,
    Meter,
    Color,
    Priority,
    String,
    Number
}