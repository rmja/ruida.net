using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// Move Absolute command (0x88)
/// </summary>
public class Cmd_88 : RuidaCommand
{
    public Cmd_88(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Mov_Abs", CommandParameter.Abs, CommandParameter.Abs };
    }
}