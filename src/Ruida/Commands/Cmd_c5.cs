using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// C5 command (0xC5)
/// </summary>
public class Cmd_c5 : RuidaCommand
{
    public Cmd_c5(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "End_Power_4", CommandParameter.Power };
    }
}