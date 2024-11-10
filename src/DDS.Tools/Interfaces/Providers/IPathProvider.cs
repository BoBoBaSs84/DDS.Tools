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
