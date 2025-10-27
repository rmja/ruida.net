using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// E7 complex command with many sub-commands (0xE7)
/// </summary>
public class Cmd_e7 : RuidaCommand
{
    public Cmd_e7(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new Dictionary<byte, object[]>
        {
            { 0x00, new object[] { "Stop" } },
            { 0x01, new object[] { "SetFilename", CommandParameter.String } },
            { 0x03, new object[] { "Bounding_Box_Top_Left", CommandParameter.Abs, CommandParameter.Abs } },
            { 0x04, new object[] { "E7 04", -4, CommandParameter.Abs, CommandParameter.Abs } },
            { 0x05, new object[] { "E7_05", -1 } },
            { 0x06, new object[] { "Feeding", CommandParameter.Abs, CommandParameter.Abs } },
            { 0x07, new object[] { "Bounding_Box_Bottom_Right", CommandParameter.Abs, CommandParameter.Abs } },
            { 0x08, new object[] { "Bottom_Right_E7_08", -4, CommandParameter.Abs, CommandParameter.Abs } },
            { 0x13, new object[] { "E7 13", CommandParameter.Abs, CommandParameter.Abs } },
            { 0x17, new object[] { "Bottom_Right_E7_17", CommandParameter.Abs, CommandParameter.Abs } },
            { 0x23, new object[] { "E7 23", CommandParameter.Abs, CommandParameter.Abs } },
            { 0x24, new object[] { "E7 24", -1 } },
            { 0x35, new object[] { "Block_X_Size", -4, -4 } },
            { 0x36, new object[] { "Set_File_Empty", -1 } },
            { 0x37, new object[] { "E7_37", CommandParameter.Abs, CommandParameter.Abs } },
            { 0x38, new object[] { "E7_38", -1 } },
            { 0x50, new object[] { "Bounding_Box_Top_Left", CommandParameter.Abs, CommandParameter.Abs } },
            { 0x51, new object[] { "Bounding_Box_Bottom_Right", CommandParameter.Abs, CommandParameter.Abs } },
            { 0x52, new object[] { "Layer_Top_Left_E7_52", CommandParameter.Layer, CommandParameter.Abs, CommandParameter.Abs } },
            { 0x53, new object[] { "Layer_Bottom_Right_E7_53", CommandParameter.Layer, CommandParameter.Abs, CommandParameter.Abs } },
            { 0x54, new object[] { "Pen_Draw_Y", CommandParameter.Layer, CommandParameter.Abs } },
            { 0x55, new object[] { "Laser_Y_Offset", CommandParameter.Layer, CommandParameter.Abs } },
            { 0x60, new object[] { "E7 60", -1 } },
            { 0x61, new object[] { "Layer_Top_Left_E7_61", CommandParameter.Layer, CommandParameter.Abs, CommandParameter.Abs } },
            { 0x62, new object[] { "Layer_Bottom_Right_E7_62", CommandParameter.Layer, CommandParameter.Abs, CommandParameter.Abs } }
        };
    }
}