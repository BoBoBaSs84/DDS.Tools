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
	/// <param name="filePath"></param>
	/// <param name="md5String"></param>
	/// <param name="image"></param>
	public Todo(string fileName, string filePath, string md5String, DDSImage image)
	{
		FileName = fileName;
		FilePath = filePath;
		FullName = Path.Combine(filePath, fileName);
		MD5String = md5String;
		Image = image;
	}

	public string FileName { get; private set; }
	public string FilePath { get; private set; }
	public string FullName { get; private set; }
	public string MD5String { get; private set; }

	[JsonIgnore]
	public DDSImage Image { get; private set; }
}
