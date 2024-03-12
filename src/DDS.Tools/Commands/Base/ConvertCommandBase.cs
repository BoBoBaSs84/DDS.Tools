using DDS.Tools.Common;
using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Settings.Base;

using Spectre.Console;
using Spectre.Console.Cli;

namespace DDS.Tools.Commands.Base;

/// <summary>
/// The convert command base class.
/// </summary>
/// <inheritdoc/>
internal abstract class ConvertCommandBase<TSettings>(ITodoService todoService) : Command<TSettings> where TSettings : CommandSettings
{
	protected int Action(ConvertSettingsBase settings, ImageType imageType)
	{
		TodoCollection todos;

		if (!Directory.Exists(settings.SourceFolder))
			throw new CommandException($"Directory '{settings.SourceFolder}' not found.");

		string jsonFilePath = Path.Combine(settings.SourceFolder, Constants.ResultFileName);
		bool jsonExists = ResultJsonExists(jsonFilePath);

		if (!jsonExists)
		{
			todos = todoService.GetTodos(settings, imageType);
			return GetItDone(todoService, todos, settings, imageType, jsonExists);
		}
		else
		{
			string jsonFileContent = File.ReadAllText(jsonFilePath);
			todos = todoService.GetTodos(settings, imageType, jsonFileContent);
			return GetItDone(todoService, todos, settings, imageType, jsonExists);
		}
	}

	private static bool ResultJsonExists(string jsonFilePath) => File.Exists(jsonFilePath);

	private static int GetItDone(ITodoService todoService, TodoCollection todos, ConvertSettingsBase settings, ImageType imageType, bool jsonExists)
	{
		if (todos.Count.Equals(0))
		{
			AnsiConsole.MarkupLine($"[yellow]There is nothing todo![/]");
			return 1;
		}

		todoService.GetTodosDone(todos, settings, imageType, jsonExists);
		return 0;
	}
}
