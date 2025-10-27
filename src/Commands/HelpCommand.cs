using Microsoft.Extensions.Logging;
using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// Displays help information for the application.
/// </summary>
public class HelpCommand : CommandBase
{
    private readonly CommandRegistry _commandRegistry;

    public HelpCommand(ILogger<HelpCommand> logger, CommandRegistry commandRegistry) : base(logger)
    {
        _commandRegistry = commandRegistry ?? throw new ArgumentNullException(nameof(commandRegistry));
    }

    public override string Name => "help";
    public override string Description => "Show help information";

    public override async Task<int> ExecuteAsync(string[] args)
    {
        try
        {
            _commandRegistry.ShowHelp();
            return await Task.FromResult(0);
        }
        catch (Exception ex)
        {
            return HandleError(ex, "showing help");
        }
    }
}