using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Models;

using Microsoft.Extensions.DependencyInjection;

namespace DDS.ToolsTests.Models;

[TestClass()]
public class DdsImageModelTests : UnitTestBase
{
	private static readonly string ResourcePath = Path.Combine(Environment.CurrentDirectory, "Resources");
	private static readonly string FilePath = Path.Combine(ResourcePath, "outfit101m.dds");
	private static readonly string NewFilePath = Path.Combine(ResourcePath, "new_outfit101m.png");

	[ClassInitialize]
	public static void ClassInitialize(TestContext context)
	{
		if (File.Exists(NewFilePath))
			File.Delete(NewFilePath);
	}

	[TestMethod]
	public void LoadTest()
	{
		IImageModel image = ServiceProvider.GetRequiredKeyedService<IImageModel>(ImageType.DDS);

		image.Load(FilePath);

		Assert.IsNotNull(image);
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
		IImageModel image = ServiceProvider.GetRequiredKeyedService<IImageModel>(ImageType.DDS);
		image?.Load(@"D:\");
	}

	[TestMethod]
	public void SaveTest()
	{
		IImageModel image = ServiceProvider.GetRequiredKeyedService<IImageModel>(ImageType.DDS);
		image.Load(FilePath);

		image.Save(NewFilePath);

		Assert.IsTrue(File.Exists(NewFilePath));
	}

	[TestMethod]
	public void SaveExceptionTest()
	{
		IImageModel image = ServiceProvider.GetRequiredKeyedService<IImageModel>(ImageType.DDS);
		image.Load(FilePath);

		image.Save(FilePath);
	}
}
