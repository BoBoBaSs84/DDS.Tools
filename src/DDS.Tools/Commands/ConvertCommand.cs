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
using DDS.Tools.Extensions;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Settings;

using Microsoft.Extensions.Logging;

using Spectre.Console;
using Spectre.Console.Cli;

namespace DDS.Tools.Commands;

/// <summary>
/// The convert command class.
/// </summary>
/// <param name="loggerService">The logger service instance to use.</param>
/// <param name="todoService">The todo service instance to use.</param>
/// <param name="directoryProvider">The directory provider instance to use.</param>
/// <param name="fileProvider">The file provider instance to use.</param>
/// <param name="pathProvider">The path provider instance to use.</param>
internal sealed class ConvertCommand(
	ILoggerService<ConvertCommand> loggerService,
	ITodoService todoService,
	IDirectoryProvider directoryProvider,
	IFileProvider fileProvider,
	IPathProvider pathProvider) : Command<ConvertSettings>
{
	private readonly ILoggerService<ConvertCommand> _loggerService = loggerService;
	private readonly ITodoService _todoService = todoService;
	private readonly IDirectoryProvider _directoryProvider = directoryProvider;
	private readonly IFileProvider _fileProvider = fileProvider;
	private readonly IPathProvider _pathProvider = pathProvider;

	private static readonly Action<ILogger, Exception?> LogException =
		LoggerMessage.Define(LogLevel.Error, 0, "Exception occured.");

	/// <inheritdoc/>
	protected override int Execute(CommandContext context, ConvertSettings settings, CancellationToken cancellationToken)
	{
		try
		{
			return AnsiConsole.Status()
				.Spinner(Spinner.Known.Line)
				.Start("Processing..", _ => Action(settings));
		}
		catch (Exception ex)
		{
			_loggerService.Log(LogException, ex);
			AnsiConsole.MarkupLine($"[maroon]{ex.Message}[/]");
			return 1;
		}
	}

	internal int Action(ConvertSettings settings)
	{
		if (!_directoryProvider.Exists(settings.SourceFolder))
			throw new CommandException($"Directory '{settings.SourceFolder}' not found.");

		settings.From ??= InferSourceFormat(settings.SourceFolder);

		// The result json is read from the source folder, but written to the target folder.
		string jsonFilePath = _pathProvider.Combine(settings.SourceFolder, Constants.ResultFileName);
		bool jsonExists = _fileProvider.Exists(jsonFilePath);

		TodoCollection todos = jsonExists
			? _todoService.GetTodos(settings, _fileProvider.ReadAllText(jsonFilePath))
			: _todoService.GetTodos(settings);

		if (todos.Count.Equals(0))
		{
			AnsiConsole.MarkupLine("[yellow]There is nothing todo![/]");
			return 1;
		}

		_todoService.GetTodosDone(todos, settings, jsonExists);
		return 0;
	}

	private ImageType InferSourceFormat(string sourceFolder)
	{
		ImageType[] present = [.. Enum.GetValues<ImageType>().Where(format => HasFilesOfFormat(sourceFolder, format))];

		return present.Length switch
		{
			1 => present[0],
			0 => throw new CommandException($"No convertible image files were found in '{sourceFolder}'."),
			_ => throw new CommandException("The source folder holds more than one image format. Specify '--from'.")
		};
	}

	private bool HasFilesOfFormat(string sourceFolder, ImageType format)
		=> format.GetFileExtensions()
			.Any(extension => _directoryProvider.GetFiles(sourceFolder, $"*.{extension}", SearchOption.AllDirectories).Length > 0);
}
