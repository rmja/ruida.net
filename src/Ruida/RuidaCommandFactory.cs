using System.Reflection;

namespace Ruida.Core;

/// <summary>
/// Factory for creating Ruida command instances
/// </summary>
public static class RuidaCommandFactory
{
    private static readonly Dictionary<byte, Type> _commandTypes = new();

    static RuidaCommandFactory()
    {
        LoadCommandTypes();
    }

    private static void LoadCommandTypes()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var commandTypes = assembly.GetTypes()
            .Where(t => t.IsSubclassOf(typeof(RuidaCommand)) && !t.IsAbstract);

        foreach (var type in commandTypes)
        {
            if (type.Name.StartsWith("Cmd_") && type.Name.Length >= 6)
            {
                var hexString = type.Name.Substring(4, 2);
                if (byte.TryParse(hexString, System.Globalization.NumberStyles.HexNumber, null, out var cmdByte))
                {
                    _commandTypes[cmdByte] = type;
                }
            }
        }
    }

    public static Type? GetCommandType(byte commandByte)
    {
        return _commandTypes.TryGetValue(commandByte, out var type) ? type : null;
    }

    public static RuidaCommand? CreateCommand(RuidaData data)
    {
        var cmdByte = data.GetCommand();
        if (!cmdByte.HasValue)
            return null;

        var commandType = GetCommandType(cmdByte.Value);
        if (commandType == null)
        {
            Console.Error.WriteLine($"*** Unknown {cmdByte.Value:X2} @ 0x{data.Position:X5}");
            Environment.Exit(1);
            return null;
        }

        var command = (RuidaCommand)Activator.CreateInstance(commandType, data)!;
        command.Interpret();
        return command;
    }

    public static IEnumerable<byte> GetKnownCommands()
    {
        return _commandTypes.Keys.OrderBy(x => x);
    }

    public static Type[] GetAllCommandTypes()
    {
        return _commandTypes.Values.ToArray();
    }
}