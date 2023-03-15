using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FatFamilyHelper.ImageSharpExtensions.Formats.Vtf;

public sealed class VtfEncoder : IImageEncoder
{
    private readonly VtfImageType _imageType;

    public VtfEncoder(VtfImageType imageType)
    {
        _imageType = imageType;
    }

    private readonly bool _skipMetadata = true;
    public bool SkipMetadata
    {
        get => _skipMetadata;
        init => _skipMetadata = true;
    }

    public Task EncodeWithMipmapsAsync(IList<Image<Rgba32>> images, Stream stream, CancellationToken cancellationToken)
    {
        var encoder = new VtfEncoderCore(_imageType);
        return encoder.EncodeAsync(images, stream, cancellationToken);
    }

    public void Encode<TPixel>(Image<TPixel> image, Stream stream) where TPixel : unmanaged, IPixel<TPixel>
    {
        if (_imageType != VtfImageType.Single1024
            && _imageType != VtfImageType.Single512)
        {
            throw new Exception($"Use {nameof(EncodeWithMipmapsAsync)}() directly for images that require mipmaps.");
        }

        var encoder = new VtfEncoderCore(_imageType);
        if (!(image is Image<Rgba32> imageAsRgba32)) imageAsRgba32 = image.CloneAs<Rgba32>();
        encoder.Encode(imageAsRgba32, stream, default);
    }

    public async Task EncodeAsync<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken)
        where TPixel : unmanaged, IPixel<TPixel>
    {
        if (_imageType != VtfImageType.Single1024
            && _imageType != VtfImageType.Single512)
        {
            throw new Exception($"Use {nameof(EncodeWithMipmapsAsync)}() directly for images that require mipmaps.");
        }

        var encoder = new VtfEncoderCore(_imageType);
        if (!(image is Image<Rgba32> imageAsRgba32)) imageAsRgba32 = image.CloneAs<Rgba32>();
        await encoder.EncodeAsync(imageAsRgba32, stream, cancellationToken).ConfigureAwait(false);
    }
}
