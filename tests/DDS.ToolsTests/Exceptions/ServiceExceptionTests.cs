using DDS.Tools.Exceptions;

namespace DDS.ToolsTests.Exceptions;

[TestClass]
public sealed class ServiceExceptionTests : UnitTestBase
{
	[TestMethod]
	public void ServiceExceptionTest()
	{
		ServiceException exception = new("Test");

		Assert.IsNotNull(exception);
		Assert.AreEqual("Test", exception.Message);
	}
}
