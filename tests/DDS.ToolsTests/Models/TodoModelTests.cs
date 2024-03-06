using DDS.Tools.Models;

namespace DDS.ToolsTests.Models;

[TestClass]
public sealed class TodoModelTests
{
	[TestMethod]
	public void TodoTest()
	{
		TodoModel todo = new(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);

		Assert.IsNotNull(todo);
		Assert.AreEqual(string.Empty, todo.FileName);
		Assert.AreEqual(string.Empty, todo.FullPathName);
		Assert.AreEqual(string.Empty, todo.RelativePath);
		Assert.AreEqual(string.Empty, todo.TargetPath);
		Assert.AreEqual(string.Empty, todo.Hash);
	}
}
