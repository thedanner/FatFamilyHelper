using Left4DeadHelper.ImageSharpExtensions.Formats.Vtf;
using Left4DeadHelper.Sprays.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    public class VtfFadingSaveProfile : BaseSaveProfile
    {
        public override int MaxWidth => 256;
        public override int MaxHeight => 256;
        public override string Extension => ".vtf";

        public override Task ConvertAsync(IList<Image<Rgba32>> images, Stream outputStream, CancellationToken cancellationToken)
        {
            if (images is null) throw new ArgumentNullException(nameof(images));
            if (images.Count != 2) throw new ArgumentException("Exactly two images are required for this format.", nameof(images));
            if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));

            // This format appears to use 9 mipmaps. The first is the biggest at 256x256 (this is the max supported).
            // Each subsequent one is half as wide and high, ending with 1x1. This makes 9 images.
            // `Math.Log(256, 2) + 1` (+ 1 because 2^0 = 1 and that's still a slot we need) can be
            // used to calculate this value, but since we're going up against file size limits anyway,
            // hard-coding is fine.

            const int totalImages = 9;
            var sizedImages = new List<Image<Rgba32>>(totalImages);
            sizedImages.AddRange(images.Take(totalImages));

            var lastImage = sizedImages.Last();

            while (sizedImages.Count < totalImages) sizedImages.Add(lastImage.Clone());

            var width = MaxWidth;
            var height = MaxHeight;

            foreach (var image in sizedImages)
            {
                ResizeUtil.Resize(image, width, height);

                width /= 2;
                height /= 2;

                lastImage = image;
            }

            var encoder = new VtfEncoder(VtfImageType.Fading);

            return encoder.EncodeWithMipmapsAsync(sizedImages, outputStream, cancellationToken);
        }
    }
}
