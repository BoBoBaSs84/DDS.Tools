﻿// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
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
