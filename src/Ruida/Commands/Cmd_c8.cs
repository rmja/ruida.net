using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// C8 End Power 1 command (0xC8)
/// </summary>
public class Cmd_c8 : RuidaCommand
{
    public Cmd_c8(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new object[] { "End_Power_1", CommandParameter.Power };
    }
}