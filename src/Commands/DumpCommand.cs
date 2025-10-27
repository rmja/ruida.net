using Microsoft.Extensions.Logging;
using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// Hexdumps RD files
/// </summary>
public class DumpCommand : CommandBase
{
    public DumpCommand(ILogger<DumpCommand> logger)
        : base(logger) { }

    public override string Name => "dump";
    public override string Description => "Hexdump RD file";

    public override async Task<int> ExecuteAsync(string[] args)
    {
        try
        {
            byte? magic = null;
            var files = new List<string>();

            // Parse arguments
            foreach (var arg in args)
            {
                if (arg == "-88")
                {
                    magic = 0x88;
                }
                else
                {
                    files.Add(arg);
                }
            }

            if (files.Count == 0)
            {
                // Read from stdin
                await DumpFromStreamAsync(Console.OpenStandardInput(), magic);
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

                    await DumpFromFileAsync(file, magic);
                }
            }

            return 0;
        }
        catch (Exception ex)
        {
            return HandleError(ex, "dumping");
        }
    }

    private async Task DumpFromStreamAsync(Stream stream, byte? magic)
    {
        if (stream == Console.OpenStandardInput())
        {
            Console.Error.WriteLine("Reading from stdin");
        }

        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        var data = memoryStream.ToArray();

        DumpData(data, magic);
    }

    private async Task DumpFromFileAsync(string filePath, byte? magic)
    {
        var data = await File.ReadAllBytesAsync(filePath);
        DumpData(data, magic);
    }

    private void DumpData(byte[] rawData, byte? magic)
    {
        var ruidaData = new RuidaData(rawData, magic);

        DumpPosition(Console.Out, ruidaData.Position);

        foreach (var b in ruidaData.EnumerateBytes())
        {
            var pos = ruidaData.Position;
            if (pos % 16 == 0)
            {
                DumpPosition(Console.Out, pos);
            }
            Console.Write($"{b:X2} ");
        }

        Console.WriteLine();
    }

    private static void DumpPosition(TextWriter writer, int pos)
    {
        writer.Write($"\n{pos:X5}: ");
    }

    protected override void ShowHelp()
    {
        base.ShowHelp();
        Console.WriteLine("Usage: dump [-88] [file1] [file2] ...");
        Console.WriteLine("  -88   - Use magic value 0x88 for decoding");
        Console.WriteLine("  file  - RD file to dump (reads from stdin if no files specified)");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dump file.rd");
        Console.WriteLine("  dump -88 file.rd");
        Console.WriteLine("  cat file.rd | dump");
    }
}
