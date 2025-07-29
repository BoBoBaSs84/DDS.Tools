// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using BB84.Extensions;
using BB84.Extensions.Serialization;

using DDS.Tools.Common;
using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Interfaces.Models;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Properties;
using DDS.Tools.Settings.Base;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Spectre.Console;

namespace DDS.Tools.Services;

/// <summary>
/// The todo service class.
/// </summary>
/// <param name="loggerService">The logger service instance to use.</param>
/// <param name="serviceProvider">The service provider instance to use.</param>
internal sealed class TodoService(ILoggerService<TodoService> loggerService, IServiceProvider serviceProvider) : ITodoService
{
	private readonly IDirectoryProvider _directoryProvider = serviceProvider.GetRequiredService<IDirectoryProvider>();
	private readonly IFileProvider _fileProvider = serviceProvider.GetRequiredService<IFileProvider>();
	private readonly IPathProvider _pathProvider = serviceProvider.GetRequiredService<IPathProvider>();
	private readonly ILoggerService<TodoService> _loggerService = loggerService;
	private readonly IServiceProvider _serviceProvider = serviceProvider;

	private readonly List<string> _todosDone = [];
	private int _todosDuplicateCount = 0;

	private static readonly Action<ILogger, Exception?> LogException =
		LoggerMessage.Define(LogLevel.Error, 0, "Exception occured.");

	/// <inheritdoc/>
	/// <exception cref="ServiceException"></exception>
	public TodoCollection GetTodos(ConvertSettingsBase settings, ImageType imageType)
	{
		try
		{
			TodoCollection todos = [];

			string[] files = _directoryProvider.GetFiles(settings.SourceFolder, $"*.{imageType}", SearchOption.AllDirectories);

			if (files.Length.Equals(0))
				return todos;

			files.AsParallel().ForEach(file => GetTodo(todos, settings, imageType, file));

			return todos;
		}
		catch (Exception ex)
		{
			_loggerService.Log(LogException, ex);
			string message = Resources.ServiceException_Message.FormatInvariant(nameof(GetTodos));
			throw new ServiceException(message, ex);
		}
	}

	/// <inheritdoc/>
	/// <exception cref="ServiceException"></exception>
	public TodoCollection GetTodos(ConvertSettingsBase settings, ImageType imageType, string jsonFileContent)
	{
		try
		{
			TodoCollection todos = [];

			TodoCollection todosFromJson = jsonFileContent.FromJson<TodoCollection>();

			todosFromJson.AsParallel().ForEach(tfj => GetTodoFromJson(todos, settings, imageType, tfj));

			return todos;
		}
		catch (Exception ex)
		{
			_loggerService.Log(LogException, ex);
			string message = Resources.ServiceException_Message.FormatInvariant(nameof(GetTodos));
			throw new ServiceException(message, ex);
		}
	}

	/// <inheritdoc/>
	/// <exception cref="ServiceException"></exception>
	public void GetTodosDone(TodoCollection todos, ConvertSettingsBase settings, ImageType imageType, bool jsonExists = false)
	{
		try
		{
			todos.AsParallel().ForEach(t => GetTodoDone(t, settings, imageType));

			if (!jsonExists && settings.ConvertMode.Equals(ConvertModeType.Automatic))
			{
				string jsonContent = todos.ToJson();
				string jsonFilePath = _pathProvider.Combine(settings.TargetFolder, Constants.ResultFileName);
				_fileProvider.WriteAllText(jsonFilePath, jsonContent);
			}

			AnsiConsole.MarkupLine($"[green]Todos done:\t{_todosDone.Count}[/]");
			AnsiConsole.MarkupLine($"[yellow]Duplicates:\t{_todosDuplicateCount}[/]");
		}
		catch (Exception ex)
		{
			_loggerService.Log(LogException, ex);
			string message = Resources.ServiceException_Message.FormatInvariant(nameof(GetTodosDone));
			throw new ServiceException(message, ex);
		}
	}

	private void GetTodo(TodoCollection todos, ConvertSettingsBase settings, ImageType imageType, string file)
	{
		FileInfo fileInfo = new(file);

		IImageModel image = _serviceProvider.GetRequiredKeyedService<IImageModel>(imageType);
		image.Load(file);

		TodoModel todo = new(
			fileName: fileInfo.Name,
			relativePath: $"{fileInfo.DirectoryName?.Replace(settings.SourceFolder, string.Empty)}",
			fullPathName: fileInfo.FullName,
			targetFolder: settings.TargetFolder,
			fileHash: image.Hash
			);

		todos.Enqueue(todo);
	}

