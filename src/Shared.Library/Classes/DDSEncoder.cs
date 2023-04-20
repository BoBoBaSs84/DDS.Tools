using BCnEncoder.Encoder;
using BCnEncoder.Shared;

namespace Shared.Library.Classes;

/// <summary>
/// The internal <see cref="DDSEncoder"/> wrapper class.
/// </summary>
/// <remarks>
/// Derives from the <see cref="BcEncoder"/> class.
/// </remarks>
internal sealed class DDSEncoder : BcEncoder
{
	/// <summary>
	/// Initializes an instance of <see cref="DDSEncoder"/> class.
	/// </summary>
	/// <param name="generateMipMaps"></param>
	/// <param name="compressionQuality"></param>
	public DDSEncoder(bool generateMipMaps = true, CompressionQuality compressionQuality = CompressionQuality.Balanced)
	{
		OutputOptions.GenerateMipMaps = generateMipMaps;
		OutputOptions.Quality = compressionQuality;
		OutputOptions.Format = CompressionFormat.Bc3;
		OutputOptions.FileFormat = OutputFileFormat.Dds;
	}
}
