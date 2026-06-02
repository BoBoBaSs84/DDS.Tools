// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
namespace DDS.Tools.Enumerators;

/// <summary>
/// Specifies the compression level for PNG encoding.
/// Higher values produce smaller files at the cost of encoding speed.
/// </summary>
public enum PngCompressionLevel
{
	/// <summary>No compression is applied.</summary>
	NoCompression = 0,

	/// <summary>Fastest compression, largest file size.</summary>
	BestSpeed = 1,

	/// <summary>Compression level 2.</summary>
	Level2 = 2,

	/// <summary>Compression level 3.</summary>
	Level3 = 3,

	/// <summary>Compression level 4.</summary>
	Level4 = 4,

	/// <summary>Compression level 5.</summary>
	Level5 = 5,

	/// <summary>Default compression level, balancing size and speed.</summary>
	DefaultCompression = 6,

	/// <summary>Compression level 7.</summary>
	Level7 = 7,

	/// <summary>Compression level 8.</summary>
	Level8 = 8,

	/// <summary>Best compression, smallest file size.</summary>
	BestCompression = 9,
}
