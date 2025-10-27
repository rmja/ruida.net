using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// Move Relative command (0x89)
/// </summary>
public class Cmd_89 : RuidaCommand
{
    public Cmd_89(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Mov_Rel", CommandParameter.Rel, CommandParameter.Rel };
    }
}