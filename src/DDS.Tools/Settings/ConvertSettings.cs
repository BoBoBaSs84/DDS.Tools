// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.ComponentModel;

using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;

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
	/// The resolved source format. Only valid once <see cref="From"/> has been set,
	/// either from the command line or by inference.
	/// </summary>
	/// <exception cref="CommandException">If the source format is still unknown.</exception>
	internal ImageType SourceFormat
		=> From ?? throw new CommandException("The source image format could not be determined.");

	/// <inheritdoc/>
	public override ValidationResult Validate()
	{
		if (!Enum.IsDefined(To))
			return ValidationResult.Error("The target format '--to' is required.");

		if (From is not null && From.Value.Equals(To))
			return ValidationResult.Error("The source and target format must differ.");

		return ValidationResult.Success();
	}
}
