// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
namespace DDS.Tools.Enumerators;

/// <summary>
/// The amount of effort the target encoder should spend compressing the output.
/// </summary>
/// <remarks>
/// The value is an intent that each encoder maps onto its own knobs
/// (block-compression quality for DDS, container quality for the WIC formats).
/// </remarks>
public enum CompressionLevel : byte
{
	/// <summary>No compression, largest output, fastest to write.</summary>
	None = 0,

	/// <summary>Light compression favouring speed over size.</summary>
	Fast = 1,

	/// <summary>Balanced trade-off between size and speed. The default.</summary>
	Balanced = 2,

	/// <summary>Maximum compression, smallest output, slowest to write.</summary>
	Maximum = 3
}
