using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// F2 command with various sub-commands (0xF2)
/// </summary>
public class Cmd_f2 : RuidaCommand
{
    public Cmd_f2(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new Dictionary<byte, object[]>
        {
            { 0x00, new object[] { "F2 00", -1 } },
            { 0x01, new object[] { "F2 01", -1 } },
            { 0x02, new object[] { "F2 02", -10 } },
            { 0x03, new object[] { "F2 03", CommandParameter.Abs, CommandParameter.Abs } },
            { 0x04, new object[] { "Bottom_Right_F2_04", CommandParameter.Abs, CommandParameter.Abs } },
            { 0x05, new object[] { "Bottom_Right_F2_05", -4, CommandParameter.Abs, CommandParameter.Abs } },
            { 0x06, new object[] { "F2 06", CommandParameter.Abs, CommandParameter.Abs } },
            { 0x07, new object[] { "F2 07", -1 } }
        };
    }
}