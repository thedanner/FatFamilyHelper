using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Left4DeadHelper.Sprays;
using Left4DeadHelper.Sprays.SaveProfiles;
using NUnit.Framework;

namespace Left4DeadHelper.Tests.Unit.Sprays
{
    [TestFixture]
    public class VtfHiResSprayTests
    {
        [Test]
        public async Task SprayToolsHiResConvert()
        {
            var sprayTools = new SprayTools();

            using var inputStream = new FileStream("memes.png", FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream("memes-st-hq.vtf", FileMode.OpenOrCreate, FileAccess.Write);

            await sprayTools.ConvertAsync(inputStream, outputStream, new VtfHiResSaveProfile(), CancellationToken.None);
        }
    }
}
