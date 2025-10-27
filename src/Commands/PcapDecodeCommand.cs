using System.Net;
using Microsoft.Extensions.Logging;
using PacketDotNet;
using Ruida.Core;
using SharpPcap;
using SharpPcap.LibPcap;

namespace Ruida.Commands;

/// <summary>
/// Command to decode Ruida packets from Wireshark PCAP files
/// </summary>
public class PcapDecodeCommand : ICommand
{
    private readonly ILogger<PcapDecodeCommand> _logger;

    public string Name => "pcap-decode";
    public string Description => "Decode Ruida packets from a Wireshark PCAP file";

    public PcapDecodeCommand(ILogger<PcapDecodeCommand> logger)
    {
        _logger = logger;
    }

    public async Task<int> ExecuteAsync(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine(
                "Usage: pcap-decode <pcap-file> [--ip <ip-address>] [--magic <magic>] [--verbose]"
            );
            Console.WriteLine("  <pcap-file>     Path to the Wireshark PCAP file");
            Console.WriteLine(
                "  --ip <ip>       Target IP address to filter (default: 192.168.1.100)"
            );
            Console.WriteLine(
                "  --magic <hex>   Magic byte for unscrambling (default: auto-detect)"
            );
            Console.WriteLine("  --verbose       Show detailed packet information");
            return 1;
        }

        var pcapFile = args[0];
        var targetIp = IPAddress.Parse("192.168.1.100");
        byte? magic = null;
        var verbose = false;

        // Parse command line arguments
        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--ip":
                    if (i + 1 < args.Length)
                    {
                        if (!IPAddress.TryParse(args[++i], out targetIp))
                        {
                            Console.WriteLine($"Invalid IP address: {args[i]}");
                            return 1;
                        }
                    }
                    break;
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
                case "-88":
                    magic = 0x88;
                    break;
                case "--verbose":
                case "-v":
                    verbose = true;
                    break;
            }
        }

        if (!File.Exists(pcapFile))
        {
            Console.WriteLine($"PCAP file not found: {pcapFile}");
            return 1;
        }

        try
        {
            DecodePcapFile(pcapFile, targetIp, magic, verbose);
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decoding PCAP file");
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    private void DecodePcapFile(string pcapFile, IPAddress targetIp, byte? magic, bool verbose)
    {
        Console.WriteLine($"Decoding PCAP file: {pcapFile}");
        Console.WriteLine($"Target IP: {targetIp}");
        Console.WriteLine($"Magic byte: {(magic?.ToString("X2") ?? "auto-detect")}");
        Console.WriteLine();

        var device = new CaptureFileReaderDevice(pcapFile);
        device.Open();

        var packetCount = 0;
        var ruidaPacketCount = 0;

        try
        {
            GetPacketStatus status;
            while (
                (status = device.GetNextPacket(out PacketCapture e)) == GetPacketStatus.PacketRead
            )
            {
                packetCount++;
                var rawPacket = Packet.ParsePacket(e.Device.LinkType, e.Data.ToArray());

                if (
                    rawPacket is EthernetPacket ethernetPacket
                    && ethernetPacket.PayloadPacket is IPPacket ipPacket
                    && ipPacket.PayloadPacket is UdpPacket udpPacket
                )
                {
                    // Check if this is a Ruida packet (to/from target IP on port 50200 or 40200)
                    var isRuidaPacket =
                        (
                            ipPacket.SourceAddress.Equals(targetIp)
                            || ipPacket.DestinationAddress.Equals(targetIp)
                        )
                        && (
                            udpPacket.SourcePort == 50200
                            || udpPacket.DestinationPort == 50200
                            || udpPacket.SourcePort == 40200
                            || udpPacket.DestinationPort == 40200
                        );

                    if (isRuidaPacket)
                    {
                        var isRequest = ipPacket.DestinationAddress.Equals(targetIp);
                        ruidaPacketCount++;
                        var payload = udpPacket.PayloadData;

                        if (verbose)
                        {
                            Console.WriteLine(
                                $"\n--- Packet {ruidaPacketCount} (#{packetCount}) ---"
                            );
                            Console.WriteLine(
                                $"Time: {e.Header.Timeval.Date:yyyy-MM-dd HH:mm:ss.fff}"
                            );
                            Console.WriteLine(
                                $"Direction: {ipPacket.SourceAddress}:{udpPacket.SourcePort} -> {ipPacket.DestinationAddress}:{udpPacket.DestinationPort}"
                            );
                            Console.WriteLine($"Length: {payload.Length} bytes");
                        }

                        if (isRequest)
                        {
                            // Validate checksum according to wiki specification
                            if (!ValidateChecksum(payload, magic))
                            {
                                Console.WriteLine(
                                    $"⚠️  Packet {ruidaPacketCount}: Checksum validation failed"
                                );
                                if (verbose)
                                {
                                    Console.WriteLine(
                                        $"Raw data: {BitConverter.ToString(payload)}"
                                    );
                                    
                                    // Show unscrambled payload if magic is available
                                    if (magic.HasValue && payload.Length > 2)
                                    {
                                        var payloadOnly = new byte[payload.Length - 2];
                                        Array.Copy(payload, 2, payloadOnly, 0, payloadOnly.Length);
                                        var unscrambledPayload = UnscramblePayload(payloadOnly, magic);
                                        
                                        Console.WriteLine(
                                            $"Unscrambled payload: {BitConverter.ToString(unscrambledPayload)}"
                                        );
                                    }
                                }
                                continue;
                            }

                            if (verbose)
                            {
                                Console.WriteLine("✅ Checksum valid");
                            }

                            // Remove checksum (first 2 bytes) and decode Ruida data
                            var ruidaData = new byte[payload.Length - 2];
                            Array.Copy(payload, 2, ruidaData, 0, ruidaData.Length);

                            if (ruidaData.Length > 0)
                            {
                                DecodeRuidaData(ruidaData, magic, ruidaPacketCount, verbose, isRequest);
                            }
                        }
                        else
                        {
                            // Packet without checksum - device response (ACK, DA response, etc.)
                            // Device responses are scrambled and need to be unscrambled first
                            if (payload.Length > 0)
                            {
                                var unscrambledPayload = UnscramblePayload(payload, magic);
                                
                                if (verbose && magic.HasValue)
                                {
                                    Console.WriteLine($"Scrambled: {BitConverter.ToString(payload)}");
                                    Console.WriteLine($"Unscrambled: {BitConverter.ToString(unscrambledPayload)}");
                                }
                                
                                DecodeRuidaData(unscrambledPayload, magic, ruidaPacketCount, verbose, isRequest);
                            }
                        }
                    }
                }
            }
        }
        finally
        {
            device.Close();
        }

        Console.WriteLine($"\nSummary:");
        Console.WriteLine($"Total packets processed: {packetCount}");
        Console.WriteLine($"Ruida packets found: {ruidaPacketCount}");
    }

    private void DecodeRuidaData(byte[] ruidaData, byte? magic, int packetNumber, bool verbose, bool isRequest)
    {
        try
        {
            if (isRequest)
            {
                DecodeRequestData(ruidaData, magic, packetNumber, verbose);
            }
            else
            {
                DecodeResponseData(ruidaData, magic, packetNumber, verbose);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Packet {packetNumber}: Error decoding - {ex.Message}");
            if (verbose)
            {
                Console.WriteLine($"Raw data: {BitConverter.ToString(ruidaData)}");
            }
        }
    }

    private void DecodeRequestData(byte[] ruidaData, byte? magic, int packetNumber, bool verbose)
    {
        // Standard command parsing for requests
        var data = new RuidaData(ruidaData, magic);
        var parser = new RuidaParser(data);

        if (verbose)
        {
            Console.WriteLine($"Decoding {ruidaData.Length} bytes of Ruida data...");
        }

        var commandCount = 0;
        foreach (var command in parser.ParseCommands())
        {
            commandCount++;
            string interpretation = command.ToString();

            if (verbose)
            {
                Console.WriteLine($"  {interpretation}");
            }
            else
            {
                Console.WriteLine($"Packet {packetNumber}: {interpretation}");
            }
        }

        if (verbose && commandCount == 0)
        {
            Console.WriteLine("  No valid commands found");
        }
    }

    private void DecodeResponseData(byte[] ruidaData, byte? magic, int packetNumber, bool verbose)
    {
        // Check if this is a DA response first  
        // After unscrambling, DA responses are 9 bytes with format: DA 01 [property_2_bytes] [value_5_bytes]
        if (ruidaData.Length == 9 && ruidaData[0] == 0xDA && ruidaData[1] == 0x01)
        {
            DecodeDaResponse(ruidaData, packetNumber, verbose);
            return;
        }

        // Handle other simple response patterns without using the complex parser
        string interpretation = DecodeSimpleResponse(ruidaData);
        
        if (verbose)
        {
            Console.WriteLine($"Decoding {ruidaData.Length} bytes of response data...");
            Console.WriteLine($"Raw data: {BitConverter.ToString(ruidaData)}");
            Console.WriteLine($"  {interpretation}");
        }
        else
        {
            Console.WriteLine($"Packet {packetNumber}: {interpretation}");
        }
    }

    private string DecodeSimpleResponse(byte[] ruidaData)
    {
        if (ruidaData.Length == 0)
        {
            return "Empty response";
        }
        
        // Very short packets are likely ACKs or simple responses
        if (ruidaData.Length <= 2)
        {
            var hex = BitConverter.ToString(ruidaData).Replace("-", " ");
            return $"Simple ACK/Response: {hex}";
        }
        
        // For other response patterns, show the raw data
        var hexData = BitConverter.ToString(ruidaData).Replace("-", " ");
        return $"Response: {hexData}";
    }

    private void DecodeDaResponse(byte[] ruidaData, int packetNumber, bool verbose)
    {
        // Extract DA response components
        var propertyByte0 = ruidaData[2];
        var propertyByte1 = ruidaData[3];
        var valueBytes = new byte[5];
        Array.Copy(ruidaData, 4, valueBytes, 0, 5);
        
        // Get property description
        var propertyDesc = Cmd_da.GetPropertyDescription(propertyByte0, propertyByte1);
        
        // Format value interpretations
        var interpretations = GetValueInterpretations(valueBytes);
        
        string valueInterpretation = string.Join(", ", interpretations);
        string interpretation = $"Read_Param_Response {propertyByte0:X2} {propertyByte1:X2} {BitConverter.ToString(valueBytes).Replace("-", " ")} ({propertyDesc}) [{valueInterpretation}]";
        
        if (verbose)
        {
            Console.WriteLine($"Decoding {ruidaData.Length} bytes of Ruida data...");
            Console.WriteLine($"  {interpretation}");
        }
        else
        {
            Console.WriteLine($"Packet {packetNumber}: {interpretation}");
        }
    }

    private List<string> GetValueInterpretations(byte[] valueBytes)
    {
        var interpretations = new List<string>();
        
        // As Ruida encoded number (5 bytes)
        if (valueBytes.Length == 5)
        {
            var ruidaNumber = DecodeRuidaNumber(valueBytes);
            interpretations.Add($"ruida: {ruidaNumber}");
        }
        
        // As hex string
        interpretations.Add($"hex: {BitConverter.ToString(valueBytes).Replace("-", " ")}");
        
        // As ASCII if printable
        if (valueBytes.All(b => b >= 32 && b <= 126))
        {
            var ascii = System.Text.Encoding.ASCII.GetString(valueBytes.TakeWhile(b => b != 0).ToArray());
            if (!string.IsNullOrEmpty(ascii))
            {
                interpretations.Add($"ascii: \"{ascii}\"");
            }
        }
        
        return interpretations;
    }

    private static double DecodeRuidaNumber(byte[] bytes)
    {
        // Replicate the GetNumber logic from RuidaCommand
        double result = 0;
        double factor = 1;
        int n = bytes.Length;
        byte xor = n > 2 ? bytes[0] : (byte)0;
        
        // Reverse the bytes (as in GetNumber)
        var reversedBytes = new byte[bytes.Length];
        Array.Copy(bytes, reversedBytes, bytes.Length);
        Array.Reverse(reversedBytes);
        
        foreach (var b in reversedBytes)
        {
            var value = (byte)(b ^ xor);
            result += factor * value;
            factor *= 0x80;
        }
        
        return result;
    }

    private static bool ValidateChecksum(byte[] packetData, byte? magic = null)
    {
        if (packetData.Length < 2)
            return false;

        // The checksum is calculated on the scrambled payload data (not unscrambled)
        // The checksum header itself is not scrambled
        var expectedChecksum = (ushort)((packetData[0] << 8) | packetData[1]);
        
        var actualChecksum = 0;
        for (int i = 2; i < packetData.Length; i++)
        {
            actualChecksum += packetData[i];
        }
        actualChecksum &= 0xFFFF;

        return expectedChecksum == actualChecksum;
    }

    private static byte[] UnscramblePayload(byte[] payload, byte? magic)
    {
        if (!magic.HasValue)
        {
            return payload;
        }

        var unscrambled = new byte[payload.Length];
        for (int i = 0; i < payload.Length; i++)
        {
            unscrambled[i] = UnscrambleByte(payload[i], magic.Value);
        }
        return unscrambled;
    }

    private static byte UnscrambleByte(byte b, byte magic)
    {
        byte resB = (byte)(b == 0 ? 0xFF : b - 1);
        resB ^= magic;

        byte fb = (byte)(resB & 0x80);
        byte lb = (byte)(resB & 1);

        resB = (byte)(resB - fb - lb);
        resB |= (byte)(lb << 7);
        resB |= (byte)(fb >> 7);

        return resB;
    }
}
