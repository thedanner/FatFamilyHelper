using Left4DeadHelper.Sprays.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Left4DeadHelper.Sprays.SaveProfiles;

public class Vtf1024SaveProfile : Vtf512SaveProfile
{
    // With hi-res, the dimensions must be 1024x1020 or vice versa.
    public override int MaxWidth => 1024;
    public override int MaxHeight => 1024;
    private const int MaxSmallerDimension = 1020;
    public override string Extension => ".vtf";

    protected override void Resize(Image<Rgba32> image)
    {
        var maxWidth = MaxWidth;
        var maxHeight = MaxHeight;

        if (image.Width > image.Height)
        {
            maxHeight = MaxSmallerDimension;
        }
        else if (maxWidth > maxHeight)
        {
            maxWidth = MaxSmallerDimension;
        }
        else
        {
            maxWidth = maxHeight = MaxSmallerDimension;
        }

        ResizeUtil.Resize(image, maxWidth, maxHeight);
    }
}