	private void GetTodoFromJson(TodoCollection todos, ConvertSettingsBase settings, ImageType imageType, TodoModel todoFromJson)
	{
		string newFullPathName = _pathProvider
			.Combine(settings.TargetFolder, todoFromJson.RelativePath, todoFromJson.FileName.Replace(GetTargetFileExtensions(imageType), $"{imageType}"));

		TodoModel todo = new(
			fileName: $"{todoFromJson.FileHash}.{GetTargetFileExtensions(imageType)}",
			relativePath: todoFromJson.RelativePath,
			fullPathName: newFullPathName,
			targetFolder: settings.TargetFolder,
			fileHash: todoFromJson.FileHash
			);

		todos.Enqueue(todo);
	}

	private void GetTodoDone(TodoModel todo, ConvertSettingsBase settings, ImageType imageType)
	{
		if (settings.ConvertMode.Equals(ConvertModeType.Automatic))
		{
			if (_todosDone.Contains(todo.FileHash))
			{
				AnsiConsole.MarkupLine($"[yellow]'{todo.FullPathName}' is a duplicate![/]");
				_todosDuplicateCount++;
				return;
			}
		}
		else if (settings.ConvertMode.Equals(ConvertModeType.Grouping))
		{
			if (_todosDone.Contains(todo.FileHash))
			{
				AnsiConsole.MarkupLine($"[yellow]'{todo.FullPathName}' is a duplicate![/]");
				_todosDuplicateCount++;
				return;
			}

			CopyImage(settings, todo, imageType);
			_todosDone.Add(todo.FileHash);
			return;
		}

		SaveImage(settings, todo, imageType);

		_todosDone.Add(todo.FileHash);
	}

	private void CopyImage(ConvertSettingsBase settings, TodoModel todo, ImageType imageType)
	{
		IImageModel image = _serviceProvider.GetRequiredKeyedService<IImageModel>(imageType);
		image.Load(todo.FullPathName);

		string targetFolder = PrepareTargetFolder(settings, image, todo);
		DirectoryInfo directoryInfo = _directoryProvider.CreateDirectory(targetFolder);

		if (directoryInfo.Exists)
		{
			string newFileName = GetTargetFileName(settings, todo);
			string newFilePath = _pathProvider.Combine(targetFolder, newFileName);

			_fileProvider.Copy(todo.FullPathName, newFilePath);
		}
	}

	private void SaveImage(ConvertSettingsBase settings, TodoModel todo, ImageType imageType)
	{
		IImageModel image = _serviceProvider.GetRequiredKeyedService<IImageModel>(imageType);
		image.Load(todo.FullPathName);

		string targetFolder = PrepareTargetFolder(settings, image, todo);
		DirectoryInfo directoryInfo = _directoryProvider.CreateDirectory(targetFolder);

		if (directoryInfo.Exists)
		{
			string newFileName = $"{GetTargetFileName(settings, todo)}.{GetTargetFileExtensions(imageType)}";
			string newFilePath = _pathProvider.Combine(targetFolder, newFileName);

			image.Save(newFilePath, settings);
		}
	}

	private string PrepareTargetFolder(ConvertSettingsBase settings, IImageModel image, TodoModel todo)
	{
		string newTargetFolder = todo.TargetFolder;

		if (settings.ConvertMode.Equals(ConvertModeType.Automatic))
		{
			newTargetFolder = _pathProvider.Combine(newTargetFolder, $"{image.Width}");
			return newTargetFolder;
		}
		else if (settings.ConvertMode.Equals(ConvertModeType.Grouping))
		{
			newTargetFolder = _pathProvider.Combine(newTargetFolder, $"{image.Width}x{image.Heigth}");
			return newTargetFolder;
		}

		if (settings.RetainStructure)
			return $"{newTargetFolder}{todo.RelativePath}";

		if (settings.SeparateBySize)
			newTargetFolder = _pathProvider.Combine(newTargetFolder, $"{image.Width}");

		return newTargetFolder;
	}

	private static string GetTargetFileName(ConvertSettingsBase settings, TodoModel todo)
	{
		if (settings.ConvertMode == ConvertModeType.Manual && settings.RetainStructure)
		{
			FileInfo info = new(todo.FullPathName);
			return todo.FileName.Replace(info.Extension, string.Empty);
		}
		else if (settings.ConvertMode.Equals(ConvertModeType.Grouping))
		{
			return todo.FileName;
		}

		return todo.FileHash;
	}

	private static string GetTargetFileExtensions(ImageType imageType) => imageType switch
	{
		ImageType.DDS => $"{ImageType.PNG}",
		_ => $"{ImageType.DDS}"
	};
}
