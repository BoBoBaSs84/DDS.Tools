// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Models;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Models;
using DDS.Tools.Services;
using DDS.Tools.Settings;

using Moq;

namespace DDS.Tools.Tests.Services;

[TestClass]
public sealed class TodoTransformationServiceTests
{
	private readonly Mock<IDirectoryProvider> _directoryProviderMock = new();
	private readonly Mock<IFileProvider> _fileProviderMock = new();
	private readonly Mock<IImageModel> _imageModelMock = new();
	private readonly Mock<IPathProvider> _pathProviderMock = new();

	[TestMethod]
	public void GetTodosDoneManualCountsEveryTransferredTodo()
	{
		ConfigureCommonMocks();
		PngConvertSettings settings = CreateSettings(ConvertModeType.Manual);

		TodoTransformationService sut = CreateSut();

		TodoProcessingResult result = sut.GetTodosDone(CreateDuplicateTodos(settings), settings, ImageType.DDS);

		// Manual does not deduplicate, so both todos are transferred and both are counted.
		Assert.AreEqual(2, result.TodosDoneCount);
		Assert.AreEqual(0, result.DuplicatesCount);
	}

	[DataTestMethod]
	[DataRow(ConvertModeType.Automatic)]
	[DataRow(ConvertModeType.Grouping)]
	public void GetTodosDoneCountsDuplicatesSeparately(ConvertModeType convertMode)
	{
		ConfigureCommonMocks();
		PngConvertSettings settings = CreateSettings(convertMode);

		TodoTransformationService sut = CreateSut();

		TodoProcessingResult result = sut.GetTodosDone(CreateDuplicateTodos(settings), settings, ImageType.DDS);

		Assert.AreEqual(1, result.TodosDoneCount);
		Assert.AreEqual(1, result.DuplicatesCount);
	}

	[TestMethod]
	public void GetTodosDoneCountsAccountForEveryTodo()
	{
		ConfigureCommonMocks();
		PngConvertSettings settings = CreateSettings(ConvertModeType.Automatic);
		TodoCollection todos = CreateDuplicateTodos(settings);
		int todoCount = todos.Count;

		TodoTransformationService sut = CreateSut();

		TodoProcessingResult result = sut.GetTodosDone(todos, settings, ImageType.DDS);

		Assert.AreEqual(todoCount, result.TodosDoneCount + result.DuplicatesCount);
	}

	private static TodoCollection CreateDuplicateTodos(PngConvertSettings settings)
	{
		TodoCollection todos = new();
		todos.Enqueue(new TodoModel("a.DDS", string.Empty, Path.Combine(settings.SourceFolder, "a.DDS"), settings.TargetFolder, "DUP_HASH"));
		todos.Enqueue(new TodoModel("b.DDS", string.Empty, Path.Combine(settings.SourceFolder, "b.DDS"), settings.TargetFolder, "DUP_HASH"));

		return todos;
	}

	private TodoTransformationService CreateSut()
		=> new(
			_directoryProviderMock.Object,
			_fileProviderMock.Object,
			_pathProviderMock.Object,
			_ => _imageModelMock.Object);

	private void ConfigureCommonMocks()
	{
		_pathProviderMock
			.Setup(x => x.Combine(It.IsAny<string>(), It.IsAny<string>()))
			.Returns((string a, string b) => Path.Combine(a, b));

		_directoryProviderMock
			.Setup(x => x.CreateDirectory(It.IsAny<string>()))
			.Returns((string path) => Directory.CreateDirectory(path));
	}

	private static PngConvertSettings CreateSettings(ConvertModeType convertMode)
	{
		string rootPath = Path.Combine(Path.GetTempPath(), $"dds-tools-tests-{Guid.NewGuid():N}");

		return new PngConvertSettings
		{
			SourceFolder = Path.Combine(rootPath, "source"),
			TargetFolder = Path.Combine(rootPath, "target"),
			ConvertMode = convertMode
		};
	}
}
