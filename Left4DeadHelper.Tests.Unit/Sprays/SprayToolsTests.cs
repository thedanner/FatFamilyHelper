using System;
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
        [TestCase("memes.png", "zzz-memes.tga")]
        [TestCase("alphatest.png", "zzz-alphatest.tga")]
        public async Task ConvertAsync_TgaWithValidImage_GeneratesValidImage(string inputFileName, string outputFileName)
        {
            ValidateArgs(inputFileName, outputFileName);

            var sprayTools = new SprayTools();

            using var inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write);

            await sprayTools.ConvertAsync(inputStream, outputStream, new TgaSaveProfile(), CancellationToken.None);
        }

        [Test]
        [TestCase("memes.png", "zzz-memes-512.vtf")]
        [TestCase("alphatest.png", "zzz-alphatest-512.vtf")]
        public async Task ConvertAsync_Vtf512WithValidImage_GeneratesValidImage(string inputFileName, string outputFileName)
        {
            var sprayTools = new SprayTools();

            using var inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write);

            await sprayTools.ConvertAsync(inputStream, outputStream, new Vtf512SaveProfile(), CancellationToken.None);
        }

        [Test]
        [TestCase("memes.png", "zzz-memes-1024.vtf")]
        [TestCase("alphatest.png", "zzz-alphatest-1024.vtf")]
        public async Task ConvertAsync_Vtf1024WithValidImage_GeneratesValidImage(string inputFileName, string outputFileName)
        {
            var sprayTools = new SprayTools();

            using var inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write);

            await sprayTools.ConvertAsync(inputStream, outputStream, new Vtf1024SaveProfile(), CancellationToken.None);
        }


        private void ValidateArgs(string inputFileName, string outputFileName)
        {
            if (string.IsNullOrWhiteSpace(inputFileName))
            {
                throw new ArgumentException($"'{nameof(inputFileName)}' cannot be null or whitespace.", nameof(inputFileName));
            }
            if (string.IsNullOrWhiteSpace(outputFileName))
            {
                throw new ArgumentException($"'{nameof(outputFileName)}' cannot be null or whitespace.", nameof(outputFileName));
            }
        }
    }
}
