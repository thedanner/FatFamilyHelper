using BCnEncoder.Encoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Buffers.Binary;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.ImageSharpExtensions.Formats.Vtf
{
    internal class VtfEncoderCore : IDisposable
    {
        public async Task EncodeAsync<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
        {
            Configuration configuration = image.GetConfiguration();
            if (stream.CanSeek)
            {
                await DoEncodeAsync(stream).ConfigureAwait(false);
            }
            else
            {
                using var ms = new MemoryStream();
                await DoEncodeAsync(ms);
                ms.Position = 0;
                await ms.CopyToAsync(stream, configuration.StreamProcessingBufferSize, cancellationToken)
                    .ConfigureAwait(false);
            }

            Task DoEncodeAsync(Stream innerStream)
            {
                try
                {
                    Encode(image, innerStream, cancellationToken);
                    return Task.CompletedTask;
                }
                catch (OperationCanceledException)
                {
                    return Task.FromCanceled(cancellationToken);
                }
                catch (Exception ex)
                {
                    return Task.FromException(ex);
                }
            }
        }

        public void Encode<TPixel>(Image<TPixel> image, Stream stream, CancellationToken cancellationToken) where TPixel : unmanaged, IPixel<TPixel>
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

			//_vtfEncoderOptions.DxtFormat

			int floatScratch;

			// Write the file signature.
            stream.Write(VtfConstants.HeaderBytes);

			byte[] dataBuffer = new byte[sizeof(long)];

			// Write the file version - currently using 7.2 specs.
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, 7);
			stream.Write(dataBuffer, 0, sizeof(uint));
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, 2);
			stream.Write(dataBuffer, 0, sizeof(uint));

			// Write the header size.
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, 80);
            stream.Write(dataBuffer, 0, sizeof(uint));

			// Now we write the width and height of the image.
			BinaryPrimitives.WriteUInt16LittleEndian(dataBuffer, (ushort)image.Width);
			stream.Write(dataBuffer, 0, sizeof(ushort));

			BinaryPrimitives.WriteUInt16LittleEndian(dataBuffer, (ushort)image.Height);
			stream.Write(dataBuffer, 0, sizeof(ushort));

			//@TODO: This is supposed to be the flags used.  What should we use here?  Let users specify?
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, 0);
			stream.Write(dataBuffer, 0, sizeof(uint));

			// Number of frames in the animation. - @TODO: Change to use animations.
			BinaryPrimitives.WriteUInt16LittleEndian(dataBuffer, 1);
			stream.Write(dataBuffer, 0, sizeof(ushort));
			// First frame in the animation
			BinaryPrimitives.WriteUInt16LittleEndian(dataBuffer, 0);
			stream.Write(dataBuffer, 0, sizeof(ushort));
			// Padding
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, 0);
			stream.Write(dataBuffer, 0, sizeof(uint));

			// Reflectivity (3 floats) - @TODO: Use what values?  User specified?
			for (var i = 0; i < 3; i++)
			{
				floatScratch = BitConverter.SingleToInt32Bits(0.0f);
				BinaryPrimitives.WriteInt32LittleEndian(dataBuffer, floatScratch);
				stream.Write(dataBuffer, 0, sizeof(float));
			}

			// Padding
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, 0);
			stream.Write(dataBuffer, 0, sizeof(uint));
			// Bumpmap scale
			floatScratch = BitConverter.SingleToInt32Bits(0.0f);
			BinaryPrimitives.WriteInt32LittleEndian(dataBuffer, floatScratch);
			stream.Write(dataBuffer, 0, sizeof(float));
			// High resolution image format
			// 15 = DXT5
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, 15);
			stream.Write(dataBuffer, 0, sizeof(uint));
			// Mipmap count - @TODO: Use mipmaps
			dataBuffer[0] = 1;
			stream.Write(dataBuffer, 0, sizeof(char));
			// Low resolution image format - @TODO: Create low resolution image.
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, 0xFFFFFFFF);
			stream.Write(dataBuffer, 0, sizeof(uint));
			// Low resolution image width and height
			dataBuffer[0] = 0;
			stream.Write(dataBuffer, 0, sizeof(char));
			dataBuffer[0] = 0;
			stream.Write(dataBuffer, 0, sizeof(char));
			// Depth of the image - @TODO: Support for volumetric images.
			BinaryPrimitives.WriteUInt16LittleEndian(dataBuffer, 1);
			stream.Write(dataBuffer, 0, sizeof(ushort));

			// Write final padding for the header (out to 80 bytes).
			for (var i = 0; i < 15; i++)
			{
				dataBuffer[0] = 0;
				stream.Write(dataBuffer, 0, sizeof(char));
			}

			// Do DXT compression here and write.
			// We have to find out how much we are writing first.
			var neededSize = CalculateSize(image.Width, image.Height);

            if (stream is MemoryStream memStream)
            {
				memStream.Capacity = 80 + neededSize;
            }

			// DXT compress the data.
			var dtxEncoder = new BcEncoder();

			dtxEncoder.OutputOptions.GenerateMipMaps = false;
			dtxEncoder.OutputOptions.Quality = CompressionQuality.BestQuality;
			dtxEncoder.OutputOptions.Format = CompressionFormat.Bc3; // BX3 = (S3TC DXT5)
			dtxEncoder.OutputOptions.FileFormat = OutputFileFormat.Dds; //Change to Dds for a dds file.


			var imageAsRgba32 = image as Image<Rgba32>;
			if (imageAsRgba32 == null)
			{
				imageAsRgba32 = image.CloneAs<Rgba32>();
			}

			dtxEncoder.EncodeToStream(imageAsRgba32, stream);
		}

        private int CalculateSize(int width, int height, int depth = 1)
		{
			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive.");
			}
			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive.");
			}
			if (depth <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(depth), "Depth must be positive.");
			}

			// DXT5 is 16 bytes per block count.
			var blockCount = ((width + 3) / 4) * ((height + 3) / 4) * depth;

			var size = blockCount * 16;

			return size;
		}

		#region Dispose pattern

		private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: set large fields to null
                _disposedValue = true;
            }
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
