using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Models;
using DDS.Tools.Settings;

namespace DDS.ToolsTests.Services;

public sealed partial class TodoServiceTests
{
	private static readonly string JsonFileContent = "[{\"fileName\": \"32.dds\",\"relativePath\": \"\",\"fileHash\": \"3B6242065CC51D558E45CD5868A18A36\"}]";

	[TestMethod]
	public void GetTodosFromJsonTest()
	{
		DdsConvertSettings settings = new() { SourceFolder = @"X:\FooBar" };

		TodoCollection todos = s_todoService.GetTodos(settings, ImageType.DDS, JsonFileContent);

		Assert.AreEqual(1, todos.Count);
		Assert.AreEqual("3B6242065CC51D558E45CD5868A18A36.PNG", todos.First().FileName);
		Assert.AreEqual("32.dds", todos.First().FullPathName);
	}

	[TestMethod]
	[ExpectedException(typeof(ServiceException))]
	public void GetTodosFromJsonExceptionTest()
	{
		DdsConvertSettings settings = new() { SourceFolder = @"X:\FooBar" };

		_ = s_todoService.GetTodos(settings, ImageType.PNG, string.Empty);
	}
}
