﻿using FatFamilyHelper.Sprays.Utilities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace FatFamilyHelper.Sprays.SaveProfiles;

public abstract class BaseSaveProfile : ISaveProfile
{
    public abstract int MaxWidth { get; }
    public abstract int MaxHeight { get; }
    public abstract string Extension { get; }

    protected virtual void Resize(Image<Rgba32> image)
    {
        ResizeUtil.Resize(image, MaxWidth, MaxHeight);
    }

    public void Validate()
    {
        if (MaxWidth <= 0)
        {
            throw new Exception($"{nameof(MaxWidth)} must be positive.");
        }

        if (MaxHeight <= 0)
        {
            throw new Exception($"{nameof(MaxHeight)} must be positive.");
        }

        if (string.IsNullOrEmpty(Extension))
        {
            throw new Exception($"{nameof(Extension)} must be provided.");
        }

        if (Extension[0] != '.')
        {
            throw new Exception($"{nameof(Extension)} must start with a dot.");
        }

        if (Extension.Length < 2)
        {
            throw new Exception($"{nameof(Extension)} must be at least two characters long.");
        }
    }

    public abstract Task ConvertAsync(IList<Image<Rgba32>> images, Stream outputStream, CancellationToken cancellationToken);
}
