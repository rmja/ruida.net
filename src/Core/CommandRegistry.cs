using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ruida.Core;

/// <summary>
/// Registry for managing and executing commands.
/// </summary>
public class CommandRegistry
{
    private readonly Dictionary<string, Type> _commands = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CommandRegistry> _logger;

    public CommandRegistry(IServiceProvider serviceProvider, ILogger<CommandRegistry> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Registers a command type with the registry.
    /// </summary>
    public void RegisterCommand<T>() where T : class, ICommand
    {
        var commandType = typeof(T);
        var instance = _serviceProvider.GetRequiredService<T>();
        _commands[instance.Name.ToLowerInvariant()] = commandType;
        _logger.LogDebug("Registered command: {CommandName}", instance.Name);
    }

    /// <summary>
    /// Gets all registered command names.
    /// </summary>
    public IEnumerable<string> GetCommandNames()
    {
        return _commands.Keys;
    }

    /// <summary>
    /// Executes a command by name with the provided arguments.
    /// </summary>
    public async Task<int> ExecuteCommandAsync(string commandName, string[] args)
    {
        if (string.IsNullOrWhiteSpace(commandName))
        {
            ShowHelp();
            return 1;
        }

        var normalizedName = commandName.ToLowerInvariant();
        
        if (!_commands.TryGetValue(normalizedName, out var commandType))
        {
            Console.Error.WriteLine($"Unknown command: {commandName}");
            ShowHelp();
            return 1;
        }

        try
        {
            var command = (ICommand)_serviceProvider.GetRequiredService(commandType);
            return await command.ExecuteAsync(args);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing command {CommandName}", commandName);
            Console.Error.WriteLine($"Error executing command '{commandName}': {ex.Message}");
            return 1;
        }
    }

    /// <summary>
    /// Shows help information for all commands.
    /// </summary>
    public void ShowHelp()
    {
        Console.WriteLine("Ruida - Laser Cutter File Analyzer");
        Console.WriteLine();
        Console.WriteLine("Available commands:");
        
        foreach (var (name, type) in _commands)
        {
            var instance = (ICommand)_serviceProvider.GetRequiredService(type);
            Console.WriteLine($"  {name,-15} - {instance.Description}");
        }
        
        Console.WriteLine();
        Console.WriteLine("Usage: ruida <command> [options]");
    }
}