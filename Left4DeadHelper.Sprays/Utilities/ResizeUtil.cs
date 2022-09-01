using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

namespace Left4DeadHelper.Sprays.Utilities;

public static class ResizeUtil
{
    public static void Resize(Image<Rgba32> image, int maxWidth, int maxHeight)
    {
        if (image is null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        if (maxWidth <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxWidth), "Width must be positive.");
        }
        if (maxHeight <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxHeight), "Height must be positive.");
        }

        // So the spary takes up full size, make the larger dimension the size of the max allowed.
        // This could make the source bigger or smaller.
        if (image.Width != maxWidth || image.Height != maxHeight)
        {
            double scaleFactor;

            if (image.Width >= image.Height)
            {
                scaleFactor = 1.0 * maxWidth / image.Width;
            }
            else
            {
                scaleFactor = 1.0 * maxHeight / image.Height;
            }

            image.Mutate(x => x.Resize(
                (int)Math.Floor(image.Width * scaleFactor),
                (int)Math.Floor(image.Height * scaleFactor)
            ));
        }

        if (image.Width != maxWidth || image.Height != maxHeight)
        {
            var resizeOptions = new ResizeOptions
            {
                Size = new Size
                {
                    Width = maxWidth,
                    Height = maxHeight
                },
                Mode = ResizeMode.Manual,
                Position = AnchorPositionMode.TopLeft,
                TargetRectangle = new Rectangle
                {
                    X = (maxWidth - image.Width) / 2,
                    Y = (maxHeight - image.Height) / 2,
                    Width = image.Width,
                    Height = image.Height
                },
                Compand = false,
            };

            image.Mutate(x => x
                .Resize(resizeOptions)
                .BackgroundColor(Color.Transparent));
        }
    }
}
