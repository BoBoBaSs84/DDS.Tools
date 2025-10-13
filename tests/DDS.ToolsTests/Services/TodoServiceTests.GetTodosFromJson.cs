// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Models;
using DDS.Tools.Settings;

namespace DDS.ToolsTests.Services;

public sealed partial class TodoServiceTests
{
	private static readonly string JsonFileContent = "[{\"fileName\": \"32.dds\",\"relativePath\": \"\",\"fileHash\": \"3B6242065CC51D558E45CD5868A18A36\"}]";

	[TestMethod]
	public void GetTodosFromJsonTest()
	{
		DdsConvertSettings settings = new() { SourceFolder = @"X:\FooBar" };

		TodoCollection todos = s_todoService.GetTodos(settings, ImageType.DDS, JsonFileContent);

		Assert.HasCount(1, todos);
		Assert.AreEqual("3B6242065CC51D558E45CD5868A18A36.PNG", todos.First().FileName);
		Assert.AreEqual("32.dds", todos.First().FullPathName);
	}

	[TestMethod]
	public void GetTodosFromJsonExceptionTest()
	{
		DdsConvertSettings settings = new() { SourceFolder = @"X:\FooBar" };

		Assert.Throws<ServiceException>(() => s_todoService.GetTodos(settings, ImageType.PNG, string.Empty));
	}
}
