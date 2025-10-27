using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// C7 Immediate Power 1 command (0xC7)
/// </summary>
public class Cmd_c7 : RuidaCommand
{
    public Cmd_c7(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "Immediate_Power_1", CommandParameter.Power };
    }
}