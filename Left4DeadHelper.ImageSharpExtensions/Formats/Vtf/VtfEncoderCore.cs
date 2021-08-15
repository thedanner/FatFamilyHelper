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
    internal class VtfEncoderCore
	{
		public async Task EncodeAsync(Image<Rgba32> image, Stream stream, CancellationToken cancellationToken)
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

        public void Encode(Image<Rgba32> image, Stream stream, CancellationToken cancellationToken)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

			for (int y = 0; y < image.Height; y++)
			{
				Span<Rgba32> pixelRowSpan = image.GetPixelRowSpan(y);
				for (int x = 0; x < image.Width; x++)
				{
					pixelRowSpan[x].A = pixelRowSpan[x].A >= 32 ? byte.MaxValue : byte.MinValue;
				}

				if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();
			}

			int floatScratch;

			// Now at address 0x00000000

			// Write the file signature.
			stream.Write(VtfConstants.HeaderBytes);

			byte[] dataBuffer = new byte[sizeof(long)];

			// Write the file version - currently using 7.2 specs.
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, 7);
			stream.Write(dataBuffer, 0, sizeof(uint));
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, 1);
			stream.Write(dataBuffer, 0, sizeof(uint));

			// Write the header size.
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, VtfConstants.HeaderSize);
            stream.Write(dataBuffer, 0, sizeof(uint));

			// Now at address 0x00000010

			// Now we write the width and height of the image.
			BinaryPrimitives.WriteUInt16LittleEndian(dataBuffer, (ushort)image.Width);
			stream.Write(dataBuffer, 0, sizeof(ushort));

			BinaryPrimitives.WriteUInt16LittleEndian(dataBuffer, (ushort)image.Height);
			stream.Write(dataBuffer, 0, sizeof(ushort));

			// Flags
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, VtfConstants.FlagsDxt1WithAlpha);
			stream.Write(dataBuffer, 0, sizeof(uint));

			// Number of frames in the animation.
			BinaryPrimitives.WriteUInt16LittleEndian(dataBuffer, 1);
			stream.Write(dataBuffer, 0, sizeof(ushort));
			// First frame in the animation
			BinaryPrimitives.WriteUInt16LittleEndian(dataBuffer, 0);
			stream.Write(dataBuffer, 0, sizeof(ushort));
			// Padding
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, 0);
			stream.Write(dataBuffer, 0, sizeof(uint));

			// Now at address 0x00000020 (reflectivity: 3 4-byte floats, plus 4 bytes padding

			// Reflectivity (3 floats)
			for (var i = 0; i < 3; i++)
			{
				floatScratch = BitConverter.SingleToInt32Bits(0.0f);
				BinaryPrimitives.WriteInt32LittleEndian(dataBuffer, floatScratch);
				stream.Write(dataBuffer, 0, sizeof(float));
			}

			// Padding
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, 0);
			stream.Write(dataBuffer, 0, sizeof(uint));

			// Now at address 0x00000030

			// Bumpmap scale
			floatScratch = BitConverter.SingleToInt32Bits(0.0f);
			BinaryPrimitives.WriteInt32LittleEndian(dataBuffer, floatScratch);
			stream.Write(dataBuffer, 0, sizeof(float));
			// High resolution image format
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, VtfConstants.FormatDxt1);
			stream.Write(dataBuffer, 0, sizeof(uint));
			// Mipmap count
			dataBuffer[0] = 1;
			stream.Write(dataBuffer, 0, sizeof(byte));
			// Low resolution image format; always DXT1
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, VtfConstants.FormatDxt1);
			stream.Write(dataBuffer, 0, sizeof(uint));
			// Low resolution image width, height.
			dataBuffer[0] = 0;
			stream.Write(dataBuffer, 0, sizeof(byte));
			dataBuffer[0] = 0;
			stream.Write(dataBuffer, 0, sizeof(byte));
			// Depth of the low resolution image? Shouldn't be needed with 7.1 but it's not reading correctly without it.
			dataBuffer[0] = 1;
			stream.Write(dataBuffer, 0, sizeof(byte));


			var neededBytes = CalculateSizeInBytes(image.Width, image.Height);

            if (stream is MemoryStream memStream)
            {
				memStream.Capacity = (int) VtfConstants.HeaderSize + neededBytes;
            }

			// DXT compress the data.
			var dxtEncoder = new BcEncoder();

			dxtEncoder.OutputOptions.MaxMipMapLevel = 1;
			dxtEncoder.OutputOptions.GenerateMipMaps = false;
			dxtEncoder.OutputOptions.Quality = CompressionQuality.BestQuality;
			dxtEncoder.OutputOptions.Format = CompressionFormat.Bc1WithAlpha; // S3TC DXT1
			dxtEncoder.OutputOptions.DdsBc1WriteAlphaFlag = true;

			if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

			var byteEncoding = dxtEncoder.EncodeToRawBytes(image);

			if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

			foreach (var mipBytes in byteEncoding)
			{
				stream.Write(mipBytes, 0, mipBytes.Length);
			}
		}

		private int CalculateSizeInBytes(int width, int height, int depth = 1)
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

			var blockCount = ((width + 3) / 4) * ((height + 3) / 4) * depth;
			
			// DXT1 is 8 bytes per block count.
			// DXT5 is 16 bytes per block count.
			var size = blockCount * 8;

			return size;
		}
    }
}
