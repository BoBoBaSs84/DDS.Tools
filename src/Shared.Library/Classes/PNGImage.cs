using BCnEncoder.ImageSharp;

namespace Shared.Library.Classes;

/// <summary>
/// The png image class.
/// </summary>
public sealed class PNGImage
{
	private readonly Image<Rgba32> _image;
	private readonly DDSEncoder _encoder;

	/// <summary>
	/// Initializes an instance of <see cref="PNGImage"/> class.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	public PNGImage(string filePath)
	{
		_image = Image.Load<Rgba32>(filePath);
		_encoder = new DDSEncoder();
	}

	/// <summary>
	/// Should save the image using default encoder.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	public void Save(string filePath)
	{
		using FileStream fileStream = File.OpenWrite(filePath);
		_encoder.EncodeToStream(_image, fileStream);
	}

	/// <summary>
	/// Should save the image using the provided encoder.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	/// <param name="encoder">The encoder to encode the image.</param>
	public void Save(string filePath, DDSEncoder encoder)
	{
		using FileStream fileStream = File.OpenWrite(filePath);
		encoder.EncodeToStream(_image, fileStream);
	}
}
