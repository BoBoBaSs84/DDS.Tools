using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Settings;

using Microsoft.Extensions.DependencyInjection;

namespace DDS.ToolsTests.Services;

[TestClass]
public sealed class TodoServiceTests : UnitTestBase
{
	private static readonly string ResourcePath = Path.Combine(Environment.CurrentDirectory, "Resources");
	private readonly ITodoService _todoService;

	public TodoServiceTests()
		=> _todoService = ServiceProvider.GetRequiredService<ITodoService>();

	[DataTestMethod]
	[DynamicData(nameof(GetResourceData))]
	public void GetTodosTest(string resourcePath, ImageType type, int expected)
	{
		DdsConvertSettings settings = new() { SourceFolder = resourcePath };

		TodoCollection todos = _todoService.GetTodos(settings, type);

		Assert.AreEqual(expected, todos.Count);
	}

	[TestMethod]
	public void GetTodosExceptionTest()
	{
		TodoCollection todos = _todoService.GetTodos(null!, ImageType.DDS);

		Assert.AreEqual(0, todos.Count);
	}

	public static IEnumerable<object[]> GetResourceData
		=> new[]
		{
			new object[] { TestConstants.PngResourcePath, ImageType.PNG, 2 },
			new object[] { TestConstants.PngResourcePath, ImageType.DDS, 0 }
		};
}
