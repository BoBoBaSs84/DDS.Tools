using DDS.Tools.Common;
using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Settings.Base;

using Microsoft.Extensions.DependencyInjection;

using Spectre.Console;
using Spectre.Console.Cli;

namespace DDS.Tools.Commands.Base;

/// <summary>
/// The convert command base class.
/// </summary>
/// <inheritdoc/>
/// <param name="todoService">The todo service instance to use.</param>
/// <param name="serviceProvider">The service provier instance to use.</param>
internal abstract class ConvertCommandBase<TSettings>(ITodoService todoService, IServiceProvider serviceProvider) : Command<TSettings> where TSettings : CommandSettings
{
	private readonly IDirectoryProvider _directoryProvider = serviceProvider.GetRequiredService<IDirectoryProvider>();
	private readonly IFileProvider _fileProvider = serviceProvider.GetRequiredService<IFileProvider>();
	private readonly IPathProvider _pathProvider = serviceProvider.GetRequiredService<IPathProvider>();

	protected int Action(ConvertSettingsBase settings, ImageType imageType)
	{
		TodoCollection todos;

		if (!_directoryProvider.Exists(settings.SourceFolder))
			throw new CommandException($"Directory '{settings.SourceFolder}' not found.");

		string jsonFilePath = _pathProvider.Combine(settings.SourceFolder, Constants.ResultFileName);
		bool jsonExists = ResultJsonExists(jsonFilePath);

		if (!jsonExists)
		{
			todos = todoService.GetTodos(settings, imageType);
			return GetItDone(todoService, todos, settings, imageType, jsonExists);
		}
		else
		{
			string jsonFileContent = _fileProvider.ReadAllText(jsonFilePath);
			todos = todoService.GetTodos(settings, imageType, jsonFileContent);
			return GetItDone(todoService, todos, settings, imageType, jsonExists);
		}
	}

	private bool ResultJsonExists(string jsonFilePath)
		=> _fileProvider.Exists(jsonFilePath);

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
