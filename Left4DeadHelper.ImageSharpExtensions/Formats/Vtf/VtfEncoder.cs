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
        public void Encode<TPixel>(Image<TPixel> image, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
        {
            using var encoder = new VtfEncoderCore();
            encoder.Encode(image, stream, default);
        }

        public async Task EncodeAsync<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
            where TPixel : unmanaged, IPixel<TPixel>
        {
            // The introduction of a local variable that refers to an object the implements
            // IDisposable means you must use async/await, where the compiler generates the
            // state machine and a continuation.
            using var encoder = new VtfEncoderCore();
            await encoder.EncodeAsync(image, stream, cancellationToken).ConfigureAwait(false);
        }
    }
}
