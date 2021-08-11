using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    public abstract class BaseSaveProfile : ISaveProfile
    {
        public abstract int MaxWidth { get; }
        public abstract int MaxHeight { get; }
        public abstract string Extension { get; }


        public virtual void ClampDimensions(Image<Rgba32> image)
        {
            ClampDimensions(image, MaxWidth, MaxHeight);
        }

        protected virtual void ClampDimensions(Image<Rgba32> image, int maxWidth, int maxHeight)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
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

        public abstract Task ConvertAsync(Image<Rgba32> image, Stream outputStream, CancellationToken cancellationToken);
    }
}
