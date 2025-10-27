using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// Finish command (0xEB)
/// </summary>
public class Cmd_eb : RuidaCommand
{
    public Cmd_eb(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Finish" };
    }
}