using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.Library.Factories;
using Shared.Library.Interfaces;
using TC = Shared.LibraryTests.TestConstants;

namespace Shared.LibraryTests.Classes.Images;

[TestClass]
public class PNGImageTests
{
	public PNGImageTests()
	{
		Directory.CreateDirectory(TC.SourceFolder);
		Directory.CreateDirectory(TC.TargetFolder);
	}
	
	[TestMethod]
	[DeploymentItem(TC.TestPngItem, TC.SourceFolder)]
	public void SaveTest()
	{
		string sourceFolder = $"{TC.SourceFolder}\\{TC.TestPngItem}";
		string targetFolder = $"{TC.TargetFolder}\\{TC.TestDdsSaveItem}";

		IImage image = ImageFactory.CreatePngImage(sourceFolder);
		image.Save(targetFolder);
		FileInfo fileInfo = new(targetFolder);

		Assert.IsTrue(image.ImageData.Length > 0);
		Assert.IsTrue(fileInfo.Exists);
	}

	[TestMethod]
	[DeploymentItem(TC.TestPngItem, TC.SourceFolder)]
	[ExpectedException(typeof(ArgumentOutOfRangeException))]
	public void SaveWithCompressionExceptionTest()
	{
		int compressionLevel = 99;
		string sourceFolder = $"{TC.SourceFolder}\\{TC.TestPngItem}";
		string targetFolder = $"{TC.TargetFolder}\\{TC.TestDdsSaveItem}";

		IImage image = ImageFactory.CreatePngImage(sourceFolder);
		image.Save(targetFolder, compressionLevel);
	}

	[TestMethod]
	[DeploymentItem(TC.TestPngItem, TC.SourceFolder)]
	public void SaveWithCompressionTest()
	{
		int compressionLevel = 0;
		string sourceFolder = $"{TC.SourceFolder}\\{TC.TestPngItem}";
		string targetFolder = $"{TC.TargetFolder}\\{compressionLevel}_{TC.TestDdsSaveItem}";

		IImage image = ImageFactory.CreatePngImage(sourceFolder);
		image.Save(targetFolder, compressionLevel);
		FileInfo fileInfo = new(targetFolder);

		Assert.IsTrue(image.ImageData.Length > 0);
		Assert.IsTrue(fileInfo.Exists);
	}
}