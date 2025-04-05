// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace DDS.Tools.Interfaces.Providers;

/// <summary>
/// The interface for the directory provider.
/// </summary>
public interface IDirectoryProvider
{
	/// <inheritdoc cref="Directory.CreateDirectory(string)"/>
	DirectoryInfo CreateDirectory(string path);

	/// <inheritdoc cref="Directory.Exists(string?)"/>
	bool Exists([NotNullWhen(true)] string? path);

	/// <inheritdoc cref="Directory.GetFiles(string, string, SearchOption)"/>
	string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
}
