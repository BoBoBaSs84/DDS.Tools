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
	public void ConvertSettingsBindSourceTargetAndTargetFormat()
	{
		ProbeCommand.LastSettings = null;
		CommandApp app = CreateApp();

		int result = app.Run(["convert-probe", "source-folder", "target-folder", "--to", "PNG"]);

		Assert.AreEqual(0, result);
		Assert.IsNotNull(ProbeCommand.LastSettings);
		Assert.AreEqual("source-folder", ProbeCommand.LastSettings.SourceFolder);
		Assert.AreEqual("target-folder", ProbeCommand.LastSettings.TargetFolder);
		Assert.AreEqual(ImageType.PNG, ProbeCommand.LastSettings.To);
		Assert.IsNull(ProbeCommand.LastSettings.From);
		Assert.AreEqual(ConvertModeType.Automatic, ProbeCommand.LastSettings.ConvertMode);
	}

	[TestMethod]
	public void ConvertSettingsBindOptionalConvertModeAndSourceFormat()
	{
		ProbeCommand.LastSettings = null;
		CommandApp app = CreateApp();

		int result = app.Run(["convert-probe", "src", "tgt", nameof(ConvertModeType.Manual), "--from", "PNG", "--to", "DDS"]);

		Assert.AreEqual(0, result);
		Assert.IsNotNull(ProbeCommand.LastSettings);
		Assert.AreEqual(ConvertModeType.Manual, ProbeCommand.LastSettings.ConvertMode);
		Assert.AreEqual(ImageType.PNG, ProbeCommand.LastSettings.From);
		Assert.AreEqual(ImageType.DDS, ProbeCommand.LastSettings.To);
	}

	[TestMethod]
	public void ConvertSettingsRejectMissingTargetFormat()
	{
		CommandApp app = CreateApp();

		int result = app.Run(["convert-probe", "src", "tgt"]);

		Assert.AreNotEqual(0, result);
	}

	[TestMethod]
	public void ConvertSettingsBindRestoreFlagAndAllowMissingTargetFormat()
	{
		ProbeCommand.LastSettings = null;
		CommandApp app = CreateApp();

		int result = app.Run(["convert-probe", "src", "tgt", "--restore"]);

		Assert.AreEqual(0, result);
		Assert.IsNotNull(ProbeCommand.LastSettings);
		Assert.IsTrue(ProbeCommand.LastSettings.Restore);
	}

	[TestMethod]
	public void ConvertSettingsRejectRestoreCombinedWithFrom()
	{
		CommandApp app = CreateApp();

		int result = app.Run(["convert-probe", "src", "tgt", "--restore", "--from", "PNG"]);

		Assert.AreNotEqual(0, result);
	}

	[TestMethod]
	public void ConvertSettingsRejectEqualSourceAndTargetFormat()
	{
		CommandApp app = CreateApp();

		int result = app.Run(["convert-probe", "src", "tgt", "--from", "PNG", "--to", "PNG"]);

		Assert.AreNotEqual(0, result);
	}

	private static CommandApp CreateApp()
	{
		CommandApp app = new();
		app.Configure(config => config.AddCommand<ProbeCommand>("convert-probe"));
		return app;
	}

	private sealed class ProbeCommand : Command<ConvertSettings>
	{
		public static ConvertSettings? LastSettings { get; set; }

		protected override int Execute(CommandContext context, ConvertSettings settings, CancellationToken cancellationToken)
		{
			LastSettings = settings;
			return 0;
		}
	}
}
