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
	/// Returns every file extension (without a leading dot) that identifies the
	/// provided <paramref name="imageType"/> on disk.
	/// </summary>
	/// <param name="imageType">The image type to get the extensions for.</param>
	/// <returns>One or more lower-case file extensions.</returns>
	/// <exception cref="ArgumentOutOfRangeException">If the image type is not supported.</exception>
	internal static string[] GetFileExtensions(this ImageType imageType)
		=> imageType switch
		{
			ImageType.DDS => ["dds"],
			ImageType.PNG => ["png"],
			ImageType.TGA => ["tga"],
			ImageType.JPG => ["jpg", "jpeg"],
			_ => throw new ArgumentOutOfRangeException(nameof(imageType), imageType, null)
		};

	/// <summary>
	/// Returns the canonical file extension (without a leading dot) used when
	/// writing a file of the provided <paramref name="imageType"/>.
	/// </summary>
	/// <param name="imageType">The image type to get the extension for.</param>
	/// <returns>The lower-case file extension.</returns>
	/// <exception cref="ArgumentOutOfRangeException">If the image type is not supported.</exception>
	internal static string GetPrimaryExtension(this ImageType imageType)
		=> imageType.GetFileExtensions()[0];
}
