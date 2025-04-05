// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

using DDS.Tools.Commands;

using Spectre.Console.Cli;

namespace DDS.Tools.Extensions;

/// <summary>
/// The configurator extensions class.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Not relevant here.")]
[SuppressMessage("Style", "IDE0058", Justification = "Not relevant here.")]
internal static class ConfiguratorExtensions
{
	internal static IConfigurator ConfigureCommands(this IConfigurator configurator)
	{
		configurator.AddCommand<DdsConvertCommand>("dds")
			.WithDescription("Converts dds files into png files.")
			.WithExample(["dds", @"""D:\DDS-Textures""", @"""D:\PNG-Textures"""]);

		configurator.AddCommand<PngConvertCommand>("png")
			.WithDescription("Converts png files into dds files.")
			.WithExample(["png", @"""D:\PNG-Textures""", @"""D:\DDS-Textures"""]);

		return configurator;
	}
}
