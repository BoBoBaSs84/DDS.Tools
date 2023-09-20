using Shared.Library.Factories;
using Shared.Library.Interfaces;

namespace Shared.LibraryTests.Classes.Images;

[TestClass]
[DeploymentItem(TestConstants.PngImage, TestConstants.SourceFolder)]
public class PNGImageTests
{
	private readonly string _sourcePath;
	private readonly string _targetPath;

	public PNGImageTests()
	{
		_sourcePath = Path.Combine(AppContext.BaseDirectory, TestConstants.SourceFolder);
		_targetPath = Path.Combine(AppContext.BaseDirectory, TestConstants.TargetFolder);
	}

	[TestMethod]
	[DeploymentItem(TestConstants.PngImage, TestConstants.SourceFolder)]
	public void SaveTest()
	{
		string sourceFolder = Path.Combine(_sourcePath, TestConstants.PngImage);
		string targetFolder = Path.Combine(_targetPath, TestConstants.DdsImageSave);

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
	[DeploymentItem(TestConstants.PngImage, TestConstants.SourceFolder)]
	[ExpectedException(typeof(ArgumentOutOfRangeException))]
	public void SaveWithCompressionExceptionTest()
	{
		int compressionLevel = 99;
		string sourceFolder = Path.Combine(_sourcePath, TestConstants.PngImage);
		string targetFolder = Path.Combine(_targetPath, TestConstants.DdsImageSave);

		IImage image = ImageFactory.CreatePngImage(sourceFolder);
		image.Save(targetFolder, compressionLevel);
	}

	[TestMethod]
	[DeploymentItem(TestConstants.PngImage, TestConstants.SourceFolder)]
	public void SaveWithCompressionTest()
	{
		int compressionLevel = 0;
		string sourceFolder = Path.Combine(_sourcePath, TestConstants.PngImage);
		string targetFolder = Path.Combine(_targetPath, $"{compressionLevel}_{TestConstants.DdsImageSave}");

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
