using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// Cut Vertical command (0xAB)
/// </summary>
public class Cmd_ab : RuidaCommand
{
    public Cmd_ab(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Cut_Vert", CommandParameter.Rel };
    }
}