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

	private static readonly Action<ILogger, Exception?> LogException =
		LoggerMessage.Define(LogLevel.Error, 0, "Exception occured.");

	/// <inheritdoc/>
	public TodoCollection GetTodos(ConvertSettings settings, ImageType imageType)
	{
		TodoCollection todos = [];
		string searchPattern = $"*.{imageType}";
		string[] files = Directory.GetFiles(settings.SourceFolder, searchPattern, SearchOption.AllDirectories);

		if (files.Length.Equals(0))
			return todos;

		try
		{
			DirectoryInfo directoryInfo = new(settings.SourceFolder);
			string targetPath = Path.Combine(directoryInfo.Parent!.FullName, ResultFolder);

			foreach (string file in files)
			{
				if (IgnoreFile(settings, file))
					continue;

				IImageModel image = _provider.GetRequiredKeyedService<IImageModel>(imageType);
				image.Load(file);

				FileInfo fileInfo = new(file);
				string relativePath = $"{fileInfo.DirectoryName!.Replace(directoryInfo.Parent!.FullName, string.Empty)}";

				todos.Add(new(image.Name, relativePath, file, targetPath, image.Hash));
			}

			return todos;
		}
		catch (Exception ex)
		{
			_logger.Log(LogException, ex);
			AnsiConsole.Markup($"[maroon]{ex.Message}[/]");
			return todos;
		}
	}

	private static bool IgnoreFile(ConvertSettings settings, string file)
	{
		if (settings.IgnoreMaps)
		{
			if (file.Contains("_d."))
				return true;
			if (file.Contains("_n."))
				return true;
		}
		return false;
	}
}
