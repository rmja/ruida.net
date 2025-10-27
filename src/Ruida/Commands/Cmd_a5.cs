using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// A5 interface commands (0xA5) - UDP:50207 protocol
/// </summary>
public class Cmd_a5 : RuidaCommand
{
    public Cmd_a5(RuidaData data)
        : base(data) { }

    public override object GetFormat()
    {
        return new Dictionary<byte, object[]>
        {
            { 0x50, new string[] { "XX", "Interface_Keypress" } },
            { 0x51, new string[] { "XX", "Interface_KeyUp" } },
        };
    }
}