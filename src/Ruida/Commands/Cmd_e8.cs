using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// E8 file storage command (0xE8)
/// </summary>
public class Cmd_e8 : RuidaCommand
{
    public Cmd_e8(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new Dictionary<byte, object[]>
        {
            { 0x00, new object[] { "Delete_Document" } },
            { 0x01, new object[] { "Document_Name", CommandParameter.Number } },
            { 0x02, new object[] { "File_Transfer" } },
            { 0x03, new object[] { "Select_Document", CommandParameter.Number } },
            { 0x04, new object[] { "Calculate_Document_Time" } }
        };
    }
}