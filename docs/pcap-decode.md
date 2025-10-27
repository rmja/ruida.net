# PCAP Decode Command

The `pcap-decode` command allows you to analyze Ruida laser cutter communication captured in Wireshark PCAP files.

## Usage

```bash
dotnet run -- pcap-decode <pcap-file> [--ip <ip-address>] [--magic <magic>] [--verbose]
```

### Parameters

- `<pcap-file>`: Path to the Wireshark PCAP file containing network traffic
- `--ip <ip>`: Target IP address to filter packets (default: `192.168.1.100`)  
- `--magic <hex>`: Magic byte for unscrambling in hex format (default: auto-detect)
- `--verbose`: Show detailed packet information including timestamps and checksums

### Examples

```bash
# Basic usage with default IP
dotnet run -- pcap-decode capture.pcap

# Filter specific IP address
dotnet run -- pcap-decode capture.pcap --ip 192.168.1.50

# Verbose output with specific magic byte
dotnet run -- pcap-decode capture.pcap --ip 192.168.1.100 --magic 88 --verbose

# Using the compiled executable
ruida pcap-decode laser_session.pcap --verbose
```

## How It Works

According to the [EduTech Wiki Ruida specification](https://edutechwiki.unige.ch/en/Ruida), UDP packets contain:

1. **2-byte checksum** (big-endian) - sum of all payload bytes
2. **Ruida command data** - scrambled laser control commands

The command:
1. Opens the PCAP file using SharpPcap
2. Filters UDP packets to/from the target IP on ports 50200 (device) and 40200 (response)
3. Validates the 16-bit checksum in each packet
4. Removes the checksum header and unscrambles the remaining data
5. Decodes the Ruida commands using the same parser as the `decode` command

## Output Formats

### Normal Mode
```
Packet 1: Read_Param 05 7E
Packet 2: Write_Param 06 20 00 00 00 00 28 00 00 00 00 28
```

### Verbose Mode
```
--- Packet 1 (#15) ---
Time: 2024-10-24 15:30:45.123
Direction: 192.168.1.100:50200 -> 192.168.1.10:40200
Length: 12 bytes
✅ Checksum valid
Decoding 10 bytes of Ruida data...
  Read_Param 05 7E                                        DA 00 05 7E

--- Packet 2 (#16) ---
Time: 2024-10-24 15:30:45.150
Direction: 192.168.1.10:40200 -> 192.168.1.100:50200
Length: 16 bytes
✅ Checksum valid
  Write_Param 06 20 00 00 00 00 28 00 00 00 00 28               DA 01 06 20 00 00 00 00 28 00 00 00 00 28
```

## Network Protocol Details

- **Port 50200**: Device listening port (laser cutter)
- **Port 40200**: Response port (from laser cutter)
- **Protocol**: UDP with custom checksum and scrambling
- **Checksum**: 16-bit sum of payload bytes (excludes checksum itself)
- **Magic Values**: 0x88 (most models), 0x11 (634XG), others per model

## Requirements

- .NET 8.0+
- SharpPcap NuGet package (for PCAP file reading)
- PacketDotNet NuGet package (for packet parsing)

## Error Handling

The command handles various error conditions:
- File not found
- Invalid IP addresses
- Corrupt PCAP files
- Invalid magic bytes
- Checksum validation failures
- Malformed Ruida commands

## See Also

- `decode` - Decode standalone .rd files
- `dump` - Raw hex dump of .rd files
- `document` - Generate command documentation