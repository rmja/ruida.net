using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// CA command with multiple sub-commands (0xCA)
/// </summary>
public class Cmd_ca : RuidaCommand
{
    public Cmd_ca(RuidaData data) : base(data) { }

    public override object GetFormat()
    {
        return new Dictionary<byte, object[]>
        {
            { 0x01, new object[] { "Flags_CA_01", "flags" } },
            { 0x02, new object[] { "Layer_Setup_Begin", CommandParameter.Layer } },
            { 0x03, new object[] { "CA_03", -1 } },
            { 0x06, new object[] { "Layer_Color", CommandParameter.Layer, CommandParameter.Color } },
            { 0x10, new object[] { "CA 10", -1 } },
            { 0x22, new object[] { "Layer_Count", -1 } },
            { 0x41, new object[] { "Layer_Setup_End", CommandParameter.Layer, -1 } }
        };
    }

    protected override void InvokeMethod(string methodName)
    {
        switch (methodName.ToLowerInvariant())
        {
            case "flags":
                HandleFlags();
                break;
            default:
                base.InvokeMethod(methodName);
                break;
        }
    }

    private void HandleFlags()
    {
        var f = Consume();
        switch (f)
        {
            case 0x12:
                _name = "Blow_off";
                break;
            case 0x13:
                _name = "Blow_on";
                break;
            default:
                _args.Add($"{f:X2}");
                break;
        }
    }
}