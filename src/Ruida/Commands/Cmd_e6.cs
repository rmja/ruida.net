using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// E6 command (0xE6)
/// </summary>
public class Cmd_e6 : RuidaCommand
{
    public Cmd_e6(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "E601", 0x01 };
    }
}