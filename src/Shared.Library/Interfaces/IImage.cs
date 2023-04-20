namespace Shared.Library.Interfaces;

/// <summary>
/// The image interface.
/// </summary>
public interface IImage
{
	/// <summary>
	/// The file name of the image.
	/// </summary>
	string FileName { get; }

	/// <summary>
	/// The file path of the image.
	/// </summary>
	string FilePath { get; }

	/// <summary>
	/// Does the image have an alpha channel?
	/// </summary>
	bool HasAlphaChannel { get; }

	/// <summary>
	/// The heigth of the image.
	/// </summary>
	int Heigth { get; }

	/// <summary>
	/// The image binary data.
	/// </summary>
	byte[] ImageData { get; }

	/// <summary>
	/// The MD5 hash of the image.
	/// </summary>
	string Md5Hash { get; }

	/// <summary>
	/// The width of the image.
	/// </summary>
	int Width { get; }

	/// <summary>
	/// Should save the image using default encoder.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	void Save(string filePath);

	/// <summary>
	/// Should save the image using default encoder.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	/// <param name="compressionLevel">The compression level for the image.</param>
	void Save(string filePath, int compressionLevel);
}
