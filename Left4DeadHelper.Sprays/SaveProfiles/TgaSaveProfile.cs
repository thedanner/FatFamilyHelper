using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    public class TgaSaveProfile : BaseSaveProfile
    {
        public override int MaxWidth => 256;
        public override int MaxHeight => 256;
        public override string Extension => ".tga";
        
        public override async Task ConvertAsync(Image<Rgba32> image, Stream outputStream, CancellationToken cancellationToken)
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));

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
