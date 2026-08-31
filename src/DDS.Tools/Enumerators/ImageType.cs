// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using BB84.SourceGenerators.Attributes;

namespace DDS.Tools.Enumerators;

/// <summary>
/// The image file types.
/// </summary>
[GenerateEnumeratorExtensions]
public enum ImageType : byte
{
	/// <summary>
	/// The direct draw surface image type.
	/// </summary>
	DDS = 1,

	/// <summary>
	/// The portable network  graphics image type.
	/// </summary>
	PNG = 2,

	/// <summary>
	/// The truevision TGA (targa) image type.
	/// </summary>
	TGA = 3,

	/// <summary>
	/// The JPEG image type.
	/// </summary>
	JPG = 4
}

public partial class ImageTypeExtensions
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

	/// <summary>
	/// Resolves the <see cref="ImageType"/> that owns the provided file extension or
	/// file name, mirroring <see cref="GetFileExtensions(ImageType)"/>.
	/// </summary>
	/// <param name="extensionOrFileName">A file extension (with or without a leading dot) or a file name.</param>
	/// <param name="imageType">The resolved image type when the lookup succeeds.</param>
	/// <returns><see langword="true"/> when the extension maps to a known image type.</returns>
	internal static bool TryGetImageType(string extensionOrFileName, out ImageType imageType)
	{
		string extension = Path.GetExtension(extensionOrFileName);

		if (extension.Length.Equals(0))
			extension = extensionOrFileName;

		extension = extension.TrimStart('.').ToLowerInvariant();

		foreach (ImageType candidate in GetValuesFast())
		{
			if (candidate.GetFileExtensions().Contains(extension))
			{
				imageType = candidate;
				return true;
			}
		}

		imageType = default;
		return false;
	}
}
