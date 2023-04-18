using Pfim;
using SixLabors.ImageSharp.Formats.Png;

namespace Shared.Library.Classes;

/// <summary>
/// The dds image class.
/// </summary>
public sealed class DDSImage
{
	private readonly IImage _image;

	/// <summary>
	/// The image binary data.
	/// </summary>
	public byte[] ImageData => _image.Data;

	/// <summary>
	/// The image width.
	/// </summary>
	public int Width => _image.Width;

	/// <summary>
	/// The image height.
	/// </summary>
	public int Height => _image.Height;

	/// <summary>
	/// Initializes an instance of <see cref="DDSImage"/> class.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	public DDSImage(string filePath)
	{
		_image = Pfimage.FromFile(filePath);
		Process(_image);
	}

	/// <summary>
	/// Initializes an instance of <see cref="DDSImage"/> class.
	/// </summary>
	/// <param name="fileStream">The image file stream.</param>
	public DDSImage(Stream fileStream)
	{
		_image = Pfimage.FromStream(fileStream);
		Process(_image);
	}

	/// <summary>
	/// Should 
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	/// <param name="compressionLevel">The compression level for the image.</param>
	public void Save(string filePath, int compressionLevel)
	{
		if (!Enum.IsDefined(typeof(PngCompressionLevel), compressionLevel))
		{
			string message = $"Range should be within 0 to 9." +
					$"0 for no compression\n" +
					$"1 for best speed\n" +
					$"9 for best compression";

			throw new ArgumentOutOfRangeException(nameof(compressionLevel), message);
		}

		PngCompressionLevel level = (PngCompressionLevel)compressionLevel;

		if (_image.Format.Equals(ImageFormat.Rgba32))
			Save<Bgra32>(filePath, level);

		if (_image.Format.Equals(ImageFormat.Rgb24))
			Save<Bgr24>(filePath, level);
	}

	private static void Process(IImage image)
	{
		if (image.Compressed)
			image.Decompress();
	}

	private void Save<TPixel>(string filePath, PngCompressionLevel compressionLevel) where TPixel : unmanaged, IPixel<TPixel>
	{
		Image<TPixel> image = Image.LoadPixelData<TPixel>(_image.Data, _image.Width, _image.Height);
		PngEncoder encoder = new() { CompressionLevel = compressionLevel };
		image.SaveAsPng(filePath, encoder);
	}
}
