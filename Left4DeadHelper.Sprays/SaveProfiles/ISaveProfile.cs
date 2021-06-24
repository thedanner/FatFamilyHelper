using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    public interface ISaveProfile
    {
        int MaxWidth { get; }
        int MaxHeight { get; }
        string Extension { get; }

        void Validate();

        Task ConvertAsync(Image<Rgba32> image, Stream outputStream, CancellationToken cancellationToken);
    }
}
