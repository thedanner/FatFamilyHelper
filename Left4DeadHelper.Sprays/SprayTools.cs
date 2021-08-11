using Left4DeadHelper.Sprays.Exceptions;
using Left4DeadHelper.Sprays.SaveProfiles;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
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

            saveProfile.ClampDimensions(image);
            
            await saveProfile.ConvertAsync(image, outputStream, cancellationToken);

            var result = new ConversionResult(saveProfile.Extension);

            return result;
        }
    }
}
