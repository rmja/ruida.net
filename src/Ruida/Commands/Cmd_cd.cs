using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// CD error message command (0xCD)
/// </summary>
public class Cmd_cd : RuidaCommand
{
    public Cmd_cd(RuidaData data)
        : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Msg_Error" };
    }
}