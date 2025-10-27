using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// A0 axis Y/U move command (0xA0)
/// </summary>
public class Cmd_a0 : RuidaCommand
{
    public Cmd_a0(RuidaData data)
        : base(data) { }

    public override object GetFormat()
    {
        return new Dictionary<byte, object[]>
        {
            { 0x00, new string[] { "abs", "Axis_Y_Move" } },
            { 0x08, new string[] { "abs", "Axis_U_Move" } },
        };
    }
}