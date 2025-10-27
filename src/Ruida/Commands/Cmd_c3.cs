using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// C3 command (0xC3)
/// </summary>
public class Cmd_c3 : RuidaCommand
{
    public Cmd_c3(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Immediate_Power_4", CommandParameter.Power };
    }
}