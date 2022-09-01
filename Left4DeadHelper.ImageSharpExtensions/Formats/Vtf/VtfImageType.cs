namespace Left4DeadHelper.ImageSharpExtensions.Formats.Vtf;

public enum VtfImageType
{
    /// <summary>
    /// A single image with resolution 1024x1020 (or reversed) with 1-bit alpha.
    /// </summary>
    Single1024,

    /// <summary>
    /// A single image with resolution 512x512 with 8-bit alpha.
    /// </summary>
    Single512,

    /// <summary>
    /// A distance-fading spray. The closest image is 256x256. The far images are half as large in each dimension.
    /// Each image uses 8-bit alpha.
    /// </summary>
    Fading
}
