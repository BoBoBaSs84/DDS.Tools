using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Models;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Settings.Base;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Spectre.Console;

namespace DDS.Tools.Services;

/// <summary>
/// The todo service class.
/// </summary>
/// <param name="logger">The logger service instance to use.</param>
/// <param name="provider">The service provider instance to use.</param>
internal sealed class TodoService(ILoggerService<TodoService> logger, IServiceProvider provider) : ITodoService
{
	private readonly ILoggerService<TodoService> _logger = logger;
	private readonly IServiceProvider _provider = provider;
	private const string ResultFolder = "Result";
	private readonly List<string> _todosDone = [];
	private int _todosDuplicateCount = 0;

	private static readonly Action<ILogger, Exception?> LogException =
		LoggerMessage.Define(LogLevel.Error, 0, "Exception occured.");

	/// <inheritdoc/>
	public TodoCollection GetTodos(ConvertSettings settings, ImageType imageType)
	{
		TodoCollection todos = [];

		try
		{
			string searchPattern = $"*.{imageType}";
			string[] files = Directory.GetFiles(settings.SourceFolder, searchPattern, SearchOption.AllDirectories);

			if (files.Length.Equals(0))
				return todos;

			DirectoryInfo directoryInfo = new(settings.SourceFolder);
			string targetPath = Path.Combine(directoryInfo.Parent!.FullName, ResultFolder);

			foreach (string file in files)
			{
				IImageModel image = _provider.GetRequiredKeyedService<IImageModel>(imageType);

				image.Load(file);

				todos.Add(new(image.Name, settings.TargetFolder, file, targetPath, image.Hash));
			}

			return todos;
		}
		catch (Exception ex)
		{
			_logger.Log(LogException, ex);
			AnsiConsole.MarkupLine($"[maroon]{ex.Message}[/]");
			return todos;
		}
	}

	public void GetTodosDone(TodoCollection todos, ConvertSettings settings, ImageType imageType)
	{
		try
		{
			foreach (var todo in todos)
			{
				if (_todosDone.Contains(todo.Hash))
				{
					AnsiConsole.MarkupLine($"[yellow]{todo.FileName} is a duplicate[/]");
					_todosDuplicateCount++;
					continue;
				}

				SaveImage(settings, todo, imageType);
				
				_todosDone.Add(todo.Hash);
			}
		}
		catch (Exception ex)
		{
			_logger.Log(LogException, ex);
			AnsiConsole.Markup($"[maroon]{ex.Message}[/]");
		}
	}

	private void SaveImage(ConvertSettings settings, TodoModel todo, ImageType imageType)
	{
		IImageModel image = _provider.GetRequiredKeyedService<IImageModel>(imageType);
		image.Load(todo.FullPathName);

		string targetFolder = PrepareTargetFolder(settings, image, todo.TargetPath);
		Directory.CreateDirectory(targetFolder);

		string newFileName = $"{GetTargetFileName(settings, todo)}.{GetTargetFileExtensions(imageType)}";
		string newFilePath = Path.Combine(targetFolder, newFileName);

		image.Save(newFilePath);
	}

	private static string PrepareTargetFolder(ConvertSettings settings, IImageModel image, string targetFolder)
	{
		string newTargetFolder = targetFolder;

		if (settings.ConvertMode.Equals(ConvertModeType.Automatic))
		{
			newTargetFolder = Path.Combine(newTargetFolder, $"{image.Width}");
			return newTargetFolder;
		}

		if (settings.RetainStructure)
			return newTargetFolder;

		if (settings.SeparateBySize)
			newTargetFolder = Path.Combine(newTargetFolder, $"{image.Width}");

		return newTargetFolder;
	}

	private static string GetTargetFileName(ConvertSettings settings, TodoModel todo)
	{
		if (settings.ConvertMode == ConvertModeType.Manual && settings.RetainStructure)
		{
			FileInfo info = new(todo.FullPathName);
			return todo.FileName.Replace(info.Extension, string.Empty);
		}

		return todo.Hash;
	}

	private static string GetTargetFileExtensions(ImageType imageType)
		=> imageType switch
		{
			ImageType.DDS => $"{ImageType.PNG}",
			ImageType.PNG => $"{ImageType.DDS}",
			_ => string.Empty,
		};
}
