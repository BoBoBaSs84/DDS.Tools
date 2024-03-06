using System.Text.Json.Serialization;

namespace DDS.Tools.Models;

/// <summary>
/// The todo model class.
/// </summary>
/// <param name="fileName">The name of the image file.</param>
/// <param name="relativePath">The relative path of the image file.</param>
/// <param name="fullPathName">The full path and file name of the image.</param>
/// <param name="targetPath">The target path of the image file.</param>
/// <param name="hash">The md5 hash of the image file.</param>
public sealed class TodoModel(string fileName, string relativePath, string fullPathName, string targetPath, string hash)
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
	[JsonIgnore]
	public string FullPathName { get; } = fullPathName;

	/// <summary>
	/// The target path of the image file.
	/// </summary>
	[JsonIgnore]
	public string TargetPath { get; } = targetPath;

	/// <summary>
	/// The md5 hash of the image file.
	/// </summary>
	public string Hash { get; private set; } = hash;
}

/// <summary>
/// The todo collection class.
/// </summary>
public class TodoCollection : List<TodoModel>
{ }
