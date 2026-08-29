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
/// Encodes a format-neutral <see cref="ImageCanvas"/> into the bytes of one image file format.
/// </summary>
internal interface IImageEncoder
{
	/// <summary>The image file format this encoder produces.</summary>
	ImageType Format { get; }

	/// <summary>
	/// Encodes the provided <paramref name="canvas"/> into a file of <see cref="Format"/>.
	/// </summary>
	/// <param name="canvas">The image to encode.</param>
	/// <param name="compression">How hard the encoder should work to shrink the output.</param>
	/// <returns>The encoded file bytes.</returns>
	byte[] Encode(ImageCanvas canvas, CompressionLevel compression);
}
