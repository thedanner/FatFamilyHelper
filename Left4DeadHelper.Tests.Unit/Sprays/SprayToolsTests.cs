using System;
using System.Collections.Generic;
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
        [TestCase(@"test_images\aaa-memes.png", "zzz-memes.tga")]
        [TestCase(@"test_images\aaa-alphatest.png", "zzz-alphatest.tga")]
        public async Task ConvertAsync_TgaWithValidImage_GeneratesValidImage(string inputFileName, string outputFileName)
        {
            ValidateArgs(inputFileName, outputFileName);

            var sprayTools = new SprayTools();

            using var inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write);

            var inputStreams = new List<Stream> { inputStream };

            await sprayTools.ConvertAsync(inputStreams, outputStream, new TgaSaveProfile(), CancellationToken.None);
        }

        [Test]
        [TestCase(@"test_images\aaa-memes.png", "zzz-memes-512.vtf")]
        [TestCase(@"test_images\aaa-alphatest.png", "zzz-alphatest-512.vtf")]
        public async Task ConvertAsync_Vtf512WithValidImage_GeneratesValidImage(string inputFileName, string outputFileName)
        {
            var sprayTools = new SprayTools();

            using var inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write);

            var inputStreams = new List<Stream> { inputStream };

            await sprayTools.ConvertAsync(inputStreams, outputStream, new Vtf512SaveProfile(), CancellationToken.None);
        }

        [Test]
        [TestCase(@"test_images\aaa-memes.png", "zzz-memes-1024.vtf")]
        [TestCase(@"test_images\aaa-alphatest.png", "zzz-alphatest-1024.vtf")]
        public async Task ConvertAsync_Vtf1024WithValidImage_GeneratesValidImage(string inputFileName, string outputFileName)
        {
            var sprayTools = new SprayTools();

            using var inputStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write);

            var inputStreams = new List<Stream> { inputStream };

            await sprayTools.ConvertAsync(inputStreams, outputStream, new Vtf1024SaveProfile(), CancellationToken.None);
        }

        [Test]
        public async Task ConvertAsync_FadingWithValidImages_GeneratesValidImage()
        {
            var sprayTools = new SprayTools();

            using var nearInputStream = new FileStream(@"test_images\aaa-fade-cookie-scary.png", FileMode.Open, FileAccess.Read);
            using var farInputStream = new FileStream(@"test_images\aaa-fade-cookie-normal.png", FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream("zzz-fade-test.vtf", FileMode.OpenOrCreate, FileAccess.Write);

            var inputStreams = new List<Stream> { nearInputStream, farInputStream };

            await sprayTools.ConvertAsync(inputStreams, outputStream, new VtfFadingSaveProfile(), CancellationToken.None);
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
