namespace DDS.Tools.Interfaces.Models;

/// <summary>
/// The image file interface.
/// </summary>
public interface IImageModel
{
	/// <summary>
	/// The file name of the image.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// The file path of the image.
	/// </summary>
	string Path { get; }

	/// <summary>
	/// The heigth of the image.
	/// </summary>
	int Heigth { get; }

	/// <summary>
	/// The width of the image.
	/// </summary>
	int Width { get; }

	/// <summary>
	/// The image binary data.
	/// </summary>
	byte[] Data { get; }

	/// <summary>
	/// The MD5 hash of the image.
	/// </summary>
	string Hash { get; }

	/// <summary>
	/// Loads the image from the provided designation.
	/// </summary>
	/// <param name="filePath">The designation to load the image file from.</param>
	abstract void Load(string filePath);

	/// <summary>
	/// Saves the image to the provided designation.
	/// </summary>
	/// <param name="filePath">The designation to save the image file to.</param>
	abstract void Save(string filePath);
}
