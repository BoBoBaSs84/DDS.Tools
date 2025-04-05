// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Interfaces.Models;

namespace DDS.Tools.Models.Base;

/// <summary>
/// The image file class.
/// </summary>
internal abstract class ImageModel : IImageModel
{
	/// <inheritdoc/>
	public string Name { get; protected set; } = string.Empty;
	/// <inheritdoc/>
	public string Path { get; protected set; } = string.Empty;
	/// <inheritdoc/>
	public int Heigth { get; protected set; } = default;
	/// <inheritdoc/>
	public int Width { get; protected set; } = default;
	/// <inheritdoc/>
	public byte[] Data { get; protected set; } = [];
	/// <inheritdoc/>
	public string Hash { get; protected set; } = string.Empty;

	/// <inheritdoc/>
	public abstract void Load(string filePath);

	/// <inheritdoc/>
	public abstract void Save(string filePath);
}
