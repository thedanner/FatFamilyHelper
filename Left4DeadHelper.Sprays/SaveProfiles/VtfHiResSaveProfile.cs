using Left4DeadHelper.Sprays.VtfFormat;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    internal class VtfHiResSaveProfile : BaseSaveProfile
    {
        // TODO one of the dimensions should be 1020 to fit under the 512 KiB limit.
        public override int MaxWidth => 1024;
        public override int MaxHeight => 1024;
        public override string Extension => ".vtf";

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task ConvertAsync(Image<Rgba32> image, Stream outputStream, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));

            var saveToken = new VtfSaveConfigToken(VtfSaveConfigTemplate.VtfTemplateSprayWithAlpha)
            {
                eImageFormat = VtfLib.ImageFormat.ImageFormatDXT1
            };

            var vtfFile = new VtfFile();

            void createOptionsModifier(ref VtfLib.CreateOptions createOptions)
            {
                createOptions.bResize = false;

                createOptions.uiResizeWidth =
                    createOptions.uiResizeClampWidth =
                    (uint)image.Width;

                createOptions.uiResizeHeight =
                    createOptions.uiResizeClampHeight =
                    (uint)image.Height;
            }
            vtfFile.Save(image, outputStream, saveToken, createOptionsModifier);
        }
    }
}
