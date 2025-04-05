// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.ComponentModel;

using DDS.Tools.Settings.Base;

using SixLabors.ImageSharp.Formats.Png;

using Spectre.Console.Cli;

namespace DDS.Tools.Settings;

/// <summary>
/// The dds convert settings class.
/// </summary>
internal sealed class DdsConvertSettings : ConvertSettingsBase
{
	/// <summary>
	/// The compression level in which the images should be saved.
	/// </summary>
	[Description("The compression level in which the images should be saved.")]
	[CommandOption("-c|--compression")]
	public PngCompressionLevel Compression { get; set; } = PngCompressionLevel.NoCompression;
}
