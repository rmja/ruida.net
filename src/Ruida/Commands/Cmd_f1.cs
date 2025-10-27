using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// F1 start/offset command (0xF1)
/// </summary>
public class Cmd_f1 : RuidaCommand
{
    public Cmd_f1(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new Dictionary<byte, object[]>
        {
            { 0x00, new object[] { "Start0", 0x00 } },
            { 0x01, new object[] { "Start1", 0x00 } },
            { 0x02, new object[] { "Start2", 0x00 } },
            { 0x03, new object[] { "Laser2_Offset", CommandParameter.Abs, CommandParameter.Abs } },
            { 0x04, new object[] { "Enable_Feeding(auto?)", CommandParameter.Bool } }
        };
    }
}