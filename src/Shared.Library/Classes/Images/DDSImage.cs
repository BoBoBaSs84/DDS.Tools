using BCnEncoder.ImageSharp;
using Shared.Library.Interfaces;
using SixLabors.ImageSharp.Formats.Png;

namespace Shared.Library.Classes.Images;

/// <summary>
/// The dds image class.
/// </summary>
public sealed class DDSImage : IImage
{
	private readonly DDSDecoder _decoder;
	private readonly Image<Rgba32> _image;

	/// <inheritdoc/>
	public byte[] ImageData { get; }

	/// <summary>
	/// Initializes an instance of <see cref="DDSImage"/> class.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	public DDSImage(string filePath)
	{
		_decoder = new DDSDecoder();
		using FileStream fileStream = File.OpenRead(filePath);
		_image = _decoder.DecodeToImageRgba32(fileStream);
		fileStream.Position = 0;
		ImageData = Helper.StreamToByteArray(fileStream);
	}

	/// <inheritdoc/>
	public void Save(string filePath)
	{
		using FileStream fileStream = File.OpenWrite(filePath);
		PngEncoder pngEncoder = new() { CompressionLevel = 0 };
		_image.SaveAsPng(fileStream, pngEncoder);
	}

	/// <summary>
	/// Should save the image using default encoder.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	/// <param name="level">The compression level for the image.</param>
	[Obsolete("I'm going to remove this method.")]
	public void Save(string filePath, int level = 0)
		=> Save(filePath);
}
