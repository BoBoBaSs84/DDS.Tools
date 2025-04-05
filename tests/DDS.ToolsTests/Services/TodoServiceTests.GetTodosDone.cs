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
	[TestMethod]
	public void GetTodosDoneExceptionTest()
		=> Assert.ThrowsException<ServiceException>(() => s_todoService.GetTodosDone(null!, null!, ImageType.DDS));

	[TestMethod]
	public void GetTodosDoneTest()
	{
		ImageType imageType = ImageType.PNG;
		PngConvertSettings settings = new()
		{
			SourceFolder = TestConstants.PngResourcePath,
			TargetFolder = TestConstants.DdsResultPath
		};

		TodoCollection todos = s_todoService.GetTodos(settings, imageType);

		s_todoService.GetTodosDone(todos, settings, imageType);
	}

	[TestMethod]
	public void GetTodosDoneManualTest()
	{
		ImageType imageType = ImageType.DDS;
		PngConvertSettings settings = new()
		{
			SourceFolder = TestConstants.DdsResourcePath,
			TargetFolder = TestConstants.PngResultPath,
			ConvertMode = ConvertModeType.Manual,
			RetainStructure = true
		};

		TodoCollection todos = s_todoService.GetTodos(settings, imageType);

		s_todoService.GetTodosDone(todos, settings, imageType);
	}

	[TestMethod]
	public void GetTodosDoneGroupingTest()
	{
		ImageType imageType = ImageType.PNG;
		PngConvertSettings settings = new()
		{
			SourceFolder = TestConstants.PngResourcePath,
			TargetFolder = TestConstants.PngResultPath + "Grouped",
			ConvertMode = ConvertModeType.Grouping
		};

		TodoCollection todos = s_todoService.GetTodos(settings, imageType);

		s_todoService.GetTodosDone(todos, settings, imageType);
	}
}
