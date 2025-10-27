# Ruida .NET

A C# console application for decoding and analyzing Ruida laser cutter control files (.rd files), converted from a Ruby project. This tool can decode, dump, document, and generate lookup tables for Ruida laser control protocols.

## Features

- **Modular Command System**: Extensible command architecture using dependency injection
- **File Operations**: Copy, list, and manage files and directories
- **Cross-Platform**: Built on .NET 8, runs on Windows, macOS, and Linux
- **Comprehensive Testing**: Unit tests with high code coverage
- **Logging**: Built-in logging support for debugging and monitoring

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or later

### Building the Project

```bash
# Clone the repository (if applicable)
git clone <repository-url>
cd ruida-net

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run tests
dotnet test
```

### Running the Application

```bash
# Run from source
dotnet run --project src -- <command> [options]

# Or build and run the executable
dotnet build -c Release
./src/bin/Release/net8.0/ruida <command> [options]
```

## Available Commands

- `list [path]` - List files and directories in the specified path (current directory if not specified)
- `copy <source> <destination>` - Copy files or directories from source to destination
- `help` - Show help information for all available commands

### Examples

```bash
# List current directory
ruida list

# List specific directory
ruida list /path/to/directory

# Copy a file
ruida copy source.txt destination.txt

# Copy a directory
ruida copy /source/dir /destination/dir

# Show help
ruida help
```

## Project Structure

```
├── src/                    # Main application source
│   ├── Commands/          # Command implementations
│   ├── Core/              # Core interfaces and base classes
│   ├── Program.cs         # Application entry point
│   └── Ruida.csproj       # Project file
├── tests/                 # Unit tests
│   ├── Commands/          # Command tests
│   ├── Core/              # Core functionality tests
│   └── Ruida.Tests.csproj # Test project file
├── docs/                  # Documentation
├── Ruida.sln             # Solution file
└── README.md             # This file
```

## Development

### Adding New Commands

1. Create a new class in `src/Commands/` that inherits from `CommandBase`
2. Implement the required `Name`, `Description`, and `ExecuteAsync` properties/methods
3. Register the command in `Program.cs` in both `ConfigureServices` and `RegisterCommands` methods
4. Add unit tests in `tests/Commands/`

### Architecture

The application follows these design principles:

- **Command Pattern**: Each command is a separate class implementing `ICommand`
- **Dependency Injection**: Uses Microsoft.Extensions.DependencyInjection for IoC
- **Separation of Concerns**: Core logic separated from command implementations
- **SOLID Principles**: Extensible and maintainable code structure
- **Async/Await**: Asynchronous operations for better performance

### Testing

Run all tests:
```bash
dotnet test
```

Run tests with coverage:
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## License

[Specify your license here]

## Conversion Notes

This project was converted from a Ruby application to C#. The core functionality and command structure have been preserved while adapting to C# conventions and best practices.