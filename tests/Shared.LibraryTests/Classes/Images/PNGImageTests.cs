using Shared.Library.Factories;
using Shared.Library.Interfaces;

using TC = Shared.LibraryTests.TestConstants;

namespace Shared.LibraryTests.Classes.Images;

[TestClass]
[SuppressMessage("Style", "IDE0058", Justification = "UnitTest")]
public class PNGImageTests
{
	public PNGImageTests()
	{
		Directory.CreateDirectory(TC.SourceFolder);
		Directory.CreateDirectory(TC.TargetFolder);
	}

	[TestMethod]
	[DeploymentItem(TC.PngImage, TC.SourceFolder)]
	public void SaveTest()
	{
		string sourceFolder = $"{TC.SourceFolder}\\{TC.PngImage}";
		string targetFolder = $"{TC.TargetFolder}\\{TC.DdsImageSave}";

		IImage image = ImageFactory.CreatePngImage(sourceFolder);
		image.Save(targetFolder);
		FileInfo fileInfo = new(targetFolder);

		Assert.IsTrue(fileInfo.Exists);
		Assert.IsTrue(image.FileName != string.Empty);
		Assert.IsTrue(image.FilePath != string.Empty);
		Assert.IsTrue(image.Heigth != 0);
		Assert.IsTrue(image.ImageData.Length > 0);
		Assert.IsTrue(image.Md5Hash != string.Empty);
		Assert.IsTrue(image.Width != 0);
	}

	[TestMethod]
	[DeploymentItem(TC.PngImage, TC.SourceFolder)]
	[ExpectedException(typeof(ArgumentOutOfRangeException))]
	public void SaveWithCompressionExceptionTest()
	{
		int compressionLevel = 99;
		string sourceFolder = $"{TC.SourceFolder}\\{TC.PngImage}";
		string targetFolder = $"{TC.TargetFolder}\\{TC.DdsImageSave}";

		IImage image = ImageFactory.CreatePngImage(sourceFolder);
		image.Save(targetFolder, compressionLevel);
	}

	[TestMethod]
	[DeploymentItem(TC.PngImage, TC.SourceFolder)]
	public void SaveWithCompressionTest()
	{
		int compressionLevel = 0;
		string sourceFolder = $"{TC.SourceFolder}\\{TC.PngImage}";
		string targetFolder = $"{TC.TargetFolder}\\{compressionLevel}_{TC.DdsImageSave}";

		IImage image = ImageFactory.CreatePngImage(sourceFolder);
		image.Save(targetFolder, compressionLevel);
		FileInfo fileInfo = new(targetFolder);

		Assert.IsTrue(fileInfo.Exists);
		Assert.IsTrue(image.FileName != string.Empty);
		Assert.IsTrue(image.FilePath != string.Empty);
		Assert.IsTrue(image.Heigth != 0);
		Assert.IsTrue(image.ImageData.Length > 0);
		Assert.IsTrue(image.Md5Hash != string.Empty);
		Assert.IsTrue(image.Width != 0);
	}
}
