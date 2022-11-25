using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FatFamilyHelper.Sprays.SaveProfiles;

public interface ISaveProfile
{
    string Extension { get; }

    void Validate();

    Task ConvertAsync(IList<Image<Rgba32>> images, Stream outputStream, CancellationToken cancellationToken);
}
