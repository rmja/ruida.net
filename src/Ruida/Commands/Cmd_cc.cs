using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// CC acknowledge command (0xCC)
/// </summary>
public class Cmd_cc : RuidaCommand
{
    public Cmd_cc(RuidaData data)
        : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Msg_Acknowledge" };
    }
}