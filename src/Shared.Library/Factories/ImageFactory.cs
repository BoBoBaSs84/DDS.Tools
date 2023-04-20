using Shared.Library.Classes.Images;
using Shared.Library.Interfaces;

namespace Shared.Library.Factories;

/// <summary>
/// The image factory class.
/// </summary>
public static class ImageFactory
{
	/// <summary>
	/// Creates a new dds image instance.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	public static IImage CreateDdsImage(string filePath)
		=> new DDSImage(filePath);

	/// <summary>
	/// Creates a new png image instance.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	public static IImage CreatePngImage(string filePath)
		=> new PNGImage(filePath);
}
