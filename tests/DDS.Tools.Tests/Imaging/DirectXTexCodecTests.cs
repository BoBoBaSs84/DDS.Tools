// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;
using DDS.Tools.Imaging;

namespace DDS.Tools.Tests.Imaging;

[TestClass]
public sealed class DirectXTexCodecTests
{
	private static readonly DirectXTexCodec DdsCodec = new(ImageType.DDS);
	private static readonly DirectXTexCodec PngCodec = new(ImageType.PNG);
	private static readonly DirectXTexCodec TgaCodec = new(ImageType.TGA);
	private static readonly DirectXTexCodec JpgCodec = new(ImageType.JPG);

	[TestMethod]
	public void DecodeDdsYieldsAnRgbaCanvas()
	{
		byte[] dds = File.ReadAllBytes(Path.Combine(TestConstants.DdsResourcePath, "32A.dds"));

		ImageCanvas canvas = DdsCodec.Decode(dds);

		Assert.AreEqual(32, canvas.Width);
		Assert.AreEqual(32, canvas.Height);
		Assert.AreEqual(32 * 32 * 4, canvas.Rgba.Length);
	}

	[TestMethod]
	public void DecodePngYieldsAnRgbaCanvas()
	{
		byte[] png = File.ReadAllBytes(Path.Combine(TestConstants.PngResourcePath, "32A.png"));

		ImageCanvas canvas = PngCodec.Decode(png);

		Assert.AreEqual(32, canvas.Width);
		Assert.AreEqual(32, canvas.Height);
		Assert.AreEqual(32 * 32 * 4, canvas.Rgba.Length);
	}

	[TestMethod]
	public void DdsToPngRoundTripKeepsDimensions()
	{
		byte[] dds = File.ReadAllBytes(Path.Combine(TestConstants.DdsResourcePath, "32A.dds"));

		byte[] png = PngCodec.Encode(DdsCodec.Decode(dds), CompressionLevel.Balanced);
		ImageCanvas roundTrip = PngCodec.Decode(png);

		Assert.IsGreaterThan(0, png.Length);
		Assert.AreEqual(32, roundTrip.Width);
		Assert.AreEqual(32, roundTrip.Height);
	}

	[TestMethod]
	public void PngToDdsRoundTripKeepsDimensions()
	{
		byte[] png = File.ReadAllBytes(Path.Combine(TestConstants.PngResourcePath, "32A.png"));

		byte[] dds = DdsCodec.Encode(PngCodec.Decode(png), CompressionLevel.Balanced);
		ImageCanvas roundTrip = DdsCodec.Decode(dds);

		Assert.IsGreaterThan(0, dds.Length);
		Assert.AreEqual(32, roundTrip.Width);
		Assert.AreEqual(32, roundTrip.Height);
	}

	[TestMethod]
	public void DecodeTgaYieldsAnRgbaCanvas()
	{
		byte[] tga = File.ReadAllBytes(Path.Combine(TestConstants.TgaResourcePath, "32A.tga"));

		ImageCanvas canvas = TgaCodec.Decode(tga);

		Assert.AreEqual(32, canvas.Width);
		Assert.AreEqual(32, canvas.Height);
		Assert.AreEqual(32 * 32 * 4, canvas.Rgba.Length);
	}

	[TestMethod]
	public void DecodeJpgYieldsAnRgbaCanvas()
	{
		byte[] jpg = File.ReadAllBytes(Path.Combine(TestConstants.JpgResourcePath, "32.jpg"));

		ImageCanvas canvas = JpgCodec.Decode(jpg);

		Assert.AreEqual(32, canvas.Width);
		Assert.AreEqual(32, canvas.Height);
		Assert.AreEqual(32 * 32 * 4, canvas.Rgba.Length);
	}

	[TestMethod]
	[DataRow(ImageType.PNG)]
	[DataRow(ImageType.DDS)]
	public void TgaConvertsToOtherFormatsKeepingDimensions(ImageType target)
	{
		byte[] tga = File.ReadAllBytes(Path.Combine(TestConstants.TgaResourcePath, "32A.tga"));
		DirectXTexCodec encoder = new(target);

		byte[] encoded = encoder.Encode(TgaCodec.Decode(tga), CompressionLevel.Balanced);
		ImageCanvas roundTrip = encoder.Decode(encoded);

		Assert.IsGreaterThan(0, encoded.Length);
		Assert.AreEqual(32, roundTrip.Width);
		Assert.AreEqual(32, roundTrip.Height);
	}

	[TestMethod]
	public void PngToTgaRoundTripKeepsDimensions()
	{
		byte[] png = File.ReadAllBytes(Path.Combine(TestConstants.PngResourcePath, "32A.png"));

		byte[] tga = TgaCodec.Encode(PngCodec.Decode(png), CompressionLevel.None);
		ImageCanvas roundTrip = TgaCodec.Decode(tga);

		Assert.AreEqual(32, roundTrip.Width);
		Assert.AreEqual(32, roundTrip.Height);
	}

	[TestMethod]
	public void JpgToDdsRoundTripKeepsDimensions()
	{
		byte[] jpg = File.ReadAllBytes(Path.Combine(TestConstants.JpgResourcePath, "32A.jpg"));

		byte[] dds = DdsCodec.Encode(JpgCodec.Decode(jpg), CompressionLevel.Balanced);
		ImageCanvas roundTrip = DdsCodec.Decode(dds);

		Assert.AreEqual(32, roundTrip.Width);
		Assert.AreEqual(32, roundTrip.Height);
	}

	[TestMethod]
	public void EncodeOpaqueImageStaysWithinExpectedSize()
	{
		byte[] png = File.ReadAllBytes(Path.Combine(TestConstants.PngResourcePath, "32.png"));

		byte[] dds = DdsCodec.Encode(PngCodec.Decode(png), CompressionLevel.Maximum);

		// A 32x32 BC1 texture with a full mip chain is well under 4 KiB.
		Assert.IsLessThan(4096, dds.Length);
	}
}
