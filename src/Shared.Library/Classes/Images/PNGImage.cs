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
	private readonly DDSEncoder _encoder;
	private readonly Image<Rgba32> _image;

	/// <summary>
	/// Initializes an instance of <see cref="PNGImage"/> class.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	public PNGImage(string filePath)
	{
		_encoder = new DDSEncoder();
		using FileStream fileStream = File.OpenRead(filePath);
		_image = Image.Load<Rgba32>(fileStream);
		fileStream.Position = 0;
		ImageData = Helper.StreamToByteArray(fileStream);
	}

	/// <inheritdoc/>
	public byte[] ImageData { get; }

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

		using FileStream fileStream = File.OpenWrite(filePath);
		_encoder.EncodeToStream(_image, fileStream);
	}
}
