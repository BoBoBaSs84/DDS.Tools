using DDS.Tools.Enumerators;
using DDS.Tools.Models;
using DDS.Tools.Settings.Base;

namespace DDS.Tools.Interfaces.Services;

/// <summary>
/// The todo service interface.
/// </summary>
internal interface ITodoService
{
	/// <summary>
	/// Returns a collection of todos depending on the provided <see cref="ConvertSettingsBase"/>.
	/// </summary>
	/// <param name="settings">The settings that need to be considered.</param>
	/// <param name="imageType">The image type to work with.</param>
	/// <returns>A collection of todos.</returns>
	TodoCollection GetTodos(ConvertSettingsBase settings, ImageType imageType);

	/// <summary>
	/// Returns a collection of todos depending on the provided <see cref="ConvertSettingsBase"/>
	/// and the <paramref name="jsonContent"/>.
	/// </summary>
	/// <param name="settings">The settings that need to be considered.</param>
	/// <param name="imageType">The image type to work with.</param>
	/// <param name="jsonContent">The json content to work with.</param>
	/// <returns>A collection of todos.</returns>
	TodoCollection GetTodosFromJson(ConvertSettingsBase settings, ImageType imageType, string jsonContent);

	/// <summary>
	/// Get the todos done.
	/// </summary>
	/// <param name="todos">The collection of todos.</param>
	/// <param name="settings">The settings that need to be considered.</param>
	/// <param name="imageType">The image type to work with.</param>
	void GetTodosDone(TodoCollection todos, ConvertSettingsBase settings, ImageType imageType);

	/// <summary>
	/// Get the todos done in a different way.
	/// </summary>
	/// <param name="todos">The collection of todos.</param>
	/// <param name="settings">The settings that need to be considered.</param>
	/// <param name="imageType">The image type to work with.</param>
	void GetTodosDoneFromJson(TodoCollection todos, ConvertSettingsBase settings, ImageType imageType);
}
