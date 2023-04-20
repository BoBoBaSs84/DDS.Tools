using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Library.Factories;
using Shared.Library.Interfaces;
using TC = Shared.LibraryTests.TestConstants;

namespace Shared.LibraryTests.Classes.Images;

[TestClass]
public class PNGImageTests
{
	[TestMethod]
	[DeploymentItem(TC.TestPngItem, TC.TestResourcesFolder)]
	public void SaveTest()
	{
		string sourceFolder = $"{TC.TestResourcesFolder}\\{TC.TestPngItem}";
		string targetFolder = $"{TC.TestResourcesFolder}\\{TC.TestDdsSaveItem}";

		IImage image = ImageFactory.CreatePngImage(sourceFolder);
		image.Save(targetFolder);
		FileInfo fileInfo = new(targetFolder);

		Assert.IsTrue(image.ImageData.Length > 0);
		Assert.IsTrue(fileInfo.Exists);
	}

	[TestMethod]
	[DeploymentItem(TC.TestDdsItem, TC.TestResourcesFolder)]
	[ExpectedException(typeof(ArgumentOutOfRangeException))]
	public void SaveWithCompressionExceptionTest()
	{
		int compressionLevel = 99;
		string sourceFolder = $"{TC.TestResourcesFolder}\\{TC.TestPngItem}";
		string targetFolder = $"{TC.TestResourcesFolder}\\{TC.TestDdsSaveItem}";

		IImage image = ImageFactory.CreatePngImage(sourceFolder);
		image.Save(targetFolder, compressionLevel);
	}

	[TestMethod]
	[DeploymentItem(TC.TestPngItem, TC.TestResourcesFolder)]
	public void SaveWithCompressionTest()
	{
		int compressionLevel = 1;
		string sourceFolder = $"{TC.TestResourcesFolder}\\{TC.TestPngItem}";
		string targetFolder = $"{TC.TestResourcesFolder}\\{TC.TestDdsSaveItem}";

		IImage image = ImageFactory.CreatePngImage(sourceFolder);
		image.Save(targetFolder, compressionLevel);
		FileInfo fileInfo = new(targetFolder);

		Assert.IsTrue(image.ImageData.Length > 0);
		Assert.IsTrue(fileInfo.Exists);
	}
}