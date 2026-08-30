// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.Collections.Concurrent;
using System.Text.Json.Serialization;

using DDS.Tools.Enumerators;

namespace DDS.Tools.Models;

/// <summary>
/// The todo model class.
/// </summary>
/// <param name="fileName">The name of the image file.</param>
/// <param name="relativePath">The relative path of the image file.</param>
/// <param name="fullPathName">The full path and file name of the image.</param>
/// <param name="targetFolder">The target path of the image file.</param>
/// <param name="fileHash">The md5 hash of the image file.</param>
/// <param name="sourceType">The image format of the file to decode.</param>
/// <param name="originalName">The original file name including its extension.</param>
public sealed class TodoModel(
	string fileName,
	string relativePath,
	string fullPathName,
	string targetFolder,
	string fileHash,
	ImageType sourceType,
	string originalName)
{
	/// <summary>
	/// The name of the image file. Holds the original name until the transformation
	/// records the name that was actually written.
	/// </summary>
	public string FileName { get; internal set; } = fileName;

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

	/// <summary>
	/// The image format of the file located at <see cref="FullPathName"/>, used to
	/// pick the decoder.
	/// </summary>
	[JsonConverter(typeof(JsonStringEnumConverter))]
	public ImageType SourceType { get; } = sourceType;

	/// <summary>
	/// The original file name including its extension.
	/// </summary>
	public string OriginalName { get; } = originalName;

	/// <summary>
	/// The explicit per-file encode target. Only set on the restore path; a
	/// <see langword="null"/> value means the target format comes from the settings.
	/// </summary>
	[JsonIgnore(Condition = JsonIgnoreCondition.Always)]
	public ImageType? TargetType { get; set; }
}

/// <summary>
/// The todo collection class.
/// </summary>
public class TodoCollection : ConcurrentQueue<TodoModel>
{ }
