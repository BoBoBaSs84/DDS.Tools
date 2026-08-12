// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.Text;

using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Models;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Settings;
using DDS.Tools.Tests;

using Microsoft.Extensions.DependencyInjection;

namespace DDS.Tools.Tests.Models;

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
		using IImageModel image = CreateImageModel();

		image.Load(FilePath);

		Assert.AreNotEqual(string.Empty, image.Name);
		Assert.AreNotEqual(string.Empty, image.Path);
		Assert.AreNotEqual(0, image.Width);
		Assert.AreNotEqual(0, image.Height);
		Assert.AreNotEqual([], image.Data);
		Assert.AreNotEqual(string.Empty, image.Hash);
	}

	[TestMethod]
	public void LoadExceptionTest()
	{
		using IImageModel image = CreateImageModel();
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
		using IImageModel image = CreateImageModel();
		image.Load(FilePath);

		image.Save(NewFilePath, settings);

		Assert.IsTrue(File.Exists(NewFilePath));
	}

	[TestMethod]
	public void SaveDoesNotLeakCompressionFormatBetweenImages()
	{
		PngConvertSettings settings = new()
		{
			SourceFolder = TestConstants.PngResourcePath,
			TargetFolder = TestConstants.ResourcePath,
		};

		// One encoder shared by both models, which is what the singleton registration produced.
		DdsEncoder encoder = ServiceProvider.GetRequiredService<DdsEncoder>();
		ILoggerService<PngImageModel> logger = ServiceProvider.GetRequiredService<ILoggerService<PngImageModel>>();

		string transparentTarget = Path.Combine(TestConstants.ResourcePath, "leak_transparent.dds");
		string opaqueTarget = Path.Combine(TestConstants.ResourcePath, "leak_opaque.dds");

		try
		{
			PngImageModel transparent = new(encoder, logger);
			transparent.Load(Path.Combine(TestConstants.PngResourcePath, "32A.png"));
			transparent.Save(transparentTarget, settings);

			PngImageModel opaque = new(encoder, logger);
			opaque.Load(Path.Combine(TestConstants.PngResourcePath, "32.png"));
			opaque.Save(opaqueTarget, settings);

			Assert.AreEqual("DXT5", ReadFourCC(transparentTarget));
			Assert.AreEqual("DXT1", ReadFourCC(opaqueTarget));
		}
		finally
		{
			File.Delete(transparentTarget);
			File.Delete(opaqueTarget);
		}
	}

	private static IImageModel CreateImageModel()
		=> ServiceProvider.GetRequiredService<Func<ImageType, IImageModel>>()(ImageType.PNG);

	/// <summary>
	/// Reads the four character code of the pixel format from a dds file header.
	/// </summary>
	private static string ReadFourCC(string filePath)
	{
		// 4 byte magic, then the pixel format four character code 80 bytes into the header.
		const int fourCCOffset = 84;

		using FileStream fileStream = File.OpenRead(filePath);
		byte[] buffer = new byte[4];

		fileStream.Position = fourCCOffset;
		fileStream.ReadExactly(buffer);

		return Encoding.ASCII.GetString(buffer);
	}

	[TestMethod]
	public void SaveExceptionTest()
	{
		PngConvertSettings settings = new()
		{
			SourceFolder = TestConstants.PngResourcePath,
			TargetFolder = TestConstants.ResourcePath,
		};
		using IImageModel image = CreateImageModel();
		image.Load(FilePath);

		image.Save(FilePath, settings);
	}
}
