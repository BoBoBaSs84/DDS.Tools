// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using BCnEncoder.Encoder;

using DDS.Tools.Enumerators;
using DDS.Tools.Settings;

namespace DDS.ToolsTests.Settings;

[TestClass]
public sealed class PngConvertSettingsTests : UnitTestBase
{
	[TestMethod]
	public void PngConvertSettingsTest()
	{
		PngConvertSettings settings = new()
		{
			SourceFolder = TestConstants.ResourcePath,
			TargetFolder = TestConstants.ResourcePath,
			Compression = CompressionQuality.Fast,
			ConvertMode = ConvertModeType.Manual,
			RetainStructure = false,
			SeparateBySize = true,
		};

		Assert.AreEqual(TestConstants.ResourcePath, settings.SourceFolder);
		Assert.AreEqual(TestConstants.ResourcePath, settings.TargetFolder);
		Assert.AreEqual(CompressionQuality.Fast, settings.Compression);
		Assert.AreEqual(ConvertModeType.Manual, settings.ConvertMode);
		Assert.IsFalse(settings.RetainStructure);
		Assert.IsTrue(settings.SeparateBySize);
	}
}
