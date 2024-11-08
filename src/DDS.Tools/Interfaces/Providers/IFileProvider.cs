namespace DDS.Tools.Interfaces.Providers;

/// <summary>
/// The interface for the file provider.
/// </summary>
public interface IFileProvider
{
	/// <inheritdoc cref="File.Move(string, string)"/>
	void Move(string sourceFileName, string destFileName);

	/// <inheritdoc cref="File.WriteAllText(string, string?)"/>
	void WriteAllText(string path, string? contents);
}
