// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;

namespace DDS.Tools.Extensions;

/// <summary>
/// The image type extensions class.
/// </summary>
internal static class ImageTypeExtensions
{
	/// <summary>
	/// Returns the image type the provided image type is converted into.
	/// </summary>
	/// <param name="imageType">The image type to get the counterpart for.</param>
	/// <returns>The image type of the conversion target.</returns>
	/// <exception cref="ArgumentOutOfRangeException">If the image type is not supported.</exception>
	internal static ImageType GetTargetType(this ImageType imageType)
		=> imageType switch
		{
			ImageType.DDS => ImageType.PNG,
			ImageType.PNG => ImageType.DDS,
			_ => throw new ArgumentOutOfRangeException(nameof(imageType), imageType, null)
		};
}
