using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// Cut Relative command (0xA9)
/// </summary>
public class Cmd_a9 : RuidaCommand
{
    public Cmd_a9(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Cut_Rel", CommandParameter.Rel, CommandParameter.Rel };
    }
}