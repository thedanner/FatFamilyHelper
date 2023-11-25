using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FatFamilyHelper.Sprays;
using FatFamilyHelper.Sprays.SaveProfiles;
using NUnit.Framework;

namespace FatFamilyHelper.Tests.Unit.Sprays;

[TestFixture]
public class SprayToolsTests
{
    private const string BaseTestImagesDirectory = "test_images";

    [Test]
    [TestCase(@"aaa-memes.png", "zzz-memes.tga")]
    [TestCase(@"aaa-alphatest.png", "zzz-alphatest.tga")]
    public async Task ConvertAsync_TgaWithValidImage_GeneratesValidImage(string inputFileName, string outputFileName)
    {
        ValidateArgs(inputFileName, outputFileName);

        var sprayTools = new SprayTools();

        using var inputStream = new FileStream(Path.Combine(BaseTestImagesDirectory, inputFileName), FileMode.Open, FileAccess.Read);
        using var outputStream = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write);

        var inputStreams = new List<Stream> { inputStream };

        await SprayTools.ConvertAsync(inputStreams, outputStream, new TgaSaveProfile(), CancellationToken.None);
    }

    [Test]
    [TestCase(@"aaa-memes.png", "zzz-memes-512.vtf")]
    [TestCase(@"aaa-alphatest.png", "zzz-alphatest-512.vtf")]
    public async Task ConvertAsync_Vtf512WithValidImage_GeneratesValidImage(string inputFileName, string outputFileName)
    {
        var sprayTools = new SprayTools();

        using var inputStream = new FileStream(Path.Combine(BaseTestImagesDirectory, inputFileName), FileMode.Open, FileAccess.Read);
        using var outputStream = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write);

        var inputStreams = new List<Stream> { inputStream };

        await SprayTools.ConvertAsync(inputStreams, outputStream, new Vtf512SaveProfile(), CancellationToken.None);
    }

    [Test]
    [TestCase(@"aaa-memes.png", "zzz-memes-1024.vtf")]
    [TestCase(@"aaa-alphatest.png", "zzz-alphatest-1024.vtf")]
    public async Task ConvertAsync_Vtf1024WithValidImage_GeneratesValidImage(string inputFileName, string outputFileName)
    {
        var sprayTools = new SprayTools();

        using var inputStream = new FileStream(Path.Combine(BaseTestImagesDirectory, inputFileName), FileMode.Open, FileAccess.Read);
        using var outputStream = new FileStream(outputFileName, FileMode.OpenOrCreate, FileAccess.Write);

        var inputStreams = new List<Stream> { inputStream };

        await SprayTools.ConvertAsync(inputStreams, outputStream, new Vtf1024SaveProfile(), CancellationToken.None);
    }

    [Test]
    public async Task ConvertAsync_FadingWithValidImages_GeneratesValidImage()
    {
        var sprayTools = new SprayTools();

        using var nearInputStream = new FileStream(
            Path.Combine(BaseTestImagesDirectory, "aaa-fade-cookie-scary.png"),
            FileMode.Open, FileAccess.Read);
        using var farInputStream = new FileStream(
            Path.Combine(BaseTestImagesDirectory, "aaa-fade-cookie-normal.png"),
            FileMode.Open, FileAccess.Read);
        using var outputStream = new FileStream("zzz-fade-test.vtf", FileMode.OpenOrCreate, FileAccess.Write);

        var inputStreams = new List<Stream> { nearInputStream, farInputStream };

        await SprayTools.ConvertAsync(inputStreams, outputStream, new VtfFadingSaveProfile(), CancellationToken.None);
    }


    private static void ValidateArgs(string inputFileName, string outputFileName)
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
