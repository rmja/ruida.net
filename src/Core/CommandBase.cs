using Microsoft.Extensions.Logging;

namespace Ruida.Core;

/// <summary>
/// Base class for all commands providing common functionality.
/// </summary>
public abstract class CommandBase : ICommand
{
    protected readonly ILogger Logger;

    protected CommandBase(ILogger logger)
    {
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public abstract string Name { get; }
    public abstract string Description { get; }

    public abstract Task<int> ExecuteAsync(string[] args);

    /// <summary>
    /// Displays help information for the command.
    /// </summary>
    protected virtual void ShowHelp()
    {
        Console.WriteLine($"Command: {Name}");
        Console.WriteLine($"Description: {Description}");
        Console.WriteLine();
    }

    /// <summary>
    /// Handles common error scenarios and logging.
    /// </summary>
    protected int HandleError(Exception ex, string context = "")
    {
        var message = string.IsNullOrEmpty(context) 
            ? $"Error in {Name}: {ex.Message}"
            : $"Error in {Name} ({context}): {ex.Message}";
            
        Logger.LogError(ex, message);
        Console.Error.WriteLine(message);
        return 1;
    }
}