using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// 80 axis X move command (0x80)
/// </summary>
public class Cmd_80 : RuidaCommand
{
    public Cmd_80(RuidaData data)
        : base(data) { }

    public override object GetFormat()
    {
        return new Dictionary<byte, object[]>
        {
            { 0x00, new string[] { "abs", "Axis_X_Move" } },
            { 0x08, new string[] { "abs", "Axis_Z_Move" } },
        };
    }
}