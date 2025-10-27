# DA Command Property Descriptions

This document describes the implementation of property descriptions for Ruida DA (0xDA) commands in the .NET Ruida decoder.

## Overview

The DA command is used for reading and writing parameter values to/from the Ruida laser controller. The command format is:

- `DA 00 <byte0> <byte1>` - Read parameter at address (byte0 << 8) | byte1
- `DA 01 <byte0> <byte1> <5-byte value>` - Write parameter at address (byte0 << 8) | byte1

## Implementation

### Property Lookup Table

The `Cmd_da.cs` class now includes a comprehensive property lookup table based on the EduTech Wiki documentation. The lookup table maps property codes to human-readable descriptions.

### Property Categories

1. **0x00xx Properties** - Basic system and axis parameters
   - G0 Velocity (0x0005)
   - Home Velocity (0x000C)
   - System Control Mode (0x0010)
   - Laser PWM/Power settings (0x0011-0x001D, 0x0063-0x006C)
   - Axis Control Parameters (0x0020-0x005B)
   - Machine Type (0x0060)

2. **0x02xx Properties** - Advanced system settings
   - Turn Velocity (0x0201)
   - Acceleration settings (0x0202, 0x0209, 0x020A)
   - Delay settings (0x0203, 0x0207, 0x020B, 0x020D)
   - Manual speeds (0x0231, 0x0232)
   - Focus and positioning (0x020E, 0x020F)

3. **0x03xx Properties** - Security and language
   - Card Language (0x0300)
   - PC Lock settings (0x0301-0x0307)

4. **0x04xx Properties** - Statistics and status
   - Machine Status (0x0400)
   - Work time statistics (0x0401-0x0411)
   - Axis positions (0x0421, 0x0431, 0x0441, 0x0451)

5. **0x05xx Properties** - Hardware identification
   - Card ID (0x057E)
   - Mainboard Version (0x057F)

6. **Special Ranges**
   - Document Time (0x0710-0x0774)
   - Card Lock (0x0B11)

## Usage Examples

When decoding DA commands, the property descriptions are automatically displayed:

```
Read_Param 05 7E (Card ID)                    DA 00 05 7E
Write_Param 00 05 00 00 00 64 00 (G0 Velocity)   DA 01 00 05 00 00 00 64 00
Read_Param 99 99 (Unknown Property (0x9999))   DA 00 99 99
```

## Key Properties

### Card ID (0x057E)
Critical for identifying the laser controller model. The response determines the scrambling algorithm used:
- 0x6301, 0x6425, 0x6912: magic 0x88 (most common)
- 0x6320: magic 0x11 (634XG model)
- Various other model-specific magic values

### System Control Mode (0x0010)
Controls the return position behavior:
- 0x0000: Return to Origin
- 0x4000: No return
- 0x8000: Return to Absolute Origin

### Machine Status (0x0400)
Provides real-time status information about the laser controller state.

## Unknown Properties

Properties not in the lookup table are displayed as "Unknown Property (0xXXXX)" where XXXX is the hexadecimal property code. This is expected as not all properties are documented in the EduTech Wiki.

## Implementation Notes

- Property lookup is case-insensitive for hexadecimal values
- The lookup table can be easily extended with additional properties as they are discovered
- The system gracefully handles unknown properties without breaking decode functionality
- Property descriptions are integrated into the standard command output format

## References

- [EduTech Wiki Ruida Documentation](https://edutechwiki.unige.ch/en/Ruida#Properties)
- Original Ruby implementation property mappings