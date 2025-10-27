using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// Magic88 command (0xF0)
/// </summary>
public class Cmd_f0 : RuidaCommand
{
    public Cmd_f0(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Magic88" };
    }
}