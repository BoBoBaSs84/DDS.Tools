using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Services;

using Microsoft.Extensions.DependencyInjection;

namespace DDS.ToolsTests.Services;

[TestClass]
public sealed partial class TodoServiceTests : UnitTestBase
{
	private static ITodoService s_todoService = default!;

	[ClassInitialize]
	public static void ClassInitialize(TestContext context)
		=> s_todoService = ServiceProvider.GetRequiredService<ITodoService>();

	private static IEnumerable<object[]> GetTodosData
		=> new[]
		{
			new object[] { TestConstants.PngResourcePath, ImageType.PNG, 4 },
			[TestConstants.PngResourcePath, ImageType.DDS, 0]
		};
}
