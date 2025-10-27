using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Ruida.Commands;
using Ruida.Core;
using Xunit;

namespace Ruida.Tests.Core;

public class CommandRegistryTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<ILogger<CommandRegistry>> _mockLogger;
    private readonly CommandRegistry _registry;

    public CommandRegistryTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockLogger = new Mock<ILogger<CommandRegistry>>();
        _registry = new CommandRegistry(_mockServiceProvider.Object, _mockLogger.Object);
    }

    [Fact]
    public void RegisterCommand_ShouldAddCommandToRegistry()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<DecodeCommand>>();
        var decodeCommand = new DecodeCommand(mockLogger.Object);
        _mockServiceProvider.Setup(sp => sp.GetService(typeof(DecodeCommand)))
            .Returns(decodeCommand);

        // Act
        _registry.RegisterCommand<DecodeCommand>();

        // Assert
        var commandNames = _registry.GetCommandNames();
        Assert.Contains("decode", commandNames);
    }

    [Fact]
    public async Task ExecuteCommandAsync_WithEmptyCommandName_ShouldReturnError()
    {
        // Act
        var result = await _registry.ExecuteCommandAsync("", Array.Empty<string>());

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task ExecuteCommandAsync_WithUnknownCommand_ShouldReturnError()
    {
        // Act
        var result = await _registry.ExecuteCommandAsync("unknown", Array.Empty<string>());

        // Assert
        Assert.Equal(1, result);
    }
}