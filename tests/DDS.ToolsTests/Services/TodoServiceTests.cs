using DDS.Tools.Interfaces.Services;

using Microsoft.Extensions.DependencyInjection;

namespace DDS.ToolsTests.Services;

[TestClass]
public sealed class TodoServiceTests : UnitTestBase
{
	private readonly ITodoService _todoService;

	public TodoServiceTests()
		=> _todoService = ServiceProvider.GetRequiredService<ITodoService>();

	[TestMethod]
	public void GetTodosTest()
	{

	}
}
