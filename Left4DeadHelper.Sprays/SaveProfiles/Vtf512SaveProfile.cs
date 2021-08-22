using Left4DeadHelper.ImageSharpExtensions.Formats.Vtf;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    public class Vtf512SaveProfile : BaseSaveProfile<SingleImageConfiguration>
    {
        public override int MaxWidth => 512;
        public override int MaxHeight => 512;
        public override string Extension => ".vtf";

        public override async Task ConvertAsync(SingleImageConfiguration imageConfiguration, Stream outputStream, CancellationToken cancellationToken)
        {
            if (imageConfiguration is null) throw new ArgumentNullException(nameof(imageConfiguration));
            if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));

            var image = imageConfiguration.Image;

            Resize(image);

            var encoder = new VtfEncoder(VtfImageType.Single512);

            await image.SaveAsync(outputStream, encoder, cancellationToken);
        }
    }
}
