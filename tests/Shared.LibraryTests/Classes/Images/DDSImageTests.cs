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
}