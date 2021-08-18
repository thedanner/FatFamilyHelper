using System;

namespace Left4DeadHelper.ImageSharpExtensions.Formats.Vtf
{
    internal static class VtfConstants
    {
        public const uint HeaderSize = 0x40;
        
        // Only used by the mini image, which we don't actually generate.
        public const uint FormatDxt1 = 0x0D; // 13
        public const uint FormatDxt5 = 0x0F; // 15
        public const uint FormatDxt1OneBitAlpha = 0x14; // 20

        /*
        TEXTUREFLAGS_ONEBITALPHA   = 0x00001000  // for DXT1
        TEXTUREFLAGS_EIGHTBITALPHA = 0x00002000, // for DXT5

        TEXTUREFLAGS_NOMIP         = 0x00000100,
        TEXTUREFLAGS_NOLOD         = 0x00000200,

        TEXTUREFLAGS_CLAMPS        = 0x00000004,
        TEXTUREFLAGS_CLAMPT        = 0x00000008,
        */
        public const uint FlagsDxt1WithAlpha = 0x130C;
        public const uint FlagsDxt5 = 0x230C;


        // https://web.archive.org/web/20210509104827/https://developer.valvesoftware.com/wiki/Valve_Texture_Format
        public static ReadOnlySpan<byte> HeaderBytes => new byte[]
        {
            // "VTF\0", in little endian
            0x56,
            0x54,
            0x46,
            0x00,
        };
    }
}
