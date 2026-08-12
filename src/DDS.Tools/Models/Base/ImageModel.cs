// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Interfaces.Models;
using DDS.Tools.Settings.Base;

using SkiaSharp;

namespace DDS.Tools.Models.Base;

/// <summary>
/// The base class for image models.
/// </summary>
internal abstract class ImageModel : IImageModel
{
	/// <summary>
	/// The decoded image, backed by native memory that is released on <see cref="Dispose"/>.
	/// </summary>
	protected SKBitmap? Bitmap { get; set; }

	/// <inheritdoc/>
	public string Name { get; protected set; } = string.Empty;
	/// <inheritdoc/>
	public string Path { get; protected set; } = string.Empty;
	/// <inheritdoc/>
	public int Height { get; protected set; } = default;
	/// <inheritdoc/>
	public int Width { get; protected set; } = default;
	/// <inheritdoc/>
	public byte[] Data { get; protected set; } = [];
	/// <inheritdoc/>
	public string Hash { get; protected set; } = string.Empty;

	/// <inheritdoc/>
	public abstract void Load(string filePath);

	/// <inheritdoc/>
	public abstract void Save(string filePath, ConvertSettingsBase settings);

	/// <inheritdoc/>
	public void Dispose()
	{
		Bitmap?.Dispose();
		Bitmap = null;
		GC.SuppressFinalize(this);
	}
}
