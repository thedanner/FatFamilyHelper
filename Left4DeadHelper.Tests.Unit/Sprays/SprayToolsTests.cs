using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Left4DeadHelper.Sprays;
using Left4DeadHelper.Sprays.SaveProfiles;
using NUnit.Framework;

namespace Left4DeadHelper.Tests.Unit.Sprays
{
    [TestFixture]
    public class SprayToolsTests
    {
        [Test]
        public async Task ConvertAsync_TgaWithValidImage_GeneratesValidImage()
        {
            var sprayTools = new SprayTools();

            using var inputStream = new FileStream("memes.png", FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream("zzz-memes.tga", FileMode.OpenOrCreate, FileAccess.Write);

            await sprayTools.ConvertAsync(inputStream, outputStream, new TgaSaveProfile(), CancellationToken.None);
        }

        [Test]
        public async Task ConvertAsync_Vtf512WithValidImage_GeneratesValidImage()
        {
            var sprayTools = new SprayTools();

            using var inputStream = new FileStream("memes.png", FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream("zzz-memes.vtf", FileMode.OpenOrCreate, FileAccess.Write);

            await sprayTools.ConvertAsync(inputStream, outputStream, new Vtf512SaveProfile(), CancellationToken.None);
        }

        [Test]
        public async Task ConvertAsync_Vtf1024WithValidImage_GeneratesValidImage()
        {
            var sprayTools = new SprayTools();

            using var inputStream = new FileStream("memes.png", FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream("zzz-memes-hq.vtf", FileMode.OpenOrCreate, FileAccess.Write);

            await sprayTools.ConvertAsync(inputStream, outputStream, new Vtf1024SaveProfile(), CancellationToken.None);
        }
    }
}
