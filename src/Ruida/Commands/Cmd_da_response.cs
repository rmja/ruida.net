using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// DA parameter response packet handler
/// These packets contain property values in response to DA read requests
/// Format: [property_code_2_bytes] [5_byte_value]
/// </summary>
public class DaResponseCommand : RuidaCommand
{
    private byte _propertyByte0;
    private byte _propertyByte1;
    private byte[] _valueBytes = new byte[5];

    public DaResponseCommand(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        // This is a special case - we don't use the normal format since this doesn't start with a command byte
        return new Dictionary<byte, object[]>();
    }

    public override void Interpret()
    {
        // For DA response packets, we consume the data directly
        // The factory will have already checked that this is a valid DA response packet
        
        // Get the first property byte
        var firstByte = _data.Consume(1, ConsumeMode.Any);
        if (!firstByte.HasValue) return;
        
        _propertyByte0 = firstByte.Value;
        
        // Get the second property byte
        var secondByte = _data.Consume(1, ConsumeMode.Any);
        if (!secondByte.HasValue) return;
        
        _propertyByte1 = secondByte.Value;
        
        // Get the 5-byte value
        var valueBytes = _data.ConsumeMultiple(5, ConsumeMode.Any);
        if (valueBytes == null || valueBytes.Length != 5) return;
        
        _valueBytes = valueBytes;
        
        // Set up the command name and args for display
        _name = "Read_Param_Response";
        _args.Clear();
        _args.Add($"{_propertyByte0:X2}");
        _args.Add($"{_propertyByte1:X2}");
        
        // Add value bytes as hex
        foreach (var b in _valueBytes)
        {
            _args.Add($"{b:X2}");
        }
    }

    /// <summary>
    /// Get property description from property code
    /// </summary>
    public string GetPropertyDescription()
    {
        return Cmd_da.GetPropertyDescription(_propertyByte0, _propertyByte1);
    }

    /// <summary>
    /// Get the property value as various interpretations
    /// </summary>
    public string GetValueInterpretation()
    {
        // Interpret the 5-byte value in different ways
        var interpretations = new List<string>();
        
        // As unsigned 32-bit integer (little endian, ignoring last byte if it's padding)
        if (_valueBytes.Length >= 4)
        {
            uint uint32Value = BitConverter.ToUInt32(_valueBytes, 0);
            interpretations.Add($"uint32: {uint32Value}");
        }
        
        // As signed 32-bit integer
        if (_valueBytes.Length >= 4)
        {
            int int32Value = BitConverter.ToInt32(_valueBytes, 0);
            interpretations.Add($"int32: {int32Value}");
        }
        
        // As hex string
        interpretations.Add($"hex: {BitConverter.ToString(_valueBytes).Replace("-", " ")}");
        
        // As ASCII if printable
        if (_valueBytes.All(b => b >= 32 && b <= 126))
        {
            var ascii = System.Text.Encoding.ASCII.GetString(_valueBytes.TakeWhile(b => b != 0).ToArray());
            if (!string.IsNullOrEmpty(ascii))
            {
                interpretations.Add($"ascii: \"{ascii}\"");
            }
        }
        
        return string.Join(", ", interpretations);
    }

    public override string ToString()
    {
        if (_name == null || _args.Count < 2) return base.ToString();
        
        string propertyDesc = GetPropertyDescription();
        string valueInterpretation = GetValueInterpretation();
        
        // Format: Read_Param_Response 05 7E 06 28 41 4A 10 (Card ID) [value interpretations]
        var result = $"{_name} {string.Join(" ", _args)} ({propertyDesc})";
        
        if (!string.IsNullOrEmpty(valueInterpretation))
        {
            result += $" [{valueInterpretation}]";
        }
        
        return result;
    }

    /// <summary>
    /// Check if the given data could be a DA response packet
    /// DA response packets start with property codes (data bytes < 0x80)
    /// and are exactly 7 bytes long: 2 bytes property code + 5 bytes value
    /// This should only match raw 7-byte packets without checksum headers
    /// </summary>
    public static bool CouldBeDAResponse(RuidaData data, int rawPacketLength)
    {
        // DA response packets are exactly 7 bytes without checksum header
        // DA request commands are 4 bytes with checksum header (6 bytes total)
        if (rawPacketLength != 7) return false;
        
        var currentPos = data.Position;
        
        // Check if we can read the first two bytes as property code
        if (currentPos + 1 >= data.Size) return false;
        
        var firstByte = data[currentPos];
        var secondByte = data[currentPos + 1];
        
        // CRITICAL: DA response packets start with property code (both bytes < 0x80)
        // DA request commands start with 0xDA (>= 0x80) so this check excludes them
        if (firstByte >= 0x80 || secondByte >= 0x80) return false;
        
        // Additional safety: if first byte looks like a command byte, reject
        if (firstByte == 0xDA) return false;
        
        // Check for common property code prefixes from actual PCAP data
        var isCommonPropertyCode = firstByte switch
        {
            0x00 => true, // System/axis properties
            0x02 => true, // Advanced settings
            0x03 => true, // Security
            0x04 => true, // Statistics
            0x05 => true, // Hardware ID
            0x0B => true, // Special properties
            _ => false
        };
        
        return isCommonPropertyCode;
    }
}