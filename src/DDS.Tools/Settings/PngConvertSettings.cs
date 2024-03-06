using System.ComponentModel;

using BCnEncoder.Encoder;

using DDS.Tools.Settings.Base;

using Spectre.Console.Cli;

namespace DDS.Tools.Settings;

/// <summary>
/// The png convert settings class.
/// </summary>
internal sealed class PngConvertSettings : ConvertSettings
{
	/// <summary>
	/// The compression level in which the images should be saved.
	/// </summary>
	[Description("The compression level in which the images should be saved.")]
	[CommandOption("-c|--compression")]
	public CompressionQuality Compression { get; set; } = CompressionQuality.Balanced;
}
