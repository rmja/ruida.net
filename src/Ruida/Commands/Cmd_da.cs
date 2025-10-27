using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// DA parameter read/write commands (0xDA)
/// </summary>
public class Cmd_da : RuidaCommand
{
    public Cmd_da(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new Dictionary<byte, object[]>
        {
            { 0x00, new object[] { "Read_Param", -1, -1 } },
            { 0x01, new object[] { "Write_Param", -1, -1, -5, -5 } },
        };
    }

    /// <summary>
    /// Property lookup table based on EduTech Wiki documentation
    /// https://edutechwiki.unige.ch/en/Ruida#Properties
    /// </summary>
    private static readonly Dictionary<ushort, string> PropertyLookup = new()
    {
        // 0x00xx properties
        { 0x0004, "IO Enable" },
        { 0x0005, "G0 Velocity" },
        { 0x000B, "Eng Facula" },
        { 0x000C, "Home Velocity" },
        { 0x000E, "Eng Vert Velocity" },
        { 0x0010, "System Control Mode" },
        { 0x0011, "Laser PWM Frequency 1" },
        { 0x0012, "Laser Min Power 1" },
        { 0x0013, "Laser Max Power 1" },
        { 0x0016, "Laser Attenuation" },
        { 0x0017, "Laser PWM Frequency 2" },
        { 0x0018, "Laser Min Power 2" },
        { 0x0019, "Laser Max Power 2" },
        { 0x001A, "Laser Standby Frequency 1" },
        { 0x001B, "Laser Standby Pulse 1" },
        { 0x001C, "Laser Standby Frequency 2" },
        { 0x001D, "Laser Standby Pulse 2" },
        { 0x001E, "Auto Type Space" },
        { 0x0020, "Axis Control Para 1" },
        { 0x0021, "Axis Precision 1" },
        { 0x0023, "Axis Max Velocity 1" },
        { 0x0024, "Axis Start Velocity 1" },
        { 0x0025, "Axis Max Acc 1" },
        { 0x0026, "Axis Range 1" },
        { 0x0027, "Axis Btn Start Vel 1" },
        { 0x0028, "Axis Btn Acc 1" },
        { 0x0029, "Axis Estp Acc 1" },
        { 0x002A, "Axis Home Offset 1" },
        { 0x002B, "Axis Backlash 1" },
        { 0x0030, "Axis Control Para 2" },
        { 0x0031, "Axis Precision 2" },
        { 0x0033, "Axis Max Velocity 2" },
        { 0x0034, "Axis Start Velocity 2" },
        { 0x0035, "Axis Max Acc 2" },
        { 0x0036, "Axis Range 2" },
        { 0x0037, "Axis Btn Start Vel 2" },
        { 0x0038, "Axis Btn Acc 2" },
        { 0x0039, "Axis Estp Acc 2" },
        { 0x003A, "Axis Home Offset 2" },
        { 0x003B, "Axis Backlash 2" },
        { 0x0040, "Axis Control Para 3" },
        { 0x0041, "Axis Precision 3" },
        { 0x0043, "Axis Max Velocity 3" },
        { 0x0044, "Axis Start Velocity 3" },
        { 0x0045, "Axis Max Acc 3" },
        { 0x0046, "Axis Range 3" },
        { 0x0047, "Axis Btn Start Vel 3" },
        { 0x0048, "Axis Btn Acc 3" },
        { 0x0049, "Axis Estp Acc 3" },
        { 0x004A, "Axis Home Offset 3" },
        { 0x004B, "Axis Backlash 3" },
        { 0x0050, "Axis Control Para 4" },
        { 0x0051, "Axis Precision 4" },
        { 0x0053, "Axis Max Velocity 4" },
        { 0x0054, "Axis Start Velocity 4" },
        { 0x0055, "Axis Max Acc 4" },
        { 0x0056, "Axis Range 4" },
        { 0x0057, "Axis Btn Start Vel 4" },
        { 0x0058, "Axis Btn Acc 4" },
        { 0x0059, "Axis Estp Acc 4" },
        { 0x005A, "Axis Home Offset 4" },
        { 0x005B, "Axis Backlash 4" },
        { 0x0060, "Machine Type" },
        { 0x0063, "Laser Min Power 3" },
        { 0x0064, "Laser Max Power 3" },
        { 0x0065, "Laser PWM Frequency 3" },
        { 0x0066, "Laser Standby Frequency 3" },
        { 0x0067, "Laser Standby Pulse 3" },
        { 0x0068, "Laser Min Power 4" },
        { 0x0069, "Laser Max Power 4" },
        { 0x006A, "Laser PWM Frequency 4" },
        { 0x006B, "Laser Standby Frequency 4" },
        { 0x006C, "Laser Standby Pulse 4" },

        // 0x02xx properties
        { 0x0200, "System Settings" },
        { 0x0201, "Turn Velocity" },
        { 0x0202, "Syn Acc" },
        { 0x0203, "G0 Delay" },
        { 0x0207, "Feed Delay After" },
        { 0x0209, "Turn Acc" },
        { 0x020A, "G0 Acc" },
        { 0x020B, "Feed Delay Prior" },
        { 0x020C, "Manual Dis" },
        { 0x020D, "Shut Down Delay" },
        { 0x020E, "Focus Depth" },
        { 0x020F, "Go Scale Blank" },
        { 0x0217, "Array Feed Repay" },
        { 0x021A, "Acc Ratio" },
        { 0x021B, "Turn Ratio" },
        { 0x021C, "Acc G0 Ratio" },
        { 0x021F, "Rotate Pulse" },
        { 0x0221, "Rotate D" },
        { 0x0224, "X Minimum Eng Velocity" },
        { 0x0225, "X Eng Acc" },
        { 0x0226, "User Para 1" },
        { 0x0228, "Z Home Velocity" },
        { 0x0229, "Z Work Velocity" },
        { 0x022A, "Z G0 Velocity" },
        { 0x022B, "Z Pen Up Position" },
        { 0x022C, "U Home Velocity" },
        { 0x022D, "U Work Velocity" },
        { 0x0231, "Manual Fast Speed" },
        { 0x0232, "Manual Slow Speed" },
        { 0x0234, "Y Minimum Eng Velocity" },
        { 0x0235, "Y Eng Acc" },
        { 0x0237, "Eng Acc Ratio" },

        // 0x03xx properties  
        { 0x0300, "Card Language" },
        { 0x0301, "PC Lock 1" },
        { 0x0302, "PC Lock 2" },
        { 0x0303, "PC Lock 3" },
        { 0x0304, "PC Lock 4" },
        { 0x0305, "PC Lock 5" },
        { 0x0306, "PC Lock 6" },
        { 0x0307, "PC Lock 7" },

        // 0x04xx properties
        { 0x0400, "Machine Status" },
        { 0x0401, "Total Open Time" },
        { 0x0402, "Total Work Time" },
        { 0x0403, "Total Work Number" },
        { 0x0405, "Total Doc Number" },
        { 0x0408, "Pre Work Time" },
        { 0x0411, "Total Laser Work Time" },
        { 0x0421, "Axis Preferred Position 1" },
        { 0x0423, "Total Work Length 1" },
        { 0x0431, "Axis Preferred Position 2" },
        { 0x0433, "Total Work Length 2" },
        { 0x0441, "Axis Preferred Position 3" },
        { 0x0443, "Total Work Length 3" },
        { 0x0451, "Axis Preferred Position 4" },
        { 0x0453, "Total Work Length 4" },

        // 0x05xx properties
        { 0x057E, "Card ID" },
        { 0x057F, "Mainboard Version" },

        // 0x07xx properties (Document Time range)
        { 0x0710, "Document Time" }, // 0x710-0x774 range

        // 0x0Bxx properties
        { 0x0B11, "Card Lock" },
    };

