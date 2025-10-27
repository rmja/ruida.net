using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// Cut Horizontal command (0xAA)
/// </summary>
public class Cmd_aa : RuidaCommand
{
    public Cmd_aa(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Cut_Horiz", CommandParameter.Rel };
    }
}