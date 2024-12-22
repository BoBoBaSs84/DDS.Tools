using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Models;

using Microsoft.Extensions.DependencyInjection;

namespace DDS.ToolsTests.Models;

[TestClass]
public sealed class PngImageModelTests : UnitTestBase
{
	private static readonly string FilePath = Path.Combine(TestConstants.PngResourcePath, "32.png");
	private static readonly string NewFilePath = Path.Combine(TestConstants.ResourcePath, "new_32.dds");

	[TestCleanup]
	public void TestCleanup()
	{
		if (File.Exists(NewFilePath))
			File.Delete(NewFilePath);
	}

	[TestMethod]
	public void LoadTest()
	{
		IImageModel image = ServiceProvider.GetRequiredKeyedService<IImageModel>(ImageType.PNG);

		image.Load(FilePath);

		Assert.AreNotEqual(string.Empty, image.Name);
		Assert.AreNotEqual(string.Empty, image.Path);
		Assert.AreNotEqual(0, image.Width);
		Assert.AreNotEqual(0, image.Heigth);
		Assert.AreNotEqual([], image.Data);
		Assert.AreNotEqual(string.Empty, image.Hash);
	}

	[TestMethod]
	public void LoadExceptionTest()
	{
		IImageModel image = ServiceProvider.GetRequiredKeyedService<IImageModel>(ImageType.PNG);
		image?.Load(@"D:\");
	}

	[TestMethod]
	public void SaveTest()
	{
		IImageModel image = ServiceProvider.GetRequiredKeyedService<IImageModel>(ImageType.PNG);
		image.Load(FilePath);

		image.Save(NewFilePath);

		Assert.IsTrue(File.Exists(NewFilePath));
	}

	[TestMethod]
	public void SaveExceptionTest()
	{
		IImageModel image = ServiceProvider.GetRequiredKeyedService<IImageModel>(ImageType.PNG);
		image.Load(FilePath);

		image.Save(FilePath);
	}
}