    /// <summary>
    /// Get property description from property code
    /// </summary>
    public static string GetPropertyDescription(byte byte0, byte byte1)
    {
        ushort propertyCode = (ushort)((byte0 << 8) | byte1);
        
        // Handle document time range (0x0710-0x0774)
        if (propertyCode >= 0x0710 && propertyCode <= 0x0774)
        {
            return "Document Time";
        }
        
        return PropertyLookup.TryGetValue(propertyCode, out string? description) 
            ? description 
            : $"Unknown Property (0x{propertyCode:X4})";
    }

    public override string ToString()
    {
        // Get the base string representation
        var baseString = base.ToString();
        
        // If we have at least 2 arguments (parameter bytes), add property description
        if (_args.Count >= 2 && _name != null)
        {
            // Extract parameter bytes from the args (they're in format "XX")
            if (byte.TryParse(_args[0], System.Globalization.NumberStyles.HexNumber, null, out byte byte0) &&
                byte.TryParse(_args[1], System.Globalization.NumberStyles.HexNumber, null, out byte byte1))
            {
                string propertyDesc = GetPropertyDescription(byte0, byte1);
                // Insert property description after the command name but before the tabs
                int nameEnd = _name.Length;
                foreach (var arg in _args)
                {
                    nameEnd += arg.Length + 1; // +1 for space
                }
                
                // Find where the tabs start
                var tabIndex = baseString.IndexOf('\t');
                if (tabIndex > 0)
                {
                    var beforeTabs = baseString.Substring(0, tabIndex);
                    var afterTabs = baseString.Substring(tabIndex);
                    return $"{beforeTabs} ({propertyDesc}){afterTabs}";
                }
                else
                {
                    return $"{baseString} ({propertyDesc})";
                }
            }
        }
        
        return baseString;
    }
}