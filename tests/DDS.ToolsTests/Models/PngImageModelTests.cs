// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Models;
using DDS.Tools.Settings;

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
		PngConvertSettings settings = new()
		{
			SourceFolder = TestConstants.PngResourcePath,
			TargetFolder = TestConstants.ResourcePath,
		};
		IImageModel image = ServiceProvider.GetRequiredKeyedService<IImageModel>(ImageType.PNG);
		image.Load(FilePath);

		image.Save(NewFilePath, settings);

		Assert.IsTrue(File.Exists(NewFilePath));
	}

	[TestMethod]
	public void SaveExceptionTest()
	{
		PngConvertSettings settings = new()
		{
			SourceFolder = TestConstants.PngResourcePath,
			TargetFolder = TestConstants.ResourcePath,
		};
		IImageModel image = ServiceProvider.GetRequiredKeyedService<IImageModel>(ImageType.PNG);
		image.Load(FilePath);

		image.Save(FilePath, settings);
	}
}
