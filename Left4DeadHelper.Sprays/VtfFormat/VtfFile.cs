using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;

namespace Left4DeadHelper.Sprays.VtfFormat
{
	internal class VtfFile
	{
        public unsafe void Save(Image image, Stream output, VtfSaveConfigToken token)
		{
            if (image is null) throw new ArgumentNullException(nameof(image));
            if (output is null) throw new ArgumentNullException(nameof(output));
            if (token is null) throw new ArgumentNullException(nameof(token));

            uint uiImage;
            VtfLib.vlCreateImage(&uiImage);
            VtfLib.vlBindImage(uiImage);

            try
            {
                VtfLib.CreateOptions CreateOptions = token.GetCreateOptions();

                var lpImageData = new byte[image.Width * image.Height * 4];

                uint uiIndex = 0;

                var y = 0;
                // https://github.com/SixLabors/ImageSharp/blob/5b5bb110ca0271f75eda400fdf7d22b919a8ab70/src/ImageSharp/PixelFormats/PixelImplementations/RgbaVector.cs#L112-L119
                // Vector4(X, Y, Z, W) = RGBA
                image.Mutate(c => c.ProcessPixelRowsAsVector4(row =>
                {
                    for (int x = 0; x < row.Length; x++)
                    {
                        // Each pixel must be in RGBA32 format.
                        // From Vector4, that's the properties X, Y, Z, W, in that order.
                        lpImageData[uiIndex++] = (byte)Math.Round(row[x].X * 255.0);
                        lpImageData[uiIndex++] = (byte)Math.Round(row[x].Y * 255.0);
                        lpImageData[uiIndex++] = (byte)Math.Round(row[x].Z * 255.0);
                        lpImageData[uiIndex++] = (byte)Math.Round(row[x].W * 255.0);
                    }
                    y++;
                }));

                fixed (byte* lpBuffer = lpImageData)
                {
                    if (!VtfLib.vlImageCreateSingle((uint)image.Width, (uint)image.Height, lpBuffer, ref CreateOptions))
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
	}
}
