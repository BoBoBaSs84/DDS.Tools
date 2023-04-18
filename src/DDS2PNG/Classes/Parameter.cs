namespace DDS2PNG.Classes;

internal sealed class Parameter
{
	public Parameter(string sourceFolder, int compressionLevel, bool divideNormalMaps)
	{
		SourceFolder = sourceFolder;
		CompressionLevel = compressionLevel;
		SeparateMaps = divideNormalMaps;
	}

	public string SourceFolder { get; }
	public int CompressionLevel { get; }
	public bool SeparateMaps { get; }
}
