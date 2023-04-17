using Shared.Library.Classes;
using DIR = Shared.Library.Classes.Constants.Directories;
using EXT = Shared.Library.Classes.Constants.Extensions;

namespace DDS2PNG;

internal sealed class Program
{
	private static void Main(string[] args)
	{
		(string directory, int level) = CheckArguments(args);

		IList<Todo> todos = GetTodos(directory);

		IList<string> todosDone = GetThigsDone(directory, level, todos);

		Console.Write($"\n" +
			$"Conversion completed.\n" +
			$"Number of files to convert: {todos.Count}\n" +
			$"Number of files converted: {todosDone.Count}\n" +
			$"Press key to exit.");

		_ = Console.ReadKey();
	}

	private static (string directory, int level) CheckArguments(string[] args)
	{
		if (args.Length < 2)
		{
			string message = $"First argument should be the path ie.: 'C:\\Temp'\n" +
				$"Second argument should be the compression level as integer.";

			Console.WriteLine(message);
			Environment.Exit(1);
		}

		if (!int.TryParse(args[1], out _))
		{
			string message = $"Second argument is not an integer. ({args[1]})";
			Console.WriteLine(message);
			Environment.Exit(1);
		}

		int level = int.Parse(args[1], NumberStyles.Integer, CultureInfo.InvariantCulture);

		if (!Directory.Exists(args[0]))
		{
			string message = $"'{args[0]}' could not be found.";
			Console.WriteLine(message);
			Environment.Exit(1);
		}

		string directory = args[0];

		return (directory, level);
	}

	private static IList<Todo> GetTodos(string directory)
	{
		string[] allFiles = Directory.GetFiles(directory, $"*.{EXT.DDS}", SearchOption.AllDirectories);

		IList<Todo> todos = new List<Todo>();

		foreach (string file in allFiles)
		{
			FileInfo fileInfo = new(file);
			DDSImage image = new(fileInfo.FullName);
			Todo todo = new(fileInfo.Name, fileInfo.DirectoryName!, Helper.GetMD5String(image.ImageData), image);
			todos.Add(todo);
		}

		return todos;
	}

	private static IList<string> GetThigsDone(string directory, int level, IList<Todo> todos)
	{
		IList<string> todosDone = new List<string>();

		DirectoryInfo dirInfo = new(directory);
		string convertedPath = Path.Combine(dirInfo.Parent!.FullName, DIR.Converted);
		_ = Directory.CreateDirectory(convertedPath);

		foreach (Todo todo in todos)
		{
			if (todosDone.Contains(todo.MD5String))
				continue;

			string newFilePath = Path.Combine(convertedPath, $"{todo.MD5String}.{EXT.PNG}");
			todo.Image.Save(newFilePath, level);
			todosDone.Add(todo.MD5String);

			Console.WriteLine($"[{DateTime.Now}]\t{todo.FullName}|{newFilePath}");
		}

		string result = Helper.JsonResult(todos);
		string resultPath = Path.Combine(convertedPath, "result.json");
		File.WriteAllText(resultPath, result);

		return todosDone;
	}
}