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
	private const string ResultFileName = "Result.json";

	protected int Action(ConvertSettingsBase settings, ImageType imageType)
	{
		TodoCollection todos;

		if (!Directory.Exists(settings.SourceFolder))
			throw new CommandException($"Directory '{settings.SourceFolder}' not found.");

		string jsonFilePath = Path.Combine(settings.SourceFolder, ResultFileName);
		bool resultFileExists = File.Exists(jsonFilePath);

		todos = resultFileExists
			? todoService.GetTodosFromJson(settings, imageType, jsonFilePath)
			: todoService.GetTodos(settings, imageType);

		if (todos.Count.Equals(0))
		{
			AnsiConsole.MarkupLine($"[yellow]There is nothing todo![/]");
			return 1;
		}

		if (resultFileExists)
			todoService.GetTodosDoneFromJson(todos, settings, imageType);
		else
			todoService.GetTodosDone(todos, settings, imageType);

		return 0;
	}
}
