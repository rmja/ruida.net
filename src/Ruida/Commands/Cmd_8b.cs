using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// Move Vertical command (0x8B)
/// </summary>
public class Cmd_8b : RuidaCommand
{
    public Cmd_8b(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Move_Vert", CommandParameter.Rel };
    }
}