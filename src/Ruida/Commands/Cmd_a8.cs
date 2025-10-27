using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// Cut Absolute command (0xA8)
/// </summary>
public class Cmd_a8 : RuidaCommand
{
    public Cmd_a8(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Cut_Abs", CommandParameter.Abs, CommandParameter.Abs };
    }
}