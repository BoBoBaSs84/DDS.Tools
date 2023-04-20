using BCnEncoder.ImageSharp;
using Shared.Library.Interfaces;

namespace Shared.Library.Classes.Images;

/// <summary>
/// The png image class.
/// </summary>
public sealed class PNGImage : IImage
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
}
