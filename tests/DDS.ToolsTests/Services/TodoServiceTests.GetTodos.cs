using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Models;
using DDS.Tools.Settings;

namespace DDS.ToolsTests.Services;

public sealed partial class TodoServiceTests
{
	[DataTestMethod]
	[DynamicData(nameof(GetTodosData), DynamicDataSourceType.Property)]
	public void GetTodosTest(string resourcePath, ImageType type, int expected)
	{
		DdsConvertSettings settings = new() { SourceFolder = resourcePath, TargetFolder = resourcePath };

		TodoCollection todos = s_todoService.GetTodos(settings, type);

		Assert.AreEqual(expected, todos.Count);
	}

	[TestMethod]
	public void GetTodosSourceFolderNotFoundTest()
		=> Assert.ThrowsException<ServiceException>(() => s_todoService.GetTodos(new DdsConvertSettings() { SourceFolder = @"X:\FooBar" }, ImageType.DDS));
}
