using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// C1 End Power 2 command (0xC1)
/// </summary>
public class Cmd_c1 : RuidaCommand
{
    public Cmd_c1(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "End_Power_2", CommandParameter.Power };
    }
}