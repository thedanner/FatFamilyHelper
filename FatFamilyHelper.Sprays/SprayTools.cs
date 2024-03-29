﻿using FatFamilyHelper.Sprays.Exceptions;
using FatFamilyHelper.Sprays.SaveProfiles;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FatFamilyHelper.Sprays;

public class SprayTools
{
    public static async Task<ConversionResult> ConvertAsync(IList<Stream> inputStreams, Stream outputStream,
        ISaveProfile saveProfile, CancellationToken cancellationToken)
    {
        if (inputStreams is null) throw new ArgumentNullException(nameof(inputStreams));
        if (inputStreams.Count == 0)
        {
            throw new ArgumentException("The collection of input streams must not be empty.", nameof(inputStreams));
        }
        if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));
        if (saveProfile is null) throw new ArgumentNullException(nameof(saveProfile));

        saveProfile.Validate();

        var memoryStreamTasks = inputStreams.Select(async i =>
        {
            var memoryStream = new MemoryStream();
            await i.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var inputFormat = await Image.DetectFormatAsync(memoryStream);
            memoryStream.Position = 0;
            var inputMimeTypes = inputFormat.MimeTypes.ToList();
            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/x-tga", "image/x-targa" };

            if (!allowedMimeTypes.Intersect(inputMimeTypes).Any())
            {
                throw new UnsupportedImageFormatException(inputMimeTypes);
            }

            return memoryStream;
        });

        var memoryStreams = await Task.WhenAll(memoryStreamTasks);

        var imageTasks = memoryStreams.Select(ms => Image.LoadAsync<Rgba32>(new DecoderOptions(), ms, cancellationToken));

        var images = await Task.WhenAll(imageTasks);

        if (images.Length > 1)
        {
            foreach (var extraImage in images.Skip(1))
            {
                if (images[0].Width != extraImage.Width
                    || images[0].Height != extraImage.Height)
                {
                    return ConversionResult.Fail("all images must have the same dimensions");
                }
            }
        }

        await saveProfile.ConvertAsync(images, outputStream, cancellationToken);

        var result = ConversionResult.Pass(saveProfile.Extension);

        return result;
    }
}
