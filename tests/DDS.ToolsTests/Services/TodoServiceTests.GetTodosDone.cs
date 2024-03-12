﻿using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Models;
using DDS.Tools.Settings;

namespace DDS.ToolsTests.Services;

public sealed partial class TodoServiceTests
{
	[TestMethod]
	[ExpectedException(typeof(ServiceException))]
	public void GetTodosDoneExceptionTest()
		=> s_todoService.GetTodosDone(null!, null!, ImageType.DDS);

	[TestMethod]
	public void GetTodosDoneTest()
	{
		ImageType imageType = ImageType.PNG;
		PngConvertSettings settings = new()
		{
			SourceFolder = TestConstants.PngResourcePath,
			TargetFolder = TestConstants.PngResultPath
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
			TargetFolder = TestConstants.DdsResultPath,
			ConvertMode = ConvertModeType.Manual,
			RetainStructure = true
		};

		TodoCollection todos = s_todoService.GetTodos(settings, imageType);

		s_todoService.GetTodosDone(todos, settings, imageType);
	}
}
