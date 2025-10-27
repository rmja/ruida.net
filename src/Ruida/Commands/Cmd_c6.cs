using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// C6 command with laser power and timing sub-commands (0xC6)
/// </summary>
public class Cmd_c6 : RuidaCommand
{
    public Cmd_c6(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new Dictionary<byte, object[]>
        {
            { 0x01, new object[] { "Laser_1_Min_Pow_C6_01", CommandParameter.Power } },
            { 0x02, new object[] { "Laser_1_Max_Pow_C6_02", CommandParameter.Power } },
            { 0x05, new object[] { "Laser_3_Min_Pow_C6_05", CommandParameter.Power } },
            { 0x06, new object[] { "Laser_3_Max_Pow_C6_06", CommandParameter.Power } },
            { 0x07, new object[] { "Laser_4_Min_Pow_C6_07", CommandParameter.Power } },
            { 0x08, new object[] { "Laser_4_Max_Pow_C6_08", CommandParameter.Power } },
            { 0x10, new object[] { "Dot time", CommandParameter.Sec } },
            { 0x12, new object[] { "Cut_Open_delay_12", CommandParameter.Ms } },
            { 0x13, new object[] { "Cut_Close_delay_13", CommandParameter.Ms } },
            { 0x15, new object[] { "Cut_Open_delay_15", CommandParameter.Ms } },
            { 0x16, new object[] { "Cut_Close_delay_16", CommandParameter.Ms } },
            { 0x21, new object[] { "Laser_2_Min_Pow_C6_21", CommandParameter.Power } },
            { 0x22, new object[] { "Laser_2_Max_Pow_C6_22", CommandParameter.Power } },
            { 0x31, new object[] { "Laser_1_Min_Pow_C6_31", CommandParameter.Layer, CommandParameter.Power } },
            { 0x32, new object[] { "Laser_1_Max_Pow_C6_32", CommandParameter.Layer, CommandParameter.Power } },
            { 0x35, new object[] { "Laser_3_Min_Pow_C6_35", CommandParameter.Layer, CommandParameter.Power } },
            { 0x36, new object[] { "Laser_3_Max_Pow_C6_36", CommandParameter.Layer, CommandParameter.Power } },
            { 0x37, new object[] { "Laser_4_Min_Pow_C6_37", CommandParameter.Layer, CommandParameter.Power } },
            { 0x38, new object[] { "Laser_4_Max_Pow_C6_38", CommandParameter.Layer, CommandParameter.Power } },
            { 0x41, new object[] { "Laser_2_Min_Pow_C6_41", CommandParameter.Layer, CommandParameter.Power } },
            { 0x42, new object[] { "Laser_2_Max_Pow_C6_42", CommandParameter.Layer, CommandParameter.Power } },
            { 0x50, new object[] { "Cut_through_power1", CommandParameter.Power } },
            { 0x51, new object[] { "Cut_through_power2", CommandParameter.Power } },
            { 0x55, new object[] { "Cut_through_power3", CommandParameter.Power } },
            { 0x56, new object[] { "Cut_through_power4", CommandParameter.Power } },
            { 0x60, new object[] { "Laser_Freq", CommandParameter.Laser, 0x00, CommandParameter.Freq } }
        };
    }
}