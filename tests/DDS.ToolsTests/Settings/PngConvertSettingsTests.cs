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
			IgnoreMaps = true,
			SeparateBySize = true,
			SeparateMaps = true
		};

		Assert.AreEqual(TestConstants.ResourcePath, settings.SourceFolder);
		Assert.AreEqual(TestConstants.ResourcePath, settings.TargetFolder);
		Assert.AreEqual(CompressionQuality.Fast, settings.Compression);
		Assert.AreEqual(ConvertModeType.Manual, settings.ConvertMode);
		Assert.IsFalse(settings.RetainStructure);
		Assert.IsTrue(settings.IgnoreMaps);
		Assert.IsTrue(settings.SeparateBySize);
		Assert.IsTrue(settings.SeparateMaps);
	}
}
