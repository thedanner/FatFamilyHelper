using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    public class TgaSaveProfile : BaseSaveProfile<SingleImageConfiguration>
    {
        public override int MaxWidth => 256;
        public override int MaxHeight => 256;
        public override string Extension => ".tga";
        
        public override async Task ConvertAsync(SingleImageConfiguration imageConfiguration,
            Stream outputStream, CancellationToken cancellationToken)
        {
            if (imageConfiguration is null) throw new ArgumentNullException(nameof(imageConfiguration));
            if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));

            var image = imageConfiguration.Image;

            Resize(image);

            var encoder = new TgaEncoder
            {
                BitsPerPixel = TgaBitsPerPixel.Pixel32,
                Compression = TgaCompression.None,
            };

            await image.SaveAsync(outputStream, encoder, cancellationToken);
        }
    }
}
