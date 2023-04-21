namespace DDS2PNG.Classes;

/// <summary>
/// The parameter class.
/// </summary>
internal sealed class Parameter
{
	/// <summary>
	/// Initializes an instance of <see cref="Parameter"/> class.
	/// </summary>
	/// <param name="sourceFolder">The source folder of the images.</param>
	/// <param name="searchPattern">The search pattern for the images.</param>
	/// <param name="compressionLevel">The compression level in which the images should be saved.</param>
	/// <param name="divideNormalMaps">Should texture maps be tried to be separated?</param>
	/// <param name="ignoreMaps">Should texture maps be tried to be ignored?</param>
	/// <param name="separateBySize">Should the textures be separated by size?</param>
	public Parameter(string sourceFolder, string searchPattern, int compressionLevel, bool divideNormalMaps, bool ignoreMaps, bool separateBySize)
	{
		SourceFolder = sourceFolder;
		SearchPattern = searchPattern;
		CompressionLevel = compressionLevel;
		SeparateMaps = divideNormalMaps;
		IgnoreMaps = ignoreMaps;
		SeparateBySize = separateBySize;
	}

	/// <summary>
	/// The source folder of the images.
	/// </summary>
	public string SourceFolder { get; }
	/// <summary>
	/// The search pattern for the images.
	/// </summary>
	public string SearchPattern { get; }
	/// <summary>
	/// The compression level in which the images should be saved.
	/// </summary>
	public int CompressionLevel { get; }
	/// <summary>
	/// Should texture maps be tried to be separated?
	/// </summary>
	public bool SeparateMaps { get; }
	/// <summary>
	/// Should texture maps be tried to be ignored?
	/// </summary>
	public bool IgnoreMaps { get; }
	/// <summary>
	/// Should the textures be separated by size?
	/// </summary>
	public bool SeparateBySize { get; }
}
