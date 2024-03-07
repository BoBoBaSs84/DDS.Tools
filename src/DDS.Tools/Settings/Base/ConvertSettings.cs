using System.ComponentModel;

using DDS.Tools.Enumerators;

using Spectre.Console.Cli;

namespace DDS.Tools.Settings.Base;

/// <summary>
/// The convert settings class
/// </summary>
internal abstract class ConvertSettings : CommandSettings
{
	/// <summary>
	/// The source folder of the images.
	/// </summary>
	[Description(@"The source folder of the images.")]
	[CommandArgument(0, "<SourceFolder>")]
	public string SourceFolder { get; set; } = string.Empty;

	/// <summary>
	/// The target folder of the images.
	/// </summary>
	[Description(@"The target folder of the images.")]
	[CommandArgument(0, "<TargetFolder>")]
	public string TargetFolder { get; set; } = string.Empty;

	/// <summary>
	/// The convert mode to use.
	/// </summary>
	[Description($"The convert mode to use.\n" +
		$"Default is {nameof(ConvertModeType.Automatic)}, options are ignored.")]
	[CommandArgument(2, "[ConvertMode]")]
	public ConvertModeType ConvertMode { get; set; } = ConvertModeType.Automatic;

	/// <summary>
	/// Should folder and file names be retained?
	/// </summary>
	[Description("Should folder and file names be retained?")]
	[CommandOption("-r|--retain")]
	public bool RetainStructure { get; set; }

	/// <summary>
	/// Should texture maps be tried to be separated?
	/// </summary>
	[Description("Should texture maps be tried to be separated?")]
	[CommandOption("-s|--separate")]
	public bool SeparateMaps { get; set; }

	/// <summary>
	/// Should texture maps be tried to be ignored?
	/// </summary>
	[Description("Should texture maps be tried to be ignored?")]
	[CommandOption("-i|--ignore")]
	public bool IgnoreMaps { get; set; }

	/// <summary>
	/// Should the textures be separated by size?
	/// </summary>
	[Description("Should the textures be separated by size?")]
	[CommandOption("-b|--bysize")]
	public bool SeparateBySize { get; set; }
}
