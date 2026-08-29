// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;
using DDS.Tools.Imaging;

namespace DDS.Tools.Interfaces.Imaging;

/// <summary>
/// Decodes the bytes of one image file format into a format-neutral <see cref="ImageCanvas"/>.
/// </summary>
internal interface IImageDecoder
{
	/// <summary>The image file format this decoder handles.</summary>
	ImageType Format { get; }

	/// <summary>
	/// Decodes the provided file <paramref name="data"/> into an RGBA canvas.
	/// </summary>
	/// <param name="data">The raw bytes of an image file of <see cref="Format"/>.</param>
	/// <returns>The decoded image.</returns>
	ImageCanvas Decode(byte[] data);
}
