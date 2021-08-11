using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    public interface ISaveProfile
    {
        string Extension { get; }

        void ClampDimensions(Image<Rgba32> image);
        void Validate();

        Task ConvertAsync(Image<Rgba32> image, Stream outputStream, CancellationToken cancellationToken);
    }
}
