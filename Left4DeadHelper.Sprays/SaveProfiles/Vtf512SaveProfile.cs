using Left4DeadHelper.ImageSharpExtensions.Formats.Vtf;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
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

        public override async Task ConvertAsync(IList<Image<Rgba32>> images,
            Stream outputStream, CancellationToken cancellationToken)
        {
            if (images is null) throw new ArgumentNullException(nameof(images));
            if (images.Count != 1) throw new ArgumentException("Only one image is permitted for this format.", nameof(images));
            if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));

            var image = images[0];

            Resize(image);

            var encoder = new VtfEncoder(VtfImageType.Single1024);

            await image.SaveAsync(outputStream, encoder, cancellationToken);
        }
    }
}
