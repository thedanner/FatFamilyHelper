using Left4DeadHelper.Bindings.VtfLibNative.VtfFormat;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    public class VtfSaveProfile : BaseSaveProfile
    {
        public override int MaxWidth => 512;
        public override int MaxHeight => 512;
        public override string Extension => ".vtf";

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task ConvertAsync(Image<Rgba32> image, Stream outputStream, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));

            var saveToken = new VtfSaveConfigToken(VtfSaveConfigTemplate.VtfTemplateSprayWithAlpha);

            var vtfFile = new VtfFile();

            void createOptionsModifier(ref VtfLib.CreateOptions createOptions)
            {
                createOptions.uiResizeWidth =
                    createOptions.uiResizeClampWidth =
                    (uint)MaxWidth;

                createOptions.uiResizeHeight =
                    createOptions.uiResizeClampHeight =
                    (uint)MaxHeight;
            }
            vtfFile.Save(image, outputStream, saveToken, createOptionsModifier);
        }
    }
}
