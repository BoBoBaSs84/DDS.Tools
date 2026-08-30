// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.ComponentModel;

using DDS.Tools.Enumerators;

using Spectre.Console;
using Spectre.Console.Cli;

namespace DDS.Tools.Settings;

/// <summary>
/// The settings for the <c>convert</c> command.
/// </summary>
public sealed class ConvertSettings : CommandSettings
{
	/// <summary>
	/// The source folder of the images.
	/// </summary>
	[Description("The source folder of the images.")]
	[CommandArgument(0, "<SourceFolder>")]
	public string SourceFolder { get; set; } = string.Empty;

	/// <summary>
	/// The target folder of the images.
	/// </summary>
	[Description("The target folder of the images.")]
	[CommandArgument(1, "<TargetFolder>")]
	public string TargetFolder { get; set; } = string.Empty;

	/// <summary>
	/// The convert mode to use.
	/// </summary>
	[Description($"The convert mode to use.\n" +
		$"Default is {nameof(ConvertModeType.Automatic)}, options are ignored.")]
	[CommandArgument(2, "[ConvertMode]")]
	public ConvertModeType ConvertMode { get; set; } = ConvertModeType.Automatic;

	/// <summary>
	/// The image format to read from the source folder.
	/// When omitted it is inferred from the files found there.
	/// </summary>
	[Description("The image format to read. Inferred from the source folder when omitted.")]
	[CommandOption("-f|--from")]
	public ImageType? From { get; set; }

	/// <summary>
	/// The image format to write into the target folder.
	/// </summary>
	[Description("The image format to write.")]
	[CommandOption("-t|--to")]
	public ImageType To { get; set; }

	/// <summary>
	/// Should folder and file names be retained?
	/// </summary>
	[Description("Should folder and file names be retained?")]
	[CommandOption("-r|--retain")]
	public bool RetainStructure { get; set; }

	/// <summary>
	/// Should the textures be separated by size?
	/// </summary>
	[Description("Should the textures be separated by size?")]
	[CommandOption("-b|--bysize")]
	public bool SeparateBySize { get; set; }

	/// <summary>
	/// How hard the target encoder should work to shrink the output.
	/// </summary>
	[Description($"The compression effort for the written images.\n" +
		$"Default is {nameof(CompressionLevel.Balanced)}.")]
	[CommandOption("-c|--compression")]
	public CompressionLevel Compression { get; set; } = CompressionLevel.Balanced;

	/// <summary>
	/// Restore edited files back to the original format recorded in the source
	/// folder's <c>Result.json</c>.
	/// </summary>
	[Description("Restore files to the original format recorded in the source folder's Result.json.")]
	[CommandOption("-x|--restore")]
	public bool Restore { get; set; }

	/// <inheritdoc/>
	public override ValidationResult Validate()
	{
		if (!Restore && !Enum.IsDefined(To))
			return ValidationResult.Error("The target format '--to' is required.");

		if (Restore && From is not null)
			return ValidationResult.Error("The '--from' option cannot be combined with '--restore'.");

		if (From is not null && From.Value.Equals(To))
			return ValidationResult.Error("The source and target format must differ.");

		return ValidationResult.Success();
	}
}
