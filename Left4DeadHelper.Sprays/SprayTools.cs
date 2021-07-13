using Left4DeadHelper.Sprays.Exceptions;
using Left4DeadHelper.Sprays.SaveProfiles;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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
        // TODO: support animated sprays
        // TODO: optionally support VTF format?
        // TODO: support creating near/far sprays (requires VTF format support)

        public async Task<ConversionResult> ConvertAsync(Stream inputStream, Stream outputStream, ISaveProfile saveProfile,
            CancellationToken cancellationToken)
        {
            if (inputStream == null) throw new ArgumentNullException(nameof(inputStream));
            if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));
            if (saveProfile is null) throw new ArgumentNullException(nameof(saveProfile));

            saveProfile.Validate();

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

            using var image = await Image.LoadAsync<Rgba32>(Configuration.Default, memoryStream, cancellationToken);

            // So the spary takes up full size, make the larger dimension the size of the max allowed.
            // This could make the source bigger or smaller.
            if (image.Width != saveProfile.MaxWidth || image.Height != saveProfile.MaxHeight)
            {
                double scaleFactor;

                if (image.Width >= image.Height)
                {
                    scaleFactor = 1.0 * saveProfile.MaxWidth / image.Width;
                }
                else
                {
                    scaleFactor = 1.0 * saveProfile.MaxHeight / image.Height;
                }

                image.Mutate(x => x.Resize(
                    (int)Math.Floor(image.Width * scaleFactor),
                    (int)Math.Floor(image.Height * scaleFactor)
                ));
            }

            if (image.Width != saveProfile.MaxWidth || image.Height != saveProfile.MaxHeight)
            {
                var resizeOptions = new ResizeOptions
                {
                    Size = new Size
                    {
                        Width = saveProfile.MaxWidth,
                        Height = saveProfile.MaxHeight
                    },
                    Mode = ResizeMode.Manual,
                    Position = AnchorPositionMode.TopLeft,
                    TargetRectangle = new Rectangle
                    {
                        X = (saveProfile.MaxWidth - image.Width) / 2,
                        Y = (saveProfile.MaxHeight - image.Height) / 2,
                        Width = image.Width,
                        Height = image.Height
                    },
                    Compand = false,
                };

                image.Mutate(x => x
                    .Resize(resizeOptions)
                    .BackgroundColor(Color.Transparent));
            }

            await saveProfile.ConvertAsync(image, outputStream, cancellationToken);

            var result = new ConversionResult(saveProfile.Extension);

            return result;
        }
    }
}
