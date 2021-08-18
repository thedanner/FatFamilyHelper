using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.ImageSharpExtensions.Formats.Vtf
{
    public sealed class VtfEncoder : IImageEncoder
    {
        private readonly DxtImageFormat _format;

        public VtfEncoder(DxtImageFormat format)
        {
            _format = format;
        }

        public void Encode<TPixel>(Image<TPixel> image, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
        {
            var encoder = new VtfEncoderCore();
            if (!(image is Image<Rgba32> imageAsRgba32)) imageAsRgba32 = image.CloneAs<Rgba32>();
            encoder.Encode(_format, imageAsRgba32, stream, default);
        }

        public async Task EncodeAsync<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            var encoder = new VtfEncoderCore();
            if (!(image is Image<Rgba32> imageAsRgba32)) imageAsRgba32 = image.CloneAs<Rgba32>();
            await encoder.EncodeAsync(_format, imageAsRgba32, stream, cancellationToken).ConfigureAwait(false);
        }
    }
}
