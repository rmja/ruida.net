using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// D8 light/upload command (0xD8)
/// </summary>
public class Cmd_d8 : RuidaCommand
{
    public Cmd_d8(RuidaData data)
        : base(data) { }

    public override object GetFormat()
    {
        return new Dictionary<byte, object[]>
        {
            { 0x00, new object[] { "Start_Process" } },
            { 0x01, new object[] { "Stop_Process" } },
            { 0x02, new object[] { "Pause_Process" } },
            { 0x03, new object[] { "Restore_Process" } },
            { 0x10, new object[] { "Ref_Point_Mode_2" } },
            { 0x11, new object[] { "Ref_Point_Mode_1" } },
            { 0x12, new object[] { "Ref_Point_Mode_0" } },
            { 0x2C, new object[] { "Home_Z" } },
            { 0x2D, new object[] { "Home_U" } },
            { 0x2E, new object[] { "Focus_Z" } },
        };
    }
}
