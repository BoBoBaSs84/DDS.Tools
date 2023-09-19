using BCnEncoder.ImageSharp;

using Shared.Library.Extensions;
using Shared.Library.Interfaces;

using SixLabors.ImageSharp.Formats.Png;

namespace Shared.Library.Classes.Images;

/// <summary>
/// The dds image class.
/// </summary>
internal sealed class DDSImage : IImage
{
	private readonly DDSDecoder _decoder = new();
	private readonly Image<Rgba32> _image;

	public string FileName { get; }
	public string FilePath { get; }
	public int Heigth { get; }
	public byte[] ImageData { get; }
	public string Md5Hash { get; }
	public int Width { get; }

	/// <summary>
	/// Initializes an instance of <see cref="DDSImage"/> class.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	public DDSImage(string filePath)
	{
		FileInfo fileInfo = new(filePath);
		FileName = fileInfo.Name;
		FilePath = fileInfo.FullName;

		using FileStream fileStream = File.OpenRead(filePath);
		_image = _decoder.DecodeToImageRgba32(fileStream);
		Width = _image.Width;
		Heigth = _image.Height;

		fileStream.Position = 0;
		ImageData = Helper.StreamToByteArray(fileStream);
		Md5Hash = Helper.GetMD5String(ImageData);
	}

	/// <inheritdoc/>
	public void Save(string filePath)
	{
		using FileStream fileStream = File.OpenWrite(filePath);
		PngEncoder pngEncoder = new() { CompressionLevel = 0 };
		_image.SaveAsPng(fileStream, pngEncoder);
	}

	/// <inheritdoc/>
	public void Save(string filePath, int compressionLevel)
	{
		if (!Enum.IsDefined(typeof(PngCompressionLevel), compressionLevel))
		{
			string errorMessage = "Possible compression level values are:\n";

			foreach (PngCompressionLevel level in PngCompressionLevel.DefaultCompression.GetListFromEnum().Distinct())
				errorMessage += $"\t{(int)level} - {level}\n";

			errorMessage += $"Current value: '{compressionLevel}'";

			throw new ArgumentOutOfRangeException(nameof(compressionLevel), errorMessage);
		}

		using FileStream fileStream = File.OpenWrite(filePath);
		PngEncoder pngEncoder = new() { CompressionLevel = (PngCompressionLevel)compressionLevel };
		_image.SaveAsPng(fileStream, pngEncoder);
	}
}
