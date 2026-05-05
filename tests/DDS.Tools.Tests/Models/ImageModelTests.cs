// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Models.Base;
using DDS.Tools.Settings;
using DDS.Tools.Settings.Base;

namespace DDS.Tools.Tests.Models;

[TestClass]
public sealed class ImageModelTests
{
	[TestMethod]
	public void ImageModelDefaultsTest()
	{
		TestImageModel model = new();

		Assert.AreEqual(string.Empty, model.Name);
		Assert.AreEqual(string.Empty, model.Path);
		Assert.AreEqual(0, model.Heigth);
		Assert.AreEqual(0, model.Width);
		Assert.AreEqual(0, model.Data.Length);
		Assert.AreEqual(string.Empty, model.Hash);
	}

	[TestMethod]
	public void ImageModelLoadAndSaveTest()
	{
		TestImageModel model = new();

		model.Load(@"X:\Source\32.png");
		model.Save(@"X:\Target\32.dds", new DdsConvertSettings());

		Assert.AreEqual("32.png", model.Name);
		Assert.AreEqual(@"X:\Target\32.dds", model.Path);
		Assert.AreEqual(32, model.Heigth);
		Assert.AreEqual(32, model.Width);
		Assert.AreEqual(3, model.Data.Length);
		Assert.AreEqual("HASH", model.Hash);
	}

	private sealed class TestImageModel : ImageModel
	{
		public override void Load(string filePath)
		{
			Name = "32.png";
			Path = filePath;
			Heigth = 32;
			Width = 32;
			Data = [1, 2, 3];
			Hash = "HASH";
		}

		public override void Save(string filePath, ConvertSettingsBase settings)
			=> Path = filePath;
	}
}
