// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
namespace DDS.Tools.Imaging;

/// <summary>
/// A decoded image in a format-neutral layout: tightly packed, top-down,
/// 32-bit straight-alpha RGBA pixels.
/// </summary>
/// <remarks>
/// This is the hand-off type between an <see cref="Interfaces.Imaging.IImageDecoder"/>
/// and an <see cref="Interfaces.Imaging.IImageEncoder"/>. It owns plain managed
/// memory, so unlike the codecs themselves it needs no disposal.
/// </remarks>
/// <param name="width">The image width in pixels.</param>
/// <param name="height">The image height in pixels.</param>
/// <param name="rgba">The pixel buffer, <c>width * height * 4</c> bytes, row-major.</param>
internal sealed class ImageCanvas(int width, int height, byte[] rgba)
{
	/// <summary>The image width in pixels.</summary>
	internal int Width { get; } = width;

	/// <summary>The image height in pixels.</summary>
	internal int Height { get; } = height;

	/// <summary>The tightly packed RGBA pixel buffer.</summary>
	internal byte[] Rgba { get; } = rgba;
}
