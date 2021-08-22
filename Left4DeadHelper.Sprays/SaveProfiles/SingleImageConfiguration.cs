using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    public class SingleImageConfiguration
    {
        public SingleImageConfiguration(Image<Rgba32> image)
        {
            Image = image ?? throw new ArgumentNullException(nameof(image));
        }

        public Image<Rgba32> Image { get; }
    }
}
