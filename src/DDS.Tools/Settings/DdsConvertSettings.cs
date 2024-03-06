using System.ComponentModel;

using DDS.Tools.Settings.Base;

using SixLabors.ImageSharp.Formats.Png;

using Spectre.Console.Cli;

namespace DDS.Tools.Settings;

/// <summary>
/// The dds convert settings class.
/// </summary>
internal sealed class DdsConvertSettings : ConvertSettings
{
	/// <summary>
	/// The compression level in which the images should be saved.
	/// </summary>
	[Description("The compression level in which the images should be saved.")]
	[CommandOption("-c|--compression")]
	public PngCompressionLevel Compression { get; set; } = PngCompressionLevel.NoCompression;
}
