// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Exceptions;
using DDS.Tools.Tests;

namespace DDS.Tools.Tests.Exceptions;

[TestClass]
public sealed class ServiceExceptionTests : UnitTestBase
{
	[TestMethod]
	public void ServiceExceptionMessageOnlyTest()
	{
		ServiceException exception = new("Test");

		Assert.IsNotNull(exception);
		Assert.AreEqual("Test", exception.Message);
		Assert.IsNull(exception.InnerException);
	}

	[TestMethod]
	public void ServiceExceptionWithInnerExceptionTest()
	{
		Exception innerException = new InvalidOperationException("Inner");

		ServiceException exception = new("Test", innerException);

		Assert.IsNotNull(exception);
		Assert.AreEqual("Test", exception.Message);
		Assert.AreSame(innerException, exception.InnerException);
	}
}
