﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FatFamilyHelper.Sprays.SaveProfiles;

public class TgaSaveProfile : BaseSaveProfile
{
    public override int MaxWidth => 256;
    public override int MaxHeight => 256;
    public override string Extension => ".tga";

    public override async Task ConvertAsync(IList<Image<Rgba32>> images,
        Stream outputStream, CancellationToken cancellationToken)
    {
        if (images is null) throw new ArgumentNullException(nameof(images));
        if (images.Count != 1) throw new ArgumentException("Only one image is permitted for this format.", nameof(images));
        if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));

        var image = images[0];

        Resize(image);

        var encoder = new TgaEncoder
        {
            BitsPerPixel = TgaBitsPerPixel.Pixel32,
            Compression = TgaCompression.None,
        };

        await image.SaveAsync(outputStream, encoder, cancellationToken);
    }
}
