using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// C4 command (0xC4)
/// </summary>
public class Cmd_c4 : RuidaCommand
{
    public Cmd_c4(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "End_Power_3", CommandParameter.Power };
    }
}