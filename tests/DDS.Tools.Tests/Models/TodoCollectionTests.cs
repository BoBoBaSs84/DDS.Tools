// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------

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
public sealed class TodoCollectionTests
{
	[TestMethod]
	public void TodoCollectionQueueBehaviorTest()
	{
		TodoCollection todos = [];
		TodoModel first = new("first.dds", "A", @"X:\A\first.dds", @"X:\Target", "HASH-1");
		TodoModel second = new("second.dds", "B", @"X:\B\second.dds", @"X:\Target", "HASH-2");

		todos.Enqueue(first);
		todos.Enqueue(second);

		Assert.AreEqual(2, todos.Count);
		Assert.IsTrue(todos.TryDequeue(out TodoModel? dequeuedFirst));
		Assert.IsTrue(todos.TryDequeue(out TodoModel? dequeuedSecond));
		Assert.AreSame(first, dequeuedFirst);
		Assert.AreSame(second, dequeuedSecond);
		Assert.AreEqual(0, todos.Count);
	}
}
