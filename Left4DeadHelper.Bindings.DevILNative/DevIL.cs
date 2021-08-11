using Left4DeadHelper.Bindings.DevILNative.Bindings;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Left4DeadHelper.Bindings.DevILNative
{
    public class DevIL : IDisposable
    {
        #region Static initialization support for native library

        // From https://stackoverflow.com/a/18709110/91993

        static DevIL()
        {
            // IL refers to the base library for loading, saving and converting images.
            Il.Init();

            // ILU refers to the middle level library for image manipulation.
            Ilu.Init();

            // ILUT refers to the high level library for displaying images.
            //DevIL.ilut.IlutRenderer(Constants.ILUT_DIRECT3D10);

            // Functions in IL, ILU and ILUT are prefixed by ‘il’, ‘ilu’ and ‘ilut’, respectively
        }

#pragma warning disable IDE0052 // Remove unread private members
        private static readonly Destructor StaticFinalizer = new Destructor();
#pragma warning restore IDE0052 // Remove unread private members
        private sealed class Destructor
        {
            ~Destructor()
            {
                // One time only destructor.
                Il.ShutDown();
            }
        }

        #endregion


        private bool _hasImage;
        private uint _image;

        public void LoadImage(Image<Rgba32> image)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var imageData = new byte[image.Width * image.Height * 4];

            uint uiIndex = 0;

            int y;
            for (y = 0; y < image.Height; y++)
            {
                Span<Rgba32> row = image.GetPixelRowSpan(y);
                if (row.Length != image.Width)
                {
                    throw new Exception("Row length doesn't match image width");
                }

                for (var x = 0; x < image.Width; x++)
                {
                    imageData[uiIndex++] = row[x].R;
                    imageData[uiIndex++] = row[x].G;
                    imageData[uiIndex++] = row[x].B;
                    imageData[uiIndex++] = row[x].A;
                }
            }

            if (y != image.Height)
            {
                throw new Exception("The number of rows processed doesn't match the image height.");
            }

            if (uiIndex != imageData.Length)
            {
                throw new Exception("uiIndex isn't at the expected value.");
            }

            LoadImageRgbaData(image.Width, image.Height, imageData);
        }

        public unsafe void LoadImageRgbaData(int width, int height, byte[] data)
        {
            if (_hasImage)
            {
                throw new InvalidOperationException("An image is already loaded. Create a new instance to operate on a different image.");
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            _image = Il.GenImage();
            _hasImage = true;
            Il.BindImage(_image);
            //Il.TexImage(width, height, 0, 4, Il.DataFormat.Rgba, Il.DataType.UnsignedByte, IntPtr.Zero);
            fixed (byte* buffer = data)
            {
                var result = Il.TexImageDxtc(width, height, 0, Il.DxtcDefinition.Dxt5, buffer);
                CheckError(result);
            }

            //var unmanagedPointer = Marshal.AllocHGlobal(data.Length);
            //Marshal.Copy(data, 0, unmanagedPointer, data.Length);
            //Marshal.FreeHGlobal(unmanagedPointer);

            //Il.SetPixels(0, 0, 0, width, height, 1, Il.DataFormat.Rgba, Il.DataType.UnsignedByte, unmanagedPointer);
        }

        public byte[] ConvertToVtf()
        {
            if (!_hasImage)
            {
                throw new InvalidOperationException("An image is has not been loaded. Load one before calling this method.");
            }

            var result = Il.ImageToDxtcData(Il.DxtcDefinition.Dxt5);
            CheckError(result);

            // A NULL ptr + size of 0 means "tell me how big to make the buffer".
            var fileSize = Il.SaveL(Il.ImageType.Vtf, IntPtr.Zero, 0);
            CheckError(fileSize);

            var unmanagedPointer = Marshal.AllocHGlobal((int) fileSize);
            fileSize = Il.SaveL(Il.ImageType.Vtf, unmanagedPointer, fileSize);
            CheckError(fileSize);

            var bytes = new byte[fileSize];
            Marshal.Copy(unmanagedPointer, bytes, 0, bytes.Length);

            Marshal.FreeHGlobal(unmanagedPointer);

            return bytes;
        }

        private void CheckError(bool result)
        {
            if (!result) Fail();
        }

        private void CheckError(uint result)
        {
            if (result == 0) Fail();
        }

        private void Fail()
        {
            throw new Exception("Error in Il.SaveL(): " + Il.GetError());
        }


        #region Disposing pattern

        private bool _disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // No managed state to dispose.
                }

                if (_hasImage)
                {
                    Il.DeleteImage(_image);
                    _image = 0;
                    _hasImage = false;
                }

                _disposedValue = true;
            }
        }

        ~DevIL()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
