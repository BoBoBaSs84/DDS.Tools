// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using BCnEncoder.Encoder;
using BCnEncoder.Shared;

namespace DDS.Tools.Models;

/// <summary>
/// The dds encoder wrapper class.
/// </summary>
internal sealed class DdsEncoder : BcEncoder
{
	/// <summary>
	/// Initializes an instance of <see cref="DdsEncoder"/> class.
	/// </summary>
	public DdsEncoder()
	{
		OutputOptions.FileFormat = OutputFileFormat.Dds;
		OutputOptions.Quality = CompressionQuality.BestQuality;
		OutputOptions.GenerateMipMaps = true;
		OutputOptions.Format = CompressionFormat.Bc1;
	}
}
