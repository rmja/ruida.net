namespace Ruida.Core;

/// <summary>
/// Parser for Ruida command data
/// </summary>
public class RuidaParser
{
    private readonly RuidaData _data;

    public RuidaParser(RuidaData data)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
    }

    public IEnumerable<RuidaCommand> ParseCommands()
    {
        while (true)
        {
            var command = RuidaCommandFactory.CreateCommand(_data);
            if (command == null)
                break;
            yield return command;
        }
    }
}