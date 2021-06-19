using Left4DeadHelper.Sprays.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Sprays
{
    public class SprayTools
    {
        private const int MaxWidth = 256;
        private const int MaxHeight = 256;

        // TODO: support animated sprays
        // TODO: optionally support VDF format?
        // TODO: support creating near/far sprays (requires VDF format support)

        public async Task<ConversionResult> ConvertAsync(Stream inputStream, CancellationToken cancellationToken)
        {
            if (inputStream == null) throw new ArgumentNullException(nameof(inputStream));

            var memoryStream = new MemoryStream();
            await inputStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var inputFormat = Image.DetectFormat(memoryStream);
            memoryStream.Position = 0;
            var inputMimeTypes = inputFormat.MimeTypes.ToList();
            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/x-tga", "image/x-targa" };

            if (!allowedMimeTypes.Intersect(inputMimeTypes).Any())
            {
                throw new UnsupportedImageFormatException(inputMimeTypes);
            }

            using var image = await Image.LoadAsync(Configuration.Default, memoryStream, cancellationToken);

            // If too big, scale down. Could be done with a transform below but this is simpler.
            if (image.Width > MaxWidth || image.Height > MaxHeight)
            {
                double scaleFactor;

                if (image.Width >= image.Height)
                {
                    scaleFactor = 1.0 * MaxWidth / image.Width;
                }
                else
                {
                    scaleFactor = 1.0 * MaxHeight / image.Height;
                }

                image.Mutate(x => x.Resize(
                    (int)Math.Floor(image.Width * scaleFactor),
                    (int)Math.Floor(image.Height * scaleFactor)
                ));
            }

            if (image.Width != MaxWidth || image.Height != MaxHeight)
            {
                var resizeOptions = new ResizeOptions
                {
                    Size = new Size
                    {
                        Width = MaxWidth,
                        Height = MaxHeight
                    },
                    Mode = ResizeMode.Manual,
                    Position = AnchorPositionMode.TopLeft,
                    TargetRectangle = new Rectangle
                    {
                        X = (MaxWidth - image.Width) / 2,
                        Y = (MaxHeight - image.Height) / 2,
                        Width = image.Width,
                        Height = image.Height
                    },
                    Compand = false,
                };

                image.Mutate(x => x
                    .Resize(resizeOptions)
                    .BackgroundColor(Color.Transparent));
            }

            var encoder = new TgaEncoder
            {
                BitsPerPixel = TgaBitsPerPixel.Pixel32,
                Compression = TgaCompression.None,
            };

            var outputStream = new MemoryStream();
            await image.SaveAsync(outputStream, encoder, cancellationToken);

            outputStream.Position = 0;

            var result = new ConversionResult(outputStream, ".tga");

            return result;
        }
    }
}
