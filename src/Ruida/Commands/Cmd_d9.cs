using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// D9 axis move with options command (0xD9)
/// </summary>
public class Cmd_d9 : RuidaCommand
{
    public Cmd_d9(RuidaData data)
        : base(data) { }

    public override object GetFormat()
    {
        return new Dictionary<byte, object[]>
        {
            { 0x00, new string[] { "XX", "abs", "Move_X" } },
            { 0x01, new string[] { "XX", "abs", "Move_Y" } },
            { 0x02, new string[] { "XX", "abs", "Move_Z" } },
            { 0x03, new string[] { "XX", "abs", "Move_U" } },
            { 0x10, new string[] { "XX", "Home_XY" } },
        };
    }
}