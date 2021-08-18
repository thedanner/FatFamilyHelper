using Left4DeadHelper.ImageSharpExtensions.Formats.Vtf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    public class Vtf512SaveProfile : BaseSaveProfile
    {
        public override int MaxWidth => 512;
        public override int MaxHeight => 512;
        public override string Extension => ".vtf";

        public override async Task ConvertAsync(Image<Rgba32> image, Stream outputStream, CancellationToken cancellationToken)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));

            Resize(image);

            var encoder = new VtfEncoder(DxtImageFormat.Dxt5);

            await image.SaveAsync(outputStream, encoder, cancellationToken);
        }
    }
}
