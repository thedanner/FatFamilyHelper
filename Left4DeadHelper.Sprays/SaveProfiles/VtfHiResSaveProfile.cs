using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.Sprays.SaveProfiles
{
    public class VtfHiResSaveProfile : BaseSaveProfile
    {
        static VtfHiResSaveProfile()
        {
            // IL refers to the base library for loading, saving and converting images.
            DevIL.il.IlInit();
            
            // ILU refers to the middle level library for image manipulation.
            DevIL.ilu.IluInit();

            // ILUT refers to the high level library for displaying images.
            //DevIL.ilut.IlutRenderer(Constants.ILUT_DIRECT3D10);

            // Functions in IL, ILU and ILUT are prefixed by ‘il’, ‘ilu’ and ‘ilut’, respectively
        }


        // With hi-res, the dimensions must be 1024x1020 or vice versa.
        public override int MaxWidth => 1024;
        public override int MaxHeight => 1024;
        public override string Extension => ".vtf";

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task ConvertAsync(Image<Rgba32> image, Stream outputStream, CancellationToken cancellationToken)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (image is null) throw new ArgumentNullException(nameof(image));
            if (outputStream is null) throw new ArgumentNullException(nameof(outputStream));

            var imagePointer = DevIL.il.IlGenImage();
            try
            {
                DevIL.il.IlSaveL();

                throw new NotImplementedException("TODO");
            }
            finally
            {
                if (imagePointer != 0)
                {
                    DevIL.il.IlDeleteImage(imagePointer);
                }
            }
        }
    }
}
