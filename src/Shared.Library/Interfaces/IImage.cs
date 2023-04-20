namespace Shared.Library.Interfaces;

/// <summary>
/// The image interface.
/// </summary>
public interface IImage
{
	/// <summary>
	/// The image binary data.
	/// </summary>
	byte[] ImageData { get; }

	/// <summary>
	/// Should save the image using default encoder.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	void Save(string filePath);
}
