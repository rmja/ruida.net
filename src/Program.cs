using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ruida.Commands;
using Ruida.Core;

namespace Ruida;

class Program
{
    static async Task<int> Main(string[] args)
    {
        // Set up dependency injection
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        using var serviceProvider = serviceCollection.BuildServiceProvider();

        // Set up command registry
        var commandRegistry = serviceProvider.GetRequiredService<CommandRegistry>();
        RegisterCommands(commandRegistry);

        // Execute command
        if (args.Length == 0)
        {
            commandRegistry.ShowHelp();
            return 0;
        }

        var commandName = args[0];
        var commandArgs = args.Skip(1).ToArray();

        return await commandRegistry.ExecuteCommandAsync(commandName, commandArgs);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Logging
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        // Core services
        services.AddSingleton<CommandRegistry>();

        // Commands
        services.AddTransient<DecodeCommand>();
        services.AddTransient<DumpCommand>();
        services.AddTransient<DocumentCommand>();
        services.AddTransient<LookupTableCommand>();
        services.AddTransient<PcapDecodeCommand>();
        services.AddTransient<HelpCommand>();
    }

    private static void RegisterCommands(CommandRegistry registry)
    {
        registry.RegisterCommand<DecodeCommand>();
        registry.RegisterCommand<DumpCommand>();
        registry.RegisterCommand<DocumentCommand>();
        registry.RegisterCommand<LookupTableCommand>();
        registry.RegisterCommand<PcapDecodeCommand>();
        registry.RegisterCommand<HelpCommand>();
    }
}