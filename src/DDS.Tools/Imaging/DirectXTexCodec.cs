// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Interfaces.Imaging;

using Hexa.NET.DirectXTex;

using HexaGen.Runtime;

namespace DDS.Tools.Imaging;

/// <summary>
/// An <see cref="IImageDecoder"/> and <see cref="IImageEncoder"/> for a single
/// <see cref="ImageType"/>, backed by the native DirectXTex library.
/// </summary>
/// <remarks>
/// One instance handles exactly one format; the service collection registers one
/// per supported format. All native scratch images are released before returning,
/// so callers only ever see managed <see cref="ImageCanvas"/> / <see cref="byte"/>[] data.
/// </remarks>
/// <param name="format">The image file format this codec reads and writes.</param>
internal sealed class DirectXTexCodec(ImageType format) : IImageDecoder, IImageEncoder
{
	private const int DxgiR8G8B8A8Unorm = 28;
	private const int DxgiBc1Unorm = 71;
	private const int DxgiBc3Unorm = 77;
	private const float AlphaThreshold = 0.5f;

	/// <inheritdoc cref="IImageDecoder.Format"/>
	public ImageType Format { get; } = format;

	/// <inheritdoc/>
	public unsafe ImageCanvas Decode(byte[] data)
	{
		ScratchImage loaded = DirectXTex.CreateScratchImage();
		ScratchImage rgba = default;
		try
		{
			Load(data, ref loaded);

			TexMetadata metadata = loaded.GetMetadata();
			ScratchImage source = loaded;

			if (metadata.Format != DxgiR8G8B8A8Unorm)
			{
				rgba = DirectXTex.CreateScratchImage();
				Check(DirectXTex.IsCompressed(metadata.Format)
					? DirectXTex.Decompress(loaded.GetImage(0, 0, 0), DxgiR8G8B8A8Unorm, ref rgba)
					: DirectXTex.Convert(loaded.GetImage(0, 0, 0), DxgiR8G8B8A8Unorm, TexFilterFlags.Default, AlphaThreshold, ref rgba),
					"decode pixels to RGBA32");
				source = rgba;
			}

			return CopyOut(source);
		}
		finally
		{
			Release(loaded);
			Release(rgba);
		}
	}

	/// <inheritdoc/>
	public unsafe byte[] Encode(ImageCanvas canvas, CompressionLevel compression)
	{
		ScratchImage source = DirectXTex.CreateScratchImage();
		try
		{
			Fill(canvas, ref source);

			return Format switch
			{
				ImageType.DDS => EncodeDds(source, compression),
				ImageType.TGA => EncodeTga(source),
				ImageType.PNG => EncodeWic(source, WICCodecs.CodecPng),
				ImageType.JPG => EncodeWic(source, WICCodecs.CodecJpeg),
				_ => throw new CommandException($"Writing '{Format}' files is not supported.")
			};
		}
		finally
		{
			Release(source);
		}
	}

	private unsafe void Load(byte[] data, ref ScratchImage image)
	{
		fixed (byte* pointer = data)
		{
			nuint size = (nuint)data.Length;
			HResult result = Format switch
			{
				ImageType.DDS => DirectXTex.LoadFromDDSMemory(pointer, size, DDSFlags.None, null, ref image),
				ImageType.TGA => DirectXTex.LoadFromTGAMemory(pointer, size, TGAFlags.None, null, ref image),
				ImageType.PNG or ImageType.JPG => DirectXTex.LoadFromWICMemory(pointer, size, WICFlags.None, null, ref image, default),
				_ => throw new CommandException($"Reading '{Format}' files is not supported.")
			};

			Check(result, $"load the {Format} file");
		}
	}

	private static unsafe ImageCanvas CopyOut(ScratchImage image)
	{
		Image* source = image.GetImage(0, 0, 0);
		if (source is null)
			throw new ServiceException("DirectXTex returned no image data.");

		int width = (int)source->Width;
		int height = (int)source->Height;
		int rowPitch = (int)source->RowPitch;
		int stride = width * 4;

		byte[] rgba = new byte[height * stride];
		fixed (byte* destination = rgba)
		{
			for (int y = 0; y < height; y++)
				Buffer.MemoryCopy(source->Pixels + ((long)y * rowPitch), destination + ((long)y * stride), stride, stride);
		}

		return new ImageCanvas(width, height, rgba);
	}

