using Microsoft.Extensions.Logging;
using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// Decodes RD files and shows human-readable laser commands
/// </summary>
public class DecodeCommand : CommandBase
{
    public DecodeCommand(ILogger<DecodeCommand> logger)
        : base(logger) { }

    public override string Name => "decode";
    public override string Description => "Decode RD file and show human-readable commands";

    public override async Task<int> ExecuteAsync(string[] args)
    {
        try
        {
            byte? magic = null;
            var files = new List<string>();
            string? singleStepOutput = null;

            // Parse arguments
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--magic":
                    if (i + 1 < args.Length)
                    {
                        if (
                            byte.TryParse(
                                args[++i],
                                System.Globalization.NumberStyles.HexNumber,
                                null,
                                out var magicByte
                            )
                        )
                        {
                            magic = magicByte;
                        }
                        else
                        {
                            Console.WriteLine($"Invalid magic byte: {args[i]}");
                            return 1;
                        }
                    }
                    break;
                    case "-11":
                        magic = 0x11;
                        break;
                    case "-88":
                        magic = 0x88;
                        break;
                    case "-s" when i + 1 < args.Length:
                        singleStepOutput = args[++i];
                        Console.WriteLine($"Single step, output to {singleStepOutput}");
                        break;
                    default:
                        files.Add(args[i]);
                        break;
                }
            }

            if (files.Count == 0)
            {
                // Read from stdin
                await DecodeFromStreamAsync(Console.OpenStandardInput(), magic, singleStepOutput);
            }
            else
            {
                // Read from files
                foreach (var file in files)
                {
                    if (!File.Exists(file))
                    {
                        Console.Error.WriteLine($"File not found: {file}");
                        return 1;
                    }

                    await DecodeFromFileAsync(file, magic, singleStepOutput);
                }
            }

            return 0;
        }
        catch (Exception ex)
        {
            return HandleError(ex, "decoding");
        }
    }

    private async Task DecodeFromStreamAsync(Stream stream, byte? magic, string? singleStepOutput)
    {
        if (stream == Console.OpenStandardInput())
        {
            Console.Error.WriteLine("Reading from stdin");
        }

        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var data = memoryStream.ToArray();

        await DecodeDataAsync(data, magic, singleStepOutput);
    }

    private async Task DecodeFromFileAsync(string filePath, byte? magic, string? singleStepOutput)
    {
        var data = await File.ReadAllBytesAsync(filePath);
        await DecodeDataAsync(data, magic, singleStepOutput);
    }

    private async Task DecodeDataAsync(byte[] rawData, byte? magic, string? singleStepOutput)
    {
        var ruidaData = new RuidaData(rawData, magic);
        var parser = new RuidaParser(ruidaData);

        FileStream? singleStepFile = null;
        if (singleStepOutput != null)
        {
            singleStepFile = File.Create(singleStepOutput);
            // Write header
            await singleStepFile.WriteAsync(ruidaData.GetRaw(0, 3));
        }

        try
        {
            foreach (var command in parser.ParseCommands())
            {
                Console.WriteLine(command.ToString());

                if (singleStepFile != null)
                {
                    Console.Out.Flush();
                    Console.Write($"Step [{command.Position:X4}:{command.Length}]?");
                    Console.ReadLine();

                    var rawBytes = command.GetRaw();
                    await singleStepFile.WriteAsync(rawBytes);
                    await singleStepFile.FlushAsync();
                }
            }
        }
        finally
        {
            singleStepFile?.Dispose();
        }
    }

    protected override void ShowHelp()
    {
        base.ShowHelp();
        Console.WriteLine("Usage: decode [-88] [file1] [file2] ... [-s output_file]");
        Console.WriteLine("  -88         - Use magic value 0x88 for decoding");
        Console.WriteLine("  -s output   - Single step mode, write output to file");
        Console.WriteLine(
            "  file        - RD file to decode (reads from stdin if no files specified)"
        );
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  decode file.rd");
        Console.WriteLine("  decode -88 file.rd");
        Console.WriteLine("  cat file.rd | decode");
        Console.WriteLine("  decode -s output.rd file.rd");
    }
}
