using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// EA command (0xEA)
/// </summary>
public class Cmd_ea : RuidaCommand
{
    public Cmd_ea(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "EA", -1 };
    }
}