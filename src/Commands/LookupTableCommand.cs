using Microsoft.Extensions.Logging;
using Ruida.Core;

namespace Ruida.Commands;

/// <summary>
/// Generates lookup tables for RD scrambling/unscrambling
/// </summary>
public class LookupTableCommand : CommandBase
{
    public LookupTableCommand(ILogger<LookupTableCommand> logger) : base(logger) { }

    public override string Name => "lookuptable";
    public override string Description => "Generate lookup table for RD scrambling/unscrambling";

    public override async Task<int> ExecuteAsync(string[] args)
    {
        try
        {
            bool asReverse = false;
            bool asRuby = false;
            bool asJava = false;
            bool asMarkdown = true; // default
            byte? magic = null;

            // Parse arguments
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--reverse":
                        asReverse = true;
                        break;
                    case "--ruby":
                        asRuby = true;
                        asMarkdown = false;
                        break;
                    case "--java":
                        asJava = true;
                        asMarkdown = false;
                        break;
                    case "--markdown":
                        asRuby = false;
                        asMarkdown = true;
                        break;
                    default:
                        if (byte.TryParse(args[i], System.Globalization.NumberStyles.HexNumber, null, out var parsedMagic))
                        {
                            magic = parsedMagic;
                            Console.Error.WriteLine($"Using magic 0x{magic:X2}");
                        }
                        break;
                }
            }

            if (magic == null)
            {
                Console.Error.WriteLine("Usage: lookuptable [--ruby][--java][--markdown][--reverse] <magic>");
                Console.Error.WriteLine("  magic is a 2-digit hex value");
                Console.Error.WriteLine("  defaults to --markdown and magic 88 (Model 320, 633x, 644xg, 644xs, 654xs)");
                Console.Error.WriteLine("  use magic 11 for Model 634xg");
                return 1;
            }

            await GenerateLookupTableAsync(magic.Value, asReverse, asRuby, asJava, asMarkdown);
            return 0;
        }
        catch (Exception ex)
        {
            return HandleError(ex, "generating lookup table");
        }
    }

    private async Task GenerateLookupTableAsync(byte magic, bool asReverse, bool asRuby, bool asJava, bool asMarkdown)
    {
        var ruidaData = new RuidaData(magic);
        var lookupTable = asReverse ? ruidaData.GetReverseLookupTable() : ruidaData.GetLookupTable();

        if (asRuby)
        {
            Console.Write("{ ");
            foreach (var kvp in lookupTable)
            {
                Console.Write($"0x{kvp.Key:X2}=>0x{kvp.Value:X2}, ");
            }
            Console.WriteLine("}");
        }

        if (asJava)
        {
            Console.Write("byte[] ");
            Console.Write(asReverse ? "encode_table" : "decode_table");
            Console.Write(" = {");
            
            for (int k = 0; k <= 255; k++)
            {
                if (k % 16 == 0)
                    Console.WriteLine();
                
                var v = lookupTable[(byte)k];
                if (v > 127)
                    Console.Write("(byte)");
                Console.Write($"0x{v:X2}, ");
            }
            Console.WriteLine("};");
        }

        if (asMarkdown)
        {
            // Header
            Console.WriteLine("<table>");
            Console.Write("<tr><th></th>");
            for (int col = 0; col <= 15; col++)
            {
                Console.Write($"<th>{col:X1}</th>");
            }
            Console.WriteLine("</tr>");

            // Rows
            for (int row = 0; row <= 15; row++)
            {
                Console.Write("<tr>");
                for (int col = 0; col <= 15; col++)
                {
                    if (col == 0)
                    {
                        Console.Write($"<td>{row:X1}</td>");
                    }
                    var index = (byte)((row << 4) + col);
                    Console.Write($"<td>{lookupTable[index]:X2}</td>");
                }
                Console.WriteLine("</tr>");
            }
            Console.WriteLine("</table>");
        }

        await Task.CompletedTask;
    }

    protected override void ShowHelp()
    {
        base.ShowHelp();
        Console.WriteLine("Usage: lookuptable [--ruby][--java][--markdown][--reverse] <magic>");
        Console.WriteLine("  magic       - 2-digit hex value (88 for most models, 11 for 634xg)");
        Console.WriteLine("  --ruby      - Output as Ruby hash");
        Console.WriteLine("  --java      - Output as Java byte array");
        Console.WriteLine("  --markdown  - Output as HTML table (default)");
        Console.WriteLine("  --reverse   - Generate reverse lookup table");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  lookuptable 88");
        Console.WriteLine("  lookuptable --ruby 88");
        Console.WriteLine("  lookuptable --java --reverse 88");
        Console.WriteLine("  lookuptable 11  # For Model 634xg");
    }
}