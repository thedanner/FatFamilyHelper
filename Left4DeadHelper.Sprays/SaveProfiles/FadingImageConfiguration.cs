using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    public class FadingImageConfiguration
    {
        public FadingImageConfiguration(IList<Image<Rgba32>> images)
        {
            Validate(images);

            Images = images;
        }

        private void Validate(IList<Image<Rgba32>>? images)
        {
            if (images == null) throw new ArgumentNullException(nameof(images));

            if (images.Count == 0)
            {
                throw new ArgumentException("The collection of images must not be empty.", nameof(images));
            }
        }

        public IList<Image<Rgba32>> Images { get; }

        public static FadingImageConfiguration Create(params Image<Rgba32>[] nearToFar)
        {
            return new FadingImageConfiguration(nearToFar);
        }

        public static FadingImageConfiguration Create(Image<Rgba32> near, Image<Rgba32> far)
        {
            if (near is null)
            {
                throw new ArgumentNullException(nameof(near));
            }

            if (far is null)
            {
                throw new ArgumentNullException(nameof(far));
            }

            return new FadingImageConfiguration(new List<Image<Rgba32>>
            {
                near,
                far
            });
        }
    }
}
