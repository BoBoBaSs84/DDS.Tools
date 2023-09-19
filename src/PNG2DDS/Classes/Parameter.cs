namespace PNG2DDS.Classes;

internal sealed class Parameter
{
	public Parameter(string sourceFolder, int compressionLevel)
	{
		SourceFolder = sourceFolder;
		CompressionLevel = compressionLevel;
	}

	public string SourceFolder { get; }
	public int CompressionLevel { get; }
}
