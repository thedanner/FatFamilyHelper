using BCnEncoder.Encoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Left4DeadHelper.ImageSharpExtensions.Formats.Vtf;

internal class VtfEncoderCore
	{
		public enum DxtImageFormat
		{
			Dxt1OneBitAlpha,
			Dxt5,
		}

		private readonly VtfImageType _imageType;
		
    public VtfEncoderCore(VtfImageType imageType)
    {
			_imageType = imageType;
    }

		public Task EncodeAsync(Image<Rgba32> image, Stream stream, CancellationToken cancellationToken)
    {
			return EncodeAsync(new List<Image<Rgba32>> { image }, stream, cancellationToken);
    }

		public async Task EncodeAsync(IList<Image<Rgba32>> images, Stream stream, CancellationToken cancellationToken)
		{
			if (images is null) throw new ArgumentNullException(nameof(images));
			if (images.Count == 0)
			{
				throw new ArgumentException("The collection of images must not be empty.", nameof(images));
			}
			if (stream is null) throw new ArgumentNullException(nameof(stream));

			Configuration configuration = images[0].GetConfiguration();
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
                Encode(images, innerStream, cancellationToken);
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
			Encode(new List<Image<Rgba32>> { image }, stream, cancellationToken);
    }

		public void Encode(IList<Image<Rgba32>> images, Stream stream, CancellationToken cancellationToken)
		{
			if (images is null) throw new ArgumentNullException(nameof(images));
			if (images.Count == 0) throw new ArgumentException("At least one image is required.", nameof(images));
			if (images.Count > byte.MaxValue)
			{
				throw new ArgumentException($"At least most {byte.MaxValue} images are permitted.", nameof(images));
			}
			if (stream is null) throw new ArgumentNullException(nameof(stream));

			var image = images[0];

			var imageFormat = GetImageFormatFromImageType(_imageType);
			FixAlphaIfNeeded(imageFormat, images, cancellationToken);

			if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

			EnsureStreamCapacityIfPossible(stream, imageFormat, images);

			if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();


			int floatScratch;

			// Now at address 0x00000000

			// Write the file signature.
			stream.Write(VtfConstants.HeaderBytes);

			byte[] dataBuffer = new byte[sizeof(long)];

			// Write the file version.
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, 7);
			stream.Write(dataBuffer, 0, sizeof(uint));
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, 1);
			stream.Write(dataBuffer, 0, sizeof(uint));

			// Write the header size.
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, VtfConstants.HeaderSize);
			stream.Write(dataBuffer, 0, sizeof(uint));

			if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();


			// Now at address 0x00000010

			// Now we write the width and height of the image.
			BinaryPrimitives.WriteUInt16LittleEndian(dataBuffer, (ushort)image.Width);
			stream.Write(dataBuffer, 0, sizeof(ushort));

			BinaryPrimitives.WriteUInt16LittleEndian(dataBuffer, (ushort)image.Height);
			stream.Write(dataBuffer, 0, sizeof(ushort));

			// Flags
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, GetVtfFlags(_imageType));
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

			if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();


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

			if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();


			// Now at address 0x00000030

			// Bumpmap scale
			floatScratch = BitConverter.SingleToInt32Bits(0.0f);
			BinaryPrimitives.WriteInt32LittleEndian(dataBuffer, floatScratch);
			stream.Write(dataBuffer, 0, sizeof(float));
			// High resolution image format
			BinaryPrimitives.WriteUInt32LittleEndian(dataBuffer, GetVtfHighResFormatValue(imageFormat));
			stream.Write(dataBuffer, 0, sizeof(uint));
			// Mipmap count
			dataBuffer[0] = (byte) images.Count;
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

			if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();


			// DXT compress the data.
			var dxtEncoder = new BcEncoder();

			dxtEncoder.OutputOptions.MaxMipMapLevel = 1;
			dxtEncoder.OutputOptions.GenerateMipMaps = false;
			dxtEncoder.OutputOptions.Quality = CompressionQuality.BestQuality;
			dxtEncoder.OutputOptions.Format = GetCompressionEncoderFormat(imageFormat);
			// Probably not needed since we're getting raw bytes and not writing a DDS file.
			dxtEncoder.OutputOptions.DdsBc1WriteAlphaFlag = imageFormat == DxtImageFormat.Dxt1OneBitAlpha;

			// 7.2 VTF layout:
			// VTF Header (done)
			// VTF Low Resolution Image Data (0x0 so not used)
			// For Each Mipmap(Smallest to Largest)
			//   For Each Frame(First to Last)
			//     For Each Face(First to Last)
			//       For Each Z Slice(Min to Max; Varies with Mipmap)
			//         VTF High Resolution Image Data
			// So for us, we just reverse these images since they come in at the highest res and get smaller.

			foreach (var currentImage in images.Reverse())
			{
				var byteEncoding = dxtEncoder.EncodeToRawBytes(currentImage);

				if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();

				foreach (var mipBytes in byteEncoding)
				{
					stream.Write(mipBytes, 0, mipBytes.Length);
				}
			}
		}

    private static void EnsureStreamCapacityIfPossible(Stream stream, DxtImageFormat imageFormat, ICollection<Image<Rgba32>> images)
    {
			var neededBytes = (int)VtfConstants.HeaderSize + CalculateSizeInBytes(imageFormat, images);
			if (stream is MemoryStream memStream)
			{
				memStream.Capacity = neededBytes;
			}
			else if (stream is FileStream fileStream)
			{
				fileStream.SetLength(neededBytes);
			}
		}

    private static DxtImageFormat GetImageFormatFromImageType(VtfImageType imageType)
    {
        switch (imageType)
        {
				case VtfImageType.Single1024:
					return DxtImageFormat.Dxt1OneBitAlpha;
				case VtfImageType.Single512:
				case VtfImageType.Fading:
					return DxtImageFormat.Dxt5;
				default:
					throw new NotImplementedException($"No case added for {nameof(VtfImageType)}.{imageType}.");
        }
    }

    private static uint GetVtfHighResFormatValue(DxtImageFormat imageFormat)
    {
        // If we use FormatDxt1OneBitAlpha, game engines will not interperate the image correctly.
        // Just use Dxt1 without the alpha flag.
        return imageFormat switch
        {
            DxtImageFormat.Dxt5 => VtfConstants.FormatDxt5,
            DxtImageFormat.Dxt1OneBitAlpha => VtfConstants.FormatDxt1,
            _ => throw new NotImplementedException($"No case added for {nameof(DxtImageFormat)}.{imageFormat}."),
        };
    }

    private static CompressionFormat GetCompressionEncoderFormat(DxtImageFormat imageFormat)
		{
        // If we use FormatDxt1OneBitAlpha, game engines will not interperate the image correctly.
        // Just use Dxt1 without the alpha flag.
        return imageFormat switch
        {
            DxtImageFormat.Dxt5 => CompressionFormat.Bc3,
            DxtImageFormat.Dxt1OneBitAlpha => CompressionFormat.Bc1WithAlpha,
            _ => throw new NotImplementedException($"No case added for {nameof(DxtImageFormat)}.{imageFormat}."),
        };
    }

    private static uint GetVtfFlags(VtfImageType imageType)
    {
			VtfConstants.VtfFlags flags = VtfConstants.VtfFlags.NoLevelOfDetail;

			switch (imageType)
			{
				case VtfImageType.Single1024:
				case VtfImageType.Single512:
					flags |= VtfConstants.VtfFlags.ClampS | VtfConstants.VtfFlags.ClampT | VtfConstants.VtfFlags.NoMipmaps;
					break;
				case VtfImageType.Fading:
					break;
				default:
					throw new NotImplementedException($"No case added for {nameof(VtfImageType)}.{imageType}.");
			}

			var imageFormat = GetImageFormatFromImageType(imageType);

        flags |= imageFormat switch
        {
            DxtImageFormat.Dxt5 => VtfConstants.VtfFlags.EightBitAlpha,
            DxtImageFormat.Dxt1OneBitAlpha => VtfConstants.VtfFlags.OneBitAlpha,
            _ => throw new NotImplementedException($"No case added for {nameof(DxtImageFormat)}.{imageFormat}."),
        };
        return (uint) flags;
		}

    private static void FixAlphaIfNeeded(DxtImageFormat imageFormat,
			ICollection<Image<Rgba32>> images, CancellationToken cancellationToken)
    {
			if (imageFormat == DxtImageFormat.Dxt1OneBitAlpha)
			{
				foreach (var image in images)
				{
					image.ProcessPixelRows(pixelAccessor =>
					{
						for (int y = 0; y < image.Height; y++)
						{
							Span<Rgba32> pixelRowSpan = pixelAccessor.GetRowSpan(y);
							for (int x = 0; x < image.Width; x++)
							{
								pixelRowSpan[x].A = pixelRowSpan[x].A >= 32 ? byte.MaxValue : byte.MinValue;
							}

							if (cancellationToken.IsCancellationRequested) throw new TaskCanceledException();
						}
					});
				}
			}
		}

		private static int CalculateSizeInBytes(DxtImageFormat imageFormat, Image<Rgba32> image)
    {
			return CalculateSizeInBytes(imageFormat, image.Width, image.Height, 1);
		}

		private static int CalculateSizeInBytes(DxtImageFormat imageFormat, ICollection<Image<Rgba32>> images)
    {
			var size = images.Aggregate(0,
				(acc, next) => acc + CalculateSizeInBytes(imageFormat, next.Width, next.Height, 1));
			return size;
		}


		private static int CalculateSizeInBytes(DxtImageFormat imageFormat, int width, int height, int depth = 1)
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
			var bytesPerBlock = imageFormat switch
        {
            DxtImageFormat.Dxt5 => 16,
            DxtImageFormat.Dxt1OneBitAlpha => 8,
            _ => throw new NotImplementedException($"No case added for {nameof(DxtImageFormat)}.{imageFormat}."),
        };
        var size = blockCount * bytesPerBlock;

			return size;
		}
}
