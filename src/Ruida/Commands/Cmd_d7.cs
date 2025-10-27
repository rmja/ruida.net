using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// EOF command (0xD7)
/// </summary>
public class Cmd_d7 : RuidaCommand
{
    public Cmd_d7(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "EOF" };
    }
}