using System;

namespace Left4DeadHelper.ImageSharpExtensions.Formats.Vtf
{
    internal static class VtfConstants
    {
        // https://web.archive.org/web/20210509104827/https://developer.valvesoftware.com/wiki/Valve_Texture_Format
        public static ReadOnlySpan<byte> HeaderBytes => new byte[]
        {
            // "VTF\0", in little endian
            0x00,
            0x46,
            0x54,
            0x56
        };
    }
}
