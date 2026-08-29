// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;

namespace DDS.Tools.Interfaces.Imaging;

/// <summary>
/// Looks up the decoder or encoder registered for an image file format.
/// </summary>
internal interface IImageCodecRegistry
{
	/// <summary>
	/// Returns the decoder for the provided <paramref name="format"/>.
	/// </summary>
	/// <param name="format">The source image format.</param>
	/// <returns>The matching decoder.</returns>
	/// <exception cref="Exceptions.CommandException">If no decoder is registered for the format.</exception>
	IImageDecoder GetDecoder(ImageType format);

	/// <summary>
	/// Returns the encoder for the provided <paramref name="format"/>.
	/// </summary>
	/// <param name="format">The target image format.</param>
	/// <returns>The matching encoder.</returns>
	/// <exception cref="Exceptions.CommandException">If no encoder is registered for the format.</exception>
	IImageEncoder GetEncoder(ImageType format);
}
