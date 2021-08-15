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
    public class VtfSprayTests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            VtfLib.vlInitialize();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            VtfLib.vlShutdown();
        }

        [Test]
        public async Task RawConvert()
        {
            var file = new VtfFile();

            using var image = await Image.LoadAsync<Rgba32>("memes.png");

            using var output = new FileStream("memes-raw.vtf", FileMode.OpenOrCreate, FileAccess.Write);

            file.Save(image, output, new VtfSaveConfigToken(VtfSaveConfigTemplate.VtfTemplateSprayWithAlpha), null);
        }

        [Test]
        public async Task SprayToolsConvert()
        {
            var sprayTools = new SprayTools();

            using var inputStream = new FileStream("memes.png", FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream("memes-st.vtf", FileMode.OpenOrCreate, FileAccess.Write);

            await sprayTools.ConvertAsync(inputStream, outputStream, new Vtf512SaveProfile(), CancellationToken.None);
        }
    }
}
