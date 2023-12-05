using System.Globalization;

using BB84.Extensions.Serialization;

using PNG2DDS.Classes;
using PNG2DDS.Properties;

using Shared.Library.Classes;

namespace PNG2DDS;

internal sealed class Program
{
	private static readonly List<string> TodosDone = [];

	private static void Main(string[] args)
	{
		try
		{
			Parameter param = GetParameter(args);

			IList<Todo> todos = GetTodos(param.SourceFolder);

			Console.WriteLine($"Found {todos.Count} files to process.\nPress key to start.");
			_ = Console.ReadKey();

			GetReverseThigsDone(param.SourceFolder, todos);

			Console.Write($"\n" +
					$"Conversion completed.\n" +
					$"Number of files to convert left: {todos.Count}\n" +
					$"Number of files converted: {TodosDone.Count}\n" +
					$"Press key to exit.");

		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			Environment.Exit(1);
		}
	}

	private static Parameter GetParameter(string[] args)
	{
		if (args.Length < 1)
		{
			Console.WriteLine($"Please provide the 'Result' source path ie.: 'D:\\Data\\Result'");
			Environment.Exit(1);
		}

		if (!Directory.Exists(args[0]))
		{
			string message = $"'{args[0]}' could not be found.";
			Console.WriteLine(message);
			Environment.Exit(1);
		}

		int level = Settings.Default.CompressionLevel;

		return new(args[0], level);
	}

	private static List<Todo> GetTodos(string sourcePath)
	{
		string jsonContent = File.ReadAllText(Path.Combine(sourcePath, Constants.Result.FileName));
		List<Todo>? todos = jsonContent.FromJson<List<Todo>>();
		todos ??= [];
		return todos;
	}

	private static void GetReverseThigsDone(string sourcePath, IList<Todo> todos)
	{
		string[] allFiles = Directory.GetFiles(sourcePath, $"*.{Constants.Extension.DDS}", SearchOption.AllDirectories);

		if (allFiles.Length.Equals(0))
			return;

		DirectoryInfo directoryInfo = new(sourcePath);

		int totalTodoCount = todos.Count;

		foreach (string file in allFiles)
		{
			FileInfo fileInfo = new(file);

			// we can have multiple file results for one hash result!
			IList<Todo> todoList = todos.Where(x => x.MD5String.Equals(fileInfo.Name.Replace(fileInfo.Extension, string.Empty), StringComparison.OrdinalIgnoreCase)).ToList();
			foreach (Todo todo in todoList)
			{
				string targetFolder = $"{directoryInfo.FullName}{todo.RelativePath}";
				_ = Directory.CreateDirectory(targetFolder);

				string targetFullName = Path.Combine($"{directoryInfo.FullName}{todo.RelativePath}", todo.FileName);

				File.Copy(file, targetFullName, true);

				_ = todos.Remove(todo);
				TodosDone.Add(file);

				string progress = (Convert.ToSingle(TodosDone.Count) * 100 / totalTodoCount).ToString("#.##", CultureInfo.InvariantCulture);

				Console.WriteLine($"{progress}%\t{file} -> {targetFullName}");
			}
		}

		return;
	}
}
