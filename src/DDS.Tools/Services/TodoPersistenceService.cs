// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using BB84.Extensions.Serialization;

using DDS.Tools.Common;
using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Models;
using DDS.Tools.Settings.Base;

using Spectre.Console;

namespace DDS.Tools.Services;

/// <summary>
/// Handles persisting todo processing results and final reporting output.
/// </summary>
/// <param name="fileProvider">The file provider instance to use.</param>
/// <param name="pathProvider">The path provider instance to use.</param>
internal sealed class TodoPersistenceService(IFileProvider fileProvider, IPathProvider pathProvider)
{
	private readonly IFileProvider _fileProvider = fileProvider;
	private readonly IPathProvider _pathProvider = pathProvider;

	internal void PersistResult(TodoCollection todos, ConvertSettingsBase settings, bool jsonExists, TodoProcessingResult result)
	{
		if (!jsonExists && settings.ConvertMode.Equals(ConvertModeType.Automatic))
		{
			string jsonContent = todos.ToJson();
			string jsonFilePath = _pathProvider.Combine(settings.TargetFolder, Constants.ResultFileName);
			_fileProvider.WriteAllText(jsonFilePath, jsonContent);
		}

		AnsiConsole.MarkupLine($"[green]Todos done:\t{result.TodosDoneCount}[/]");
		AnsiConsole.MarkupLine($"[yellow]Duplicates:\t{result.DuplicatesCount}[/]");
	}
}
