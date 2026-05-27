// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Common;
using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Settings.Base;

using Microsoft.Extensions.Logging;

using Spectre.Console;
using Spectre.Console.Cli;

namespace DDS.Tools.Commands.Base;

/// <summary>
/// The convert command base class.
/// </summary>
/// <inheritdoc/>
/// <param name="loggerService">The logger service instance to use.</param>
/// <param name="todoService">The todo service instance to use.</param>
/// <param name="directoryProvider">The directory provider instance to use.</param>
/// <param name="fileProvider">The file provider instance to use.</param>
/// <param name="pathProvider">The path provider instance to use.</param>
internal abstract class ConvertCommandBase<TSettings, TCommand>(
	ILoggerService<TCommand> loggerService,
	ITodoService todoService,
	IDirectoryProvider directoryProvider,
	IFileProvider fileProvider,
	IPathProvider pathProvider) : Command<TSettings>
	where TSettings : CommandSettings
	where TCommand : class
{
	private readonly ILoggerService<TCommand> _loggerService = loggerService;
	private readonly IDirectoryProvider _directoryProvider = directoryProvider;
	private readonly IFileProvider _fileProvider = fileProvider;
	private readonly IPathProvider _pathProvider = pathProvider;

	private static readonly Action<ILogger, Exception?> LogException =
		LoggerMessage.Define(LogLevel.Error, 0, "Exception occured.");

	protected int ExecuteCommand(ConvertSettingsBase settings, ImageType imageType)
	{
		try
		{
			return AnsiConsole.Status()
				.Spinner(Spinner.Known.Line)
				.Start("Processing..", action => Action(settings, imageType));
		}
		catch (Exception ex)
		{
			_loggerService.Log(LogException, ex);
			AnsiConsole.MarkupLine($"[maroon]{ex.Message}[/]");
			return 1;
		}
	}

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
