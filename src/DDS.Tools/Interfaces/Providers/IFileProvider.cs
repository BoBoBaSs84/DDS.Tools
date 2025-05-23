﻿// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.Diagnostics.CodeAnalysis;

namespace DDS.Tools.Interfaces.Providers;

/// <summary>
/// The interface for the file provider.
/// </summary>
public interface IFileProvider
{
	/// <inheritdoc cref="File.Copy(string, string)"/>
	void Copy(string sourceFileName, string destFileName);

	/// <inheritdoc cref="File.Exists(string?)"/>
	bool Exists([NotNullWhen(true)] string? path);

	/// <inheritdoc cref="File.Move(string, string)"/>
	void Move(string sourceFileName, string destFileName);

	/// <inheritdoc cref="File.ReadAllText(string)"/>
	string ReadAllText(string path);

	/// <inheritdoc cref="File.WriteAllText(string, string?)"/>
	void WriteAllText(string path, string? contents);
}
