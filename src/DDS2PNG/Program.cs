using System.Globalization;

using DDS2PNG.Classes;
using DDS2PNG.Properties;

using Shared.Library.Classes;
using Shared.Library.Factories;
using Shared.Library.Interfaces;

namespace DDS2PNG;

internal sealed class Program
{
	private static readonly IList<string> TodosDone = new List<string>();
	private static readonly IList<Todo> Todos = new List<Todo>();
	private static int s_totalTodoCount;
	private static int s_totalTodoDuplicateCount;

	private static void Main(string[] args)
	{
		try
		{
			Parameter parameter = GetParameter(args);

			GetTodos(parameter);

			Console.WriteLine($"Found {Todos.Count} files to process.\nPress key to start.");
			_ = Console.ReadKey();

			GetThigsDone(parameter);

			Console.Write($"\n" +
					$"Conversion completed.\n" +
					$"Number of files to convert: {Todos.Count}\n" +
					$"Number of files converted: {TodosDone.Count}\n" +
					$"Number of duplicates: {s_totalTodoDuplicateCount}\n" +
					$"Press key to exit.");

			_ = Console.ReadKey();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			Environment.Exit(1);
		}
	}

	private static Parameter GetParameter(string[] args)
	{
		if (args.Length < 2)
		{
			Console.WriteLine($"Please provide the texture source path ie.: 'D:\\Data\\Textures'\n" +
					$"Please provide a search pattern for the files ie.: '*?n.dds'\n");
			Environment.Exit(1);
		}

		if (!Directory.Exists(args[0]))
		{
			string message = $"'{args[0]}' could not be found.";
			Console.WriteLine(message);
			Environment.Exit(1);
		}

		int compressionLevel = Settings.Default.CompressionLevel;
		bool separateMaps = Settings.Default.SeparateMaps;
		bool ignoreMaps = Settings.Default.IgnoreMaps;
		bool separateBySize = Settings.Default.SeparateBySize;

		return new(args[0], args[1], compressionLevel, separateMaps, ignoreMaps, separateBySize);
	}

	private static void GetTodos(Parameter parameter)
	{
		string[] allFiles = Directory.GetFiles(parameter.SourceFolder, parameter.SearchPattern, SearchOption.AllDirectories);

		if (!allFiles.Any())
			return;

		DirectoryInfo directoryInfo = new(parameter.SourceFolder);
		string targetPath = Path.Combine(directoryInfo.Parent!.FullName, Constants.Result.Folder);

		foreach (string file in allFiles)
		{
			if (IgnoreFile(parameter, file))
				continue;

			try
			{
				FileInfo fileInfo = new(file);
				IImage image = ImageFactory.CreateDdsImage(file);

				string relativePath = $"{fileInfo.DirectoryName!.Replace(directoryInfo.Parent!.FullName, string.Empty)}";

				Todo todo = new(image.FileName, relativePath, file, targetPath, image.Md5Hash);
				Todos.Add(todo);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error: {file} -> {ex.Message}");
			}
		}

		s_totalTodoCount = Todos.Count;

		return;
	}

	private static void GetThigsDone(Parameter parameter)
	{
		if (parameter.SeparateMaps)
		{
			foreach (Todo todo in Todos.Where(x => x.FileName.EndsWith($"_n.{Constants.Extension.DDS}", StringComparison.CurrentCultureIgnoreCase)))
			{
				if (TodosDone.Contains(todo.MD5String))
				{
					s_totalTodoDuplicateCount++;
					continue;
				}

				string targetFolder = Path.Combine(todo.TargetPath, "normalMaps");
				SaveImage(parameter, todo, targetFolder);
			}
		}

		foreach (Todo todo in Todos)
		{
			if (TodosDone.Contains(todo.MD5String))
				continue;

			string targetFolder = Path.Combine(todo.TargetPath, "textureMaps");
			SaveImage(parameter, todo, targetFolder);
		}

		string result = Helper.GetJsonResultFromList(Todos);
		string resultPath = Path.Combine(Todos.First().TargetPath, Constants.Result.FileName);
		File.WriteAllText(resultPath, result);

		return;
	}

	private static void SaveImage(Parameter parameter, Todo todo, string targetFolder)
	{
		IImage image = ImageFactory.CreateDdsImage(todo.FullPathName);

		if (parameter.SeparateBySize)
			targetFolder = Path.Combine(targetFolder, $"{image.Width}");
		_ = Directory.CreateDirectory(targetFolder);

		string filePath = Path.Combine(targetFolder, $"{todo.MD5String}.{Constants.Extension.PNG}");
		image.Save(filePath, parameter.CompressionLevel);

		TodosDone.Add(todo.MD5String);

		string progress = (Convert.ToSingle(TodosDone.Count + s_totalTodoDuplicateCount) * 100 / s_totalTodoCount).ToString("#.##", CultureInfo.InvariantCulture);
		Console.WriteLine($"{progress}%\t{Path.Combine(todo.RelativePath, todo.FileName)} -> {filePath}");
	}

	private static bool IgnoreFile(Parameter parameter, string filePath)
	{
		if (parameter.IgnoreMaps)
			if (filePath.EndsWith("_n.dds", StringComparison.InvariantCultureIgnoreCase))
				return true;
		return false;
	}
}