	private static unsafe void Fill(ImageCanvas canvas, ref ScratchImage image)
	{
		Check(image.Initialize2D(DxgiR8G8B8A8Unorm, (nuint)canvas.Width, (nuint)canvas.Height, 1, 1, CPFlags.None), "allocate the pixel buffer");

		Image* destination = image.GetImage(0, 0, 0);
		int stride = canvas.Width * 4;
		int rowPitch = (int)destination->RowPitch;

		fixed (byte* source = canvas.Rgba)
		{
			for (int y = 0; y < canvas.Height; y++)
				Buffer.MemoryCopy(source + ((long)y * stride), destination->Pixels + ((long)y * rowPitch), rowPitch, stride);
		}
	}

	private static unsafe byte[] EncodeDds(ScratchImage source, CompressionLevel compression)
	{
		int targetFormat = DirectXTex.IsAlphaAllOpaque(source) ? DxgiBc1Unorm : DxgiBc3Unorm;

		ScratchImage mipChain = DirectXTex.CreateScratchImage();
		ScratchImage compressed = DirectXTex.CreateScratchImage();
		try
		{
			TexMetadata sourceMetadata = source.GetMetadata();
			Check(DirectXTex.GenerateMipMaps2(source.GetImages(), source.GetImageCount(), ref sourceMetadata, TexFilterFlags.Default, 0, ref mipChain), "generate mip maps");

			TexMetadata mipMetadata = mipChain.GetMetadata();
			Check(DirectXTex.Compress2(mipChain.GetImages(), mipChain.GetImageCount(), ref mipMetadata, targetFormat, ToCompressFlags(compression), AlphaThreshold, ref compressed), "compress the texture");

			TexMetadata compressedMetadata = compressed.GetMetadata();
			return SaveDds(compressed, compressedMetadata);
		}
		finally
		{
			Release(mipChain);
			Release(compressed);
		}
	}

	private static unsafe byte[] SaveDds(ScratchImage image, TexMetadata metadata)
	{
		Blob blob = DirectXTex.CreateBlob();
		try
		{
			Check(DirectXTex.SaveToDDSMemory2(image.GetImages(), image.GetImageCount(), ref metadata, DDSFlags.None, ref blob), "write the DDS file");
			return ToArray(blob);
		}
		finally
		{
			DirectXTex.BlobRelease(blob);
		}
	}

	private static unsafe byte[] EncodeTga(ScratchImage source)
	{
		Blob blob = DirectXTex.CreateBlob();
		try
		{
			Check(DirectXTex.SaveToTGAMemory(source.GetImage(0, 0, 0), TGAFlags.None, ref blob, null), "write the TGA file");
			return ToArray(blob);
		}
		finally
		{
			DirectXTex.BlobRelease(blob);
		}
	}

	private static unsafe byte[] EncodeWic(ScratchImage source, WICCodecs codec)
	{
		Blob blob = DirectXTex.CreateBlob();
		try
		{
			Check(DirectXTex.SaveToWICMemory(source.GetImage(0, 0, 0), WICFlags.None, DirectXTex.GetWICCodec(codec), ref blob, null, default), $"write the {codec} file");
			return ToArray(blob);
		}
		finally
		{
			DirectXTex.BlobRelease(blob);
		}
	}

	private static unsafe byte[] ToArray(Blob blob)
	{
		void* pointer = DirectXTex.BlobGetBufferPointer(blob);
		int size = (int)DirectXTex.BlobGetBufferSize(blob);

		byte[] result = new byte[size];
		fixed (byte* destination = result)
			Buffer.MemoryCopy(pointer, destination, size, size);

		return result;
	}

	private static TexCompressFlags ToCompressFlags(CompressionLevel compression)
		=> compression switch
		{
			CompressionLevel.None => TexCompressFlags.Default,
			CompressionLevel.Fast => TexCompressFlags.Parallel,
			CompressionLevel.Balanced => TexCompressFlags.Parallel | TexCompressFlags.Dither,
			CompressionLevel.Maximum => TexCompressFlags.Parallel | TexCompressFlags.Dither | TexCompressFlags.Uniform,
			_ => TexCompressFlags.Parallel | TexCompressFlags.Dither
		};

	private static void Release(ScratchImage image)
	{
		if (!image.IsNull)
			image.Release();
	}

	private static void Check(HResult result, string operation)
	{
		if (result.IsError)
			throw new ServiceException($"DirectXTex failed to {operation} (0x{result.Value:X8}).");
	}
}
