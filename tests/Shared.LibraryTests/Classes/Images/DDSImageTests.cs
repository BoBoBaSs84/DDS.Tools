using Shared.Library.Factories;
using Shared.Library.Interfaces;

namespace Shared.LibraryTests.Classes.Images;

[TestClass]
[DeploymentItem(TestConstants.DdsImage, TestConstants.SourceFolder)]
public class DDSImageTests
{
	private readonly string _sourcePath;
	private readonly string _targetPath;

	public DDSImageTests()
	{
		_sourcePath = Path.Combine(AppContext.BaseDirectory, TestConstants.SourceFolder);
		_targetPath = Path.Combine(AppContext.BaseDirectory, TestConstants.TargetFolder);

		Directory.CreateDirectory(_sourcePath);
		Directory.CreateDirectory(_targetPath);
	}

	[TestMethod]
	public void SaveTest()
	{
		string sourceFolder = Path.Combine(_sourcePath, TestConstants.DdsImage);
		string targetFolder = Path.Combine(_targetPath, TestConstants.PngImageSave);

		IImage image = ImageFactory.CreateDdsImage(sourceFolder);
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
	[ExpectedException(typeof(ArgumentOutOfRangeException))]
	public void SaveWithCompressionExceptionTest()
	{
		int compressionLevel = 99;
		string sourceFolder = Path.Combine(_sourcePath, TestConstants.DdsImage);
		string targetFolder = Path.Combine(_targetPath, TestConstants.PngImageSave);

		IImage image = ImageFactory.CreateDdsImage(sourceFolder);
		image.Save(targetFolder, compressionLevel);
	}

	[TestMethod]
	public void SaveWithCompressionTest()
	{
		int compressionLevel = 9;
		string sourceFolder = Path.Combine(_sourcePath, TestConstants.DdsImage);
		string targetFolder = Path.Combine(_targetPath, $"{compressionLevel}_{TestConstants.PngImageSave}");

		IImage image = ImageFactory.CreateDdsImage(sourceFolder);
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
