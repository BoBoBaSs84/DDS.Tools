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
}