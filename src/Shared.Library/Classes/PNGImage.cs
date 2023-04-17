namespace Shared.Library.Classes;

/// <summary>
/// The png image class.
/// </summary>
public sealed class PNGImage
{
	private readonly Image _image;

	/// <summary>
	/// Initializes an instance of <see cref="PNGImage"/> class.
	/// </summary>
	/// <param name="filePath">The path to the image file.</param>
	public PNGImage(string filePath)
			=> _image = Image.Load(filePath);
}
