// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.Collections.Concurrent;
using System.Text.Json.Serialization;

namespace DDS.Tools.Models;

/// <summary>
/// The todo model class.
/// </summary>
/// <param name="fileName">The name of the image file.</param>
/// <param name="relativePath">The relative path of the image file.</param>
/// <param name="fullPathName">The full path and file name of the image.</param>
/// <param name="targetFolder">The target path of the image file.</param>
/// <param name="fileHash">The md5 hash of the image file.</param>
public sealed class TodoModel(string fileName, string relativePath, string fullPathName, string targetFolder, string fileHash)
{
	/// <summary>
	/// The name of the image file.
	/// </summary>
	public string FileName { get; } = fileName;

	/// <summary>
	/// The relative path of the image file.
	/// </summary>
	public string RelativePath { get; } = relativePath;

	/// <summary>
	/// The full path and file name of the image.
	/// </summary>
	[JsonIgnore(Condition = JsonIgnoreCondition.Always)]
	public string FullPathName { get; } = fullPathName;

	/// <summary>
	/// The target path of the image file.
	/// </summary>
	[JsonIgnore(Condition = JsonIgnoreCondition.Always)]
	public string TargetFolder { get; } = targetFolder;

	/// <summary>
	/// The md5 hash of the image file.
	/// </summary>
	public string FileHash { get; } = fileHash;
}

/// <summary>
/// The todo collection class.
/// </summary>
public class TodoCollection : ConcurrentQueue<TodoModel>
{ }
