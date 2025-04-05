// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
namespace DDS.Tools.Interfaces.Providers;

/// <summary>
/// The interface for the path provider.
/// </summary>
public interface IPathProvider
{
	/// <inheritdoc cref="Path.Combine(string, string)"/>
	string Combine(string path1, string path2);

	/// <inheritdoc cref="Path.Combine(string, string, string)"/>
	string Combine(string path1, string path2, string path3);
}
