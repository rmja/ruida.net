namespace Ruida.Core;

/// <summary>
/// Interface for all commands in the Ruida application.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the description of what the command does.
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Executes the command with the provided arguments.
    /// </summary>
    /// <param name="args">Command line arguments</param>
    /// <returns>Exit code (0 for success, non-zero for error)</returns>
    Task<int> ExecuteAsync(string[] args);
}