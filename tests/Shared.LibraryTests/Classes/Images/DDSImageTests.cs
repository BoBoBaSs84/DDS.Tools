using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Library.Factories;
using Shared.Library.Interfaces;
using TC = Shared.LibraryTests.TestConstants;

namespace Shared.LibraryTests.Classes.Images;

[TestClass]
public class DDSImageTests
{
	[TestMethod]
	[DeploymentItem(TC.TestDdsItem, TC.TestResourcesFolder)]
	public void SaveTest()
	{
		string sourceFolder = $"{TC.TestResourcesFolder}\\{TC.TestDdsItem}";
		string targetFolder = $"{TC.TestResourcesFolder}\\{TC.TestPngSaveItem}";

		IImage image = ImageFactory.CreateDdsImage(sourceFolder);
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
		string sourceFolder = $"{TC.TestResourcesFolder}\\{TC.TestDdsItem}";
		string targetFolder = $"{TC.TestResourcesFolder}\\{TC.TestPngSaveItem}";

		IImage image = ImageFactory.CreateDdsImage(sourceFolder);
		image.Save(targetFolder, compressionLevel);
	}

	[TestMethod]
	[DeploymentItem(TC.TestDdsItem, TC.TestResourcesFolder)]
	public void SaveWithCompressionTest()
	{
		int compressionLevel = 9;
		string sourceFolder = $"{TC.TestResourcesFolder}\\{TC.TestDdsItem}";
		string targetFolder = $"{TC.TestResourcesFolder}\\{TC.TestPngSaveItem}";

		IImage image = ImageFactory.CreateDdsImage(sourceFolder);
		image.Save(targetFolder, compressionLevel);
		FileInfo fileInfo = new(targetFolder);

		Assert.IsTrue(image.ImageData.Length > 0);
		Assert.IsTrue(fileInfo.Exists);
	}
}