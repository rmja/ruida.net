using System.Text;

namespace Ruida.Core;

/// <summary>
/// Represents Ruida laser control file data with unscrambling capabilities
/// </summary>
public class RuidaData
{
    private readonly byte[] _raw;
    private readonly byte[] _data;
    private readonly byte _magic;
    private readonly int _startPos;
    private int _pos;
    private readonly int _size;
    private readonly byte[] _fileType = new byte[3];

    public int Position => _pos;
    public int Size => _size;
    public byte[] FileType => _fileType;
    public byte Magic => _magic;

    public RuidaData(byte[] rawData, byte? magic = null)
    {
        _raw = rawData ?? throw new ArgumentNullException(nameof(rawData));
        
        if (magic.HasValue)
        {
            _magic = magic.Value;
            _startPos = 0;
        }
        else
        {
            _startPos = 3;
            _magic = RecognizeFileType();
        }

        // Unscramble the data
        _data = new byte[rawData.Length];
        for (int i = 0; i < rawData.Length; i++)
        {
            _data[i] = Unscramble(rawData[i]);
        }

        _size = _data.Length;
        Rewind();
    }

    /// <summary>
    /// Constructor for lookup table generation
    /// </summary>
    public RuidaData(byte magic)
    {
        _magic = magic;
        _raw = Array.Empty<byte>();
        _data = Array.Empty<byte>();
        _startPos = 0;
        _size = 0;
    }

    private byte RecognizeFileType()
    {
        if (_raw.Length < 3)
            throw new InvalidOperationException("File too short to recognize");

        _fileType[0] = _raw[0];
        _fileType[1] = _raw[1];
        _fileType[2] = _raw[2];

        return _fileType[2] switch
        {
            0xFA => 0x88, // Model 320, 633x, 644xg, 644xs, 654xs: D2 9B FA
            0x61 => 0x11, // Model 634xg: 49 04 61
            _ => throw new InvalidOperationException(
                $"Unknown model: {_fileType[0]:X2} {_fileType[1]:X2} {_fileType[2]:X2}. Maybe try specifying magic 0x88?")
        };
    }

    private byte Unscramble(byte b)
    {
        byte resB = (byte)(b == 0 ? 0xFF : b - 1);
        resB ^= _magic;
        
        byte fb = (byte)(resB & 0x80);
        byte lb = (byte)(resB & 1);
        
        resB = (byte)(resB - fb - lb);
        resB |= (byte)(lb << 7);
        resB |= (byte)(fb >> 7);
        
        return resB;
    }

    public Dictionary<byte, byte> GetLookupTable()
    {
        var result = new Dictionary<byte, byte>();
        for (int c = 0; c <= 255; c++)
        {
            result[(byte)c] = Unscramble((byte)c);
        }
        return result;
    }

    public Dictionary<byte, byte> GetReverseLookupTable()
    {
        var lookupTable = GetLookupTable();
        var reverseTable = new Dictionary<byte, byte>();
        foreach (var kvp in lookupTable)
        {
            reverseTable[kvp.Value] = kvp.Key;
        }
        return reverseTable;
    }

    public byte this[int pos] => _data[pos];

    public void Rewind()
    {
        _pos = _startPos;
    }

    public byte? Consume(int n = 1, ConsumeMode mode = ConsumeMode.Data)
    {
        if (_pos + n > _size)
            return null;

        var values = new byte[n];
        Array.Copy(_data, _pos, values, 0, n);
        _pos += n;

        switch (mode)
        {
            case ConsumeMode.Data:
                // Data bytes should have high bit clear
                foreach (var v in values)
                {
                    if (v > 127)
                        Console.Error.WriteLine($"*** Data 0x{v:X2}");
                }
                break;
            case ConsumeMode.Command:
                // Command bytes should have high bit set
                if (values[0] < 128)
                    throw new InvalidOperationException($"Command 0x{values[0]:X2}");
                break;
            case ConsumeMode.Any:
                break;
        }

        return n == 1 ? values[0] : null;
    }

    public byte[]? ConsumeMultiple(int n, ConsumeMode mode = ConsumeMode.Data)
    {
        if (_pos + n > _size)
            return null;

        var values = new byte[n];
        Array.Copy(_data, _pos, values, 0, n);
        _pos += n;

        switch (mode)
        {
            case ConsumeMode.Data:
                foreach (var v in values)
                {
                    if (v > 127)
                        Console.Error.WriteLine($"*** Data 0x{v:X2}");
                }
                break;
            case ConsumeMode.Command:
                if (values[0] < 128)
                    throw new InvalidOperationException($"Command 0x{values[0]:X2}");
                break;
        }

        return values;
    }

    public byte? GetCommand()
    {
        return Consume(1, ConsumeMode.Command);
    }

    public byte[]? GetValues(int n = 1)
    {
        return ConsumeMultiple(n, ConsumeMode.Data);
    }

    public byte? Peek()
    {
        return _pos < _size ? _data[_pos] : null;
    }

    public byte[] GetRaw(int start, int length)
    {
        var result = new byte[length];
        Array.Copy(_raw, start, result, 0, Math.Min(length, _raw.Length - start));
        return result;
    }

    public IEnumerable<byte> EnumerateBytes()
    {
        Rewind();
        byte? value;
        while ((value = Consume(1, ConsumeMode.Any)) != null)
        {
            yield return value.Value;
        }
        Rewind();
    }

    public override string ToString()
    {
        return $"RD {_size} bytes";
    }
}

public enum ConsumeMode
{
    Data,
    Command,
    Any
}