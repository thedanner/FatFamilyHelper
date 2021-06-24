using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;

namespace Left4DeadHelper.Sprays.VtfFormat
{
	internal class VtfFile
	{
        #region Static initialization support for native library
        
        // From https://stackoverflow.com/a/18709110/91993

        static VtfFile()
        {
            var result = VtfLib.vlInitialize();
            System.Diagnostics.Debug.WriteLine("VtfLib.vlInitialize() returned " + result);
        }

        // ReSharper disable once UnusedMember.Local
        private static readonly Destructor Finalize = new Destructor();
        private sealed class Destructor
        {
            ~Destructor()
            {
                // One time only destructor.
                VtfLib.vlShutdown();
            }
        }

        #endregion

        public VtfFile()
        {

        }

        public delegate void CreateOptionsModifier(ref VtfLib.CreateOptions createOptions);

        public unsafe void Save(Image<Rgba32> image, Stream output, VtfSaveConfigToken token, CreateOptionsModifier createOptionsModifier)
		{
            if (image is null) throw new ArgumentNullException(nameof(image));
            if (output is null) throw new ArgumentNullException(nameof(output));
            if (token is null) throw new ArgumentNullException(nameof(token));

            uint uiImage;
            var result = VtfLib.vlCreateImage(&uiImage);
            if (!result)
            {
                throw new Exception("Failed to create VTF image. Did you call VtfLib.vlInitialize()?");
            }

            result = VtfLib.vlBindImage(uiImage);

            try
            {
                if (token.uiVersionMajor == 0)
                {
                    token.uiVersionMajor = 7;
                    token.uiVersionMinor = 2;
                }

                VtfLib.CreateOptions createOptions = token.GetCreateOptions();

                createOptionsModifier?.Invoke(ref createOptions);

                var lpImageData = new byte[image.Width * image.Height * 4];

                uint uiIndex = 0;

                var y = 0;
                for (y = 0; y < image.Height; y++)
                {
                    Span<Rgba32> row = image.GetPixelRowSpan(y);
                    if (row.Length != image.Width)
                    {
                        throw new Exception("Row length doesn't match image width");
                    }

                    for (var x = 0; x < image.Width; x++)
                    {
                        lpImageData[uiIndex++] = row[x].R;
                        lpImageData[uiIndex++] = row[x].G;
                        lpImageData[uiIndex++] = row[x].B;
                        lpImageData[uiIndex++] = row[x].A;
                    }
                }

                if (y != image.Height)
                {
                    throw new Exception("The number of rows processed doesn't match the image height.");
                }

                if (uiIndex != lpImageData.Length)
                {
                    throw new Exception("uiIndex isn't at the expected value.");
                }

                fixed (byte* lpBuffer = lpImageData)
                {
                    result = ValidateImageCreateSingle((uint)image.Width, (uint)image.Height, 1, 1, 1, lpBuffer, ref createOptions, out string? err);
                    if (!result) throw new Exception("Validation error: " + err);

                    if (!VtfLib.vlImageCreateSingle((uint)image.Width, (uint)image.Height, lpBuffer, ref createOptions))
                    {
                        throw new FormatException(VtfLib.vlGetLastError());
                    }
                }

                uint uiSize;
                byte[] lpOutput = new byte[VtfLib.vlImageGetSize()];
                fixed (byte* lpBuffer = lpOutput)
                {
                    if (!VtfLib.vlImageSaveLump(lpBuffer, (uint)lpOutput.Length, &uiSize))
                    {
                        throw new FormatException(VtfLib.vlGetLastError());
                    }
                }

                output.Write(lpOutput, 0, (int)uiSize);
            }
            finally
            {
                VtfLib.vlDeleteImage(uiImage);
            }
		}

        private const uint VTF_MAJOR_VERSION = 7;		//!< VTF major version number
        private const uint VTF_MINOR_VERSION = 5;		//!< VTF minor version number
        private const uint VTF_MINOR_VERSION_DEFAULT = 3;

        private const uint VTF_MINOR_VERSION_MIN_SPHERE_MAP = 1;
        private const uint VTF_MINOR_VERSION_MIN_VOLUME = 2;
        private const uint VTF_MINOR_VERSION_MIN_RESOURCE = 3;
        private const uint VTF_MINOR_VERSION_MIN_NO_SPHERE_MAP = 5;

        private unsafe bool ValidateImageCreateSingle(
            uint uiWidth, uint uiHeight, uint uiFrames, uint uiFaces, uint uiSlices, byte* lpImageDataRGBA8888, ref VtfLib.CreateOptions createOptions,
            out string? err)
        {
            uint uiCount = 0;
            if (uiFrames > uiCount)
                uiCount = uiFrames;
            if (uiFaces > uiCount)
                uiCount = uiFaces;
            if (uiSlices > uiCount)
                uiCount = uiSlices;
            //byte* lpNewImageDataRGBA8888;

            if ((uiFrames == 1 && uiFaces > 1 && uiSlices > 1) || (uiFrames > 1 && uiFaces == 1 && uiSlices > 1) || (uiFrames > 1 && uiFaces > 1 && uiSlices == 1))
            {
                err = "Invalid image frame, face and slice count combination.  Function does not support images with any combination of multiple frames or faces or slices.";
                return false;
            }

            if (createOptions.uiVersionMajor != VTF_MAJOR_VERSION || (createOptions.uiVersionMinor < 0 || createOptions.uiVersionMinor > VTF_MINOR_VERSION))
            {
                err = string.Format("File version {0}.{1} does not match {2}.{3} to {4}.{5}.", createOptions.uiVersionMajor, createOptions.uiVersionMinor, VTF_MAJOR_VERSION, 0, VTF_MAJOR_VERSION, VTF_MINOR_VERSION);
                return false;
            }

            if (createOptions.uiVersionMajor == VTF_MAJOR_VERSION && createOptions.uiVersionMinor < VTF_MINOR_VERSION_MIN_VOLUME && uiSlices > 1)
            {
                err = string.Format("Volume textures are only supported in version {0}.{1} and up.", VTF_MAJOR_VERSION, VTF_MINOR_VERSION_MIN_VOLUME);
                return false;
            }

            if (createOptions.uiVersionMajor == VTF_MAJOR_VERSION && createOptions.uiVersionMinor < VTF_MINOR_VERSION_MIN_SPHERE_MAP && uiFaces == 7)
            {
                err = string.Format("Sphere maps are only supported in version %d.%d and up.", VTF_MAJOR_VERSION, VTF_MINOR_VERSION_MIN_SPHERE_MAP);
                return false;
            }

            if (createOptions.bMipmaps && uiSlices > 1)
            {
                err = string.Format("Mipmap generation for depth textures is not supported.");
                return false;
            }

            err = null;
            return true;
        }
    }
}
