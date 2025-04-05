// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;
using DDS.Tools.Settings;

using SixLabors.ImageSharp.Formats.Png;

namespace DDS.ToolsTests.Settings;

[TestClass]
public sealed class DdsConvertSettingsTests : UnitTestBase
{
	[TestMethod()]
	public void DdsConvertSettingsTest()
	{
		DdsConvertSettings settings = new()
		{
			SourceFolder = TestConstants.ResourcePath,
			TargetFolder = TestConstants.ResourcePath,
			Compression = PngCompressionLevel.BestCompression,
			ConvertMode = ConvertModeType.Manual,
			RetainStructure = false,
			SeparateBySize = true,
		};

		Assert.AreEqual(TestConstants.ResourcePath, settings.SourceFolder);
		Assert.AreEqual(TestConstants.ResourcePath, settings.TargetFolder);
		Assert.AreEqual(PngCompressionLevel.BestCompression, settings.Compression);
		Assert.AreEqual(ConvertModeType.Manual, settings.ConvertMode);
		Assert.IsFalse(settings.RetainStructure);
		Assert.IsTrue(settings.SeparateBySize);
	}
}
