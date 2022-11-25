using System;

namespace FatFamilyHelper.ImageSharpExtensions.Formats.Vtf;

internal static class VtfConstants
{
    public const uint HeaderSize = 0x40;
    
    // Only used by the mini image, which we don't actually generate.
    public const uint FormatDxt1 = 0x0D; // 13
    public const uint FormatDxt5 = 0x0F; // 15
    public const uint FormatDxt1OneBitAlpha = 0x14; // 20

    // https://developer.valvesoftware.com/wiki/Valve_Texture_Format#Image_flags
    [Flags]
    public enum VtfFlags : uint
    {
        None                                 = 0x00000000,
        PointSampling                        = 0x00000001, // Low quality, "pixel art" texture filtering.
        TrilinearSampling                    = 0x00000002,// Medium quality texture filtering.
        ClampS                               = 0x00000004, // Clamp S coordinates.
        ClampT                               = 0x00000008, // Clamp T coordinates.
        AnisotropicSampling                  = 0x00000010, // High quality texture filtering.
        HintDxt5                             = 0x00000020, // Used in skyboxes. Makes sure edges are seamless.
        PwlCorrected                         = 0x00000040, // Purpose unknown.
        NormalMap                            = 0x00000080, // Texture is a normal map.
        NoMipmaps                            = 0x00000100, // Render largest mipmap only. (Does not delete existing mipmaps, just disables them.)
        NoLevelOfDetail                      = 0x00000200, // Not affected by texture resolution settings.
        NoMinimumMipmap                      = 0x00000400, // If set, load mipmaps below 32x32 pixels.
        Procedural                           = 0x00000800, // Texture is an procedural texture (code can modify it).
        OneBitAlpha                          = 0x00001000, // One bit alpha channel used.
        EightBitAlpha                        = 0x00002000, // Eight bit alpha channel used.
        EnvironmentMap                       = 0x00004000, // Texture is an environment map.
        RenderTarget                         = 0x00008000, // Texture is a render target.
        DepthRenderTarget                    = 0x00010000, // Texture is a depth render target.
        NoDebugOverride                      = 0x00020000, // To do: Add description
        SingleCopy                           = 0x00040000, // To do: Add description
        PreSrgb                              = 0x00080000, // SRGB correction has already been applied
        PremultiplyColorByOneOverMipmapLevel = 0x00100000, // (Internal to VTEX?)
        NormalToDuDv                         = 0x00200000, // Texture is a DuDv map. (Internal to VTEX?)
        AlphaTestMipmapGeneration            = 0x00400000, // (Internal to VTEX?)
        NoDepthBuffer                        = 0x00800000, // Do not buffer for Video Processing, generally render distance.
        NiceFiltered                         = 0x01000000, // Use NICE filtering to generate mipmaps. (Internal to VTEX?)
        ClampU                               = 0x02000000, // Clamp U coordinates(for volumetric textures).
        VertexTexture                        = 0x04000000, // Usable as a vertex texture
        SSBump                               = 0x08000000, // Texture is a SSBump. (SSB)
        Border                               = 0x20000000, // Clamp to border colour on all texture coordinates
    }

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
