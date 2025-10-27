using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// C9 speed command (0xC9)
/// </summary>
public class Cmd_c9 : RuidaCommand
{
    public Cmd_c9(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new Dictionary<byte, object[]>
        {
            { 0x02, new object[] { "Velocity", CommandParameter.Speed } },
            { 0x03, new object[] { "Axis_Velocity", CommandParameter.Speed } },
            { 0x04, new object[] { "Part_Velocity", CommandParameter.Layer, CommandParameter.Speed } },
            { 0x05, new object[] { "Force_Velocity", CommandParameter.Speed } },
            { 0x06, new object[] { "Axis_Move_Speed", CommandParameter.Speed } }
        };
    }
}