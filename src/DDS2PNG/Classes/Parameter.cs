namespace DDS2PNG.Classes;

internal sealed class Parameter
{
	public Parameter(string sourceFolder, int compressionLevel, bool divideNormalMaps, bool separateBySize)
	{
		SourceFolder = sourceFolder;
		CompressionLevel = compressionLevel;
		SeparateMaps = divideNormalMaps;
		SeparateBySize = separateBySize;
	}

	public string SourceFolder { get; }
	public int CompressionLevel { get; }
	public bool SeparateMaps { get; }
	public bool SeparateBySize { get; }
}
