using System.Text.Json.Serialization;

namespace Shared.Library.Classes;

/// <summary>
/// The todo class
/// </summary>
public sealed class Todo
{
	/// <summary>
	/// Initializes an instance of <see cref="Todo"/> class.
	/// </summary>
	/// <param name="fileName"></param>
	/// <param name="relativePath"></param>
	/// <param name="fullPathName"></param>
	/// <param name="targetPath"></param>
	/// <param name="md5String"></param>
	public Todo(string fileName, string relativePath, string fullPathName, string targetPath, string md5String)
	{
		FileName = fileName;
		RelativePath = relativePath;
		FullPathName = fullPathName;
		TargetPath = targetPath;
		MD5String = md5String;
	}

	/// <summary>
	/// The name of the image file.
	/// </summary>
	public string FileName { get; }

	/// <summary>
	/// The relative path of the image file.
	/// </summary>
	public string RelativePath { get; }

	/// <summary>
	/// The full path and file name of the image.
	/// </summary>
	[JsonIgnore]
	public string FullPathName { get; }

	/// <summary>
	/// The target path of the image file.
	/// </summary>
	[JsonIgnore]
	public string TargetPath { get; }

	/// <summary>
	/// The md5 hash of the image file.
	/// </summary>
	public string MD5String { get; private set; }

	/// <summary>
	/// Should update the md5 checksum.
	/// </summary>
	/// <param name="md5String"></param>
	public void UpdateMd5(string md5String)
		=> MD5String = md5String;
}
