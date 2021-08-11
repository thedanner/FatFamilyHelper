using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Left4DeadHelper.Bindings.VtfLibNative.VtfFormat;
using Left4DeadHelper.Sprays;
using Left4DeadHelper.Sprays.SaveProfiles;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Left4DeadHelper.Tests.Unit.Sprays
{
    [TestFixture]
    public class TgaSprayTests
    {
        [Test]
        public async Task TgaConvert()
        {
            var sprayTools = new SprayTools();

            using var inputStream = new FileStream("memes.png", FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream("memes-old.tga", FileMode.OpenOrCreate, FileAccess.Write);

            await sprayTools.ConvertAsync(inputStream, outputStream, new TgaSaveProfile(), CancellationToken.None);
        }
    }
}
