using Left4DeadHelper.ImageSharpExtensions.Formats.Vtf;
using Left4DeadHelper.ImageSharpExtensions.Utilities;
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

        public override void Resize(Image<Rgba32> image)
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

        public override async Task ConvertAsync(Image<Rgba32> image, Stream outputStream, CancellationToken cancellationToken)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));

            Resize(image);

            var encoder = new VtfEncoder();

            await image.SaveAsync(outputStream, encoder, cancellationToken);
        }
    }
}
