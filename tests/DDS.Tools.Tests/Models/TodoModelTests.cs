// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Models;

namespace DDS.Tools.Tests.Models;

[TestClass]
public sealed class TodoModelTests
{
	[TestMethod]
	public void TodoModelValuesTest()
	{
		TodoModel todo = new("32.dds", "Blue", @"X:\Source\Blue\32.dds", @"X:\Target", "HASH");

		Assert.IsNotNull(todo);
		Assert.AreEqual("32.dds", todo.FileName);
		Assert.AreEqual("Blue", todo.RelativePath);
		Assert.AreEqual(@"X:\Source\Blue\32.dds", todo.FullPathName);
		Assert.AreEqual(@"X:\Target", todo.TargetFolder);
		Assert.AreEqual("HASH", todo.FileHash);
	}
}
