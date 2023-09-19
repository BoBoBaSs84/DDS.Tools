using BCnEncoder.Encoder;
using BCnEncoder.ImageSharp;

using Shared.Library.Extensions;
using Shared.Library.Interfaces;

namespace Shared.Library.Classes.Images;

/// <summary>
/// The png image class.
/// </summary>
internal sealed class PNGImage : IImage
{
	private readonly DDSEncoder _encoder = new();
	private readonly Image<Rgba32> _image;

	public string FileName { get; }
	public string FilePath { get; }
	public int Heigth { get; }
	public byte[] ImageData { get; }
	public string Md5Hash { get; }
	public int Width { get; }

	/// <summary>
	/// Initializes an instance of <see cref="PNGImage"/> class.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	public PNGImage(string filePath)
	{
		FileInfo fileInfo = new(filePath);
		FileName = fileInfo.Name;
		FilePath = fileInfo.FullName;

		using FileStream fileStream = File.OpenRead(filePath);
		_image = Image.Load<Rgba32>(fileStream);
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
		_encoder.EncodeToStream(_image, fileStream);
	}

	/// <inheritdoc/>
	public void Save(string filePath, int compressionLevel)
	{
		if (!Enum.IsDefined(typeof(CompressionQuality), compressionLevel))
		{
			string errorMessage = "Possible compression level values are:\n";

			foreach (CompressionQuality quality in CompressionQuality.Fast.GetListFromEnum().Distinct())
				errorMessage += $"\t{(int)quality} - {quality}\n";

			throw new ArgumentOutOfRangeException(nameof(compressionLevel), errorMessage);
		}

		_encoder.OutputOptions.Quality = (CompressionQuality)compressionLevel;
		_encoder.OutputOptions.DdsPreferDxt10Header = true;
		_encoder.OutputOptions.MaxMipMapLevel = -1;
		_encoder.OutputOptions.Format = BCnEncoder.Shared.CompressionFormat.Bc3;

		Save(filePath);
	}
}
