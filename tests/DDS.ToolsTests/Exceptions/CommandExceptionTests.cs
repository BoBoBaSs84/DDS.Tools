using DDS.Tools.Exceptions;

namespace DDS.ToolsTests.Exceptions;

[TestClass]
public sealed class CommandExceptionTests : UnitTestBase
{
	[TestMethod]
	public void CommandExceptionTest()
	{
		CommandException exception = new("Test");

		Assert.IsNotNull(exception);
		Assert.AreEqual("Test", exception.Message);
	}
}
