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
			SeparateMaps = true,
			IgnoreMaps = true,
		};

		Assert.AreEqual(TestConstants.ResourcePath, settings.SourceFolder);
		Assert.AreEqual(TestConstants.ResourcePath, settings.TargetFolder);
		Assert.AreEqual(PngCompressionLevel.BestCompression, settings.Compression);
		Assert.AreEqual(ConvertModeType.Manual, settings.ConvertMode);
		Assert.IsFalse(settings.RetainStructure);
		Assert.IsTrue(settings.SeparateBySize);
		Assert.IsTrue(settings.SeparateMaps);
		Assert.IsTrue(settings.IgnoreMaps);
	}
}
