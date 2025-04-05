// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
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
		Assert.AreEqual(string.Empty, todo.TargetFolder);
		Assert.AreEqual(string.Empty, todo.FileHash);
	}
}
