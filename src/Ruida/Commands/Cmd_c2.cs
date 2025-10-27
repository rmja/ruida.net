using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// C2 Immediate Power 3 command (0xC2)
/// </summary>
public class Cmd_c2 : RuidaCommand
{
    public Cmd_c2(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Immediate_Power_3", CommandParameter.Power };
    }
}