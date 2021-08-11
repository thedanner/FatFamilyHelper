using Left4DeadHelper.ImageSharpExtensions.Formats.Vtf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    public class VtfHiResSaveProfile : BaseSaveProfile
    {
        // With hi-res, the dimensions must be 1024x1020 or vice versa.
        public override int MaxWidth => 1024;
        public override int MaxHeight => 1024;

        private const int MaxSmallerDimension = 1020;

        public override string Extension => ".hi.vtf";

        public override void ClampDimensions(Image<Rgba32> image)
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

            ClampDimensions(image, maxWidth, maxHeight);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task ConvertAsync(Image<Rgba32> image, Stream outputStream, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));

            var encoder = new VtfEncoder();

            await image.SaveAsync(outputStream, encoder, cancellationToken);
        }
    }
}
