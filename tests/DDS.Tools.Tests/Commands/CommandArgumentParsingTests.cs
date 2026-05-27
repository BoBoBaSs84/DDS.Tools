// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;
using DDS.Tools.Settings;

using Spectre.Console.Cli;

namespace DDS.Tools.Tests.Commands;

[TestClass]
public sealed class CommandArgumentParsingTests
{
	[TestMethod]
	public void DdsSettingsParserBindsSourceAndTargetByPosition()
	{
		DdsParserProbeCommand.LastSettings = null;
		CommandApp app = new();

		app.Configure(config => config.AddCommand<DdsParserProbeCommand>("dds-probe"));

		int result = app.Run(["dds-probe", "source-folder", "target-folder"]);

		Assert.AreEqual(0, result);
		Assert.IsNotNull(DdsParserProbeCommand.LastSettings);
		Assert.AreEqual("source-folder", DdsParserProbeCommand.LastSettings.SourceFolder);
		Assert.AreEqual("target-folder", DdsParserProbeCommand.LastSettings.TargetFolder);
		Assert.AreEqual(ConvertModeType.Automatic, DdsParserProbeCommand.LastSettings.ConvertMode);
	}

	[TestMethod]
	public void DdsSettingsParserBindsOptionalConvertMode()
	{
		DdsParserProbeCommand.LastSettings = null;
		CommandApp app = new();

		app.Configure(config => config.AddCommand<DdsParserProbeCommand>("dds-probe"));

		int result = app.Run(["dds-probe", "source-folder", "target-folder", nameof(ConvertModeType.Manual)]);

		Assert.AreEqual(0, result);
		Assert.IsNotNull(DdsParserProbeCommand.LastSettings);
		Assert.AreEqual(ConvertModeType.Manual, DdsParserProbeCommand.LastSettings.ConvertMode);
	}

	[TestMethod]
	public void PngSettingsParserBindsSourceAndTargetByPosition()
	{
		PngParserProbeCommand.LastSettings = null;
		CommandApp app = new();

		app.Configure(config => config.AddCommand<PngParserProbeCommand>("png-probe"));

		int result = app.Run(["png-probe", "png-source", "dds-target"]);

		Assert.AreEqual(0, result);
		Assert.IsNotNull(PngParserProbeCommand.LastSettings);
		Assert.AreEqual("png-source", PngParserProbeCommand.LastSettings.SourceFolder);
		Assert.AreEqual("dds-target", PngParserProbeCommand.LastSettings.TargetFolder);
		Assert.AreEqual(ConvertModeType.Automatic, PngParserProbeCommand.LastSettings.ConvertMode);
	}

	[TestMethod]
	public void PngSettingsParserBindsOptionalConvertMode()
	{
		PngParserProbeCommand.LastSettings = null;
		CommandApp app = new();

		app.Configure(config => config.AddCommand<PngParserProbeCommand>("png-probe"));

		int result = app.Run(["png-probe", "png-source", "dds-target", nameof(ConvertModeType.Grouping)]);

		Assert.AreEqual(0, result);
		Assert.IsNotNull(PngParserProbeCommand.LastSettings);
		Assert.AreEqual(ConvertModeType.Grouping, PngParserProbeCommand.LastSettings.ConvertMode);
	}

	private sealed class DdsParserProbeCommand : Command<DdsConvertSettings>
	{
		public static DdsConvertSettings? LastSettings { get; set; }

		protected override int Execute(CommandContext context, DdsConvertSettings settings, CancellationToken cancellationToken)
		{
			LastSettings = settings;
			return 0;
		}
	}

	private sealed class PngParserProbeCommand : Command<PngConvertSettings>
	{
		public static PngConvertSettings? LastSettings { get; set; }

		protected override int Execute(CommandContext context, PngConvertSettings settings, CancellationToken cancellationToken)
		{
			LastSettings = settings;
			return 0;
		}
	}
}
