using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// Move Horizontal command (0x8A)
/// </summary>
public class Cmd_8a : RuidaCommand
{
    public Cmd_8a(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Move_Horiz", CommandParameter.Rel };
    }
}