using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// C0 Immediate Power 2 command (0xC0)
/// </summary>
public class Cmd_c0 : RuidaCommand
{
    public Cmd_c0(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Immediate_Power_2", CommandParameter.Power };
    }
}