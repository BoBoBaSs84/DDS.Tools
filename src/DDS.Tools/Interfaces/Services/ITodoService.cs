// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Models;
using DDS.Tools.Settings;

namespace DDS.Tools.Interfaces.Services;

/// <summary>
/// The todo service interface.
/// </summary>
internal interface ITodoService
{
	/// <summary>
	/// Returns a collection of todos discovered from the source folder.
	/// </summary>
	/// <param name="settings">The settings that need to be considered.</param>
	/// <returns>A collection of todos.</returns>
	TodoCollection GetTodos(ConvertSettings settings);

	/// <summary>
	/// Returns a collection of todos derived from a previous run's result json.
	/// </summary>
	/// <param name="settings">The settings that need to be considered.</param>
	/// <param name="jsonFileContent">The json content to work with.</param>
	/// <returns>A collection of todos.</returns>
	TodoCollection GetTodos(ConvertSettings settings, string jsonFileContent);

	/// <summary>
	/// Get the todos done.
	/// </summary>
	/// <param name="todos">The collection of todos.</param>
	/// <param name="settings">The settings that need to be considered.</param>
	/// <param name="jsonExists">Do the todos come from a json result file?</param>
	void GetTodosDone(TodoCollection todos, ConvertSettings settings, bool jsonExists = false);
}
