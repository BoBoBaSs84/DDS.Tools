// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Interfaces.Models;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Services;
using DDS.Tools.Settings;
using DDS.Tools.Settings.Base;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

namespace DDS.Tools.Tests.Services;

[TestClass]
public sealed class TodoServiceTests
{
	private readonly Mock<IDirectoryProvider> _directoryProviderMock = new();
	private readonly Mock<IFileProvider> _fileProviderMock = new();
	private readonly Mock<IImageModel> _imageModelMock = new();
	private readonly Mock<ILoggerService<TodoService>> _loggerServiceMock = new();
	private readonly Mock<IPathProvider> _pathProviderMock = new();

	[TestMethod]
	public void GetTodosNoFilesFoundReturnsEmptyCollection()
	{
		ConfigureCommonMocks();
		PngConvertSettings settings = CreateSettings(ConvertModeType.Automatic);

		_directoryProviderMock
			.Setup(x => x.GetFiles(settings.SourceFolder, "*.DDS", SearchOption.AllDirectories))
			.Returns([]);

		TodoService sut = CreateSut();

		TodoCollection result = sut.GetTodos(settings, ImageType.DDS);

		Assert.IsEmpty(result);
		_imageModelMock.Verify(x => x.Load(It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void GetTodosFilesFoundReturnsTodos()
	{
		ConfigureCommonMocks();
		PngConvertSettings settings = CreateSettings(ConvertModeType.Automatic);

		string[] files =
		[
			Path.Combine(settings.SourceFolder, "Blue", "64.DDS"),
			Path.Combine(settings.SourceFolder, "Red", "64A.DDS")
		];

		_directoryProviderMock
			.Setup(x => x.GetFiles(settings.SourceFolder, "*.DDS", SearchOption.AllDirectories))
			.Returns(files);

		_imageModelMock.SetupGet(x => x.Hash).Returns("HASH_01");

		TodoService sut = CreateSut();

		TodoCollection result = sut.GetTodos(settings, ImageType.DDS);

		Assert.HasCount(2, result);
		_imageModelMock.Verify(x => x.Load(It.IsAny<string>()), Times.Exactly(2));
	}

	[TestMethod]
	public void GetTodosFromJsonReturnsMappedTodos()
	{
		ConfigureCommonMocks();
		PngConvertSettings settings = CreateSettings(ConvertModeType.Automatic);

		string jsonContent = """
			[
				{
					"fileName":"image.PNG",
					"relativePath":"sub",
					"fileHash":"ABC123"
				}
			]
			""";

		TodoService sut = CreateSut();

		TodoCollection result = sut.GetTodos(settings, ImageType.DDS, jsonContent);

		Assert.HasCount(1, result);

		TodoModel todo = result.Single();
		Assert.AreEqual("ABC123.PNG", todo.FileName);
		Assert.AreEqual("sub", todo.RelativePath);
		Assert.AreEqual(Path.Combine(settings.TargetFolder, "sub", "image.DDS"), todo.FullPathName);
	}

	[TestMethod]
	public void GetTodosFromJsonWhenJsonIsInvalidThrowsServiceExceptionAndLogs()
	{
		ConfigureCommonMocks();
		PngConvertSettings settings = CreateSettings(ConvertModeType.Automatic);
		TodoService sut = CreateSut();

		Assert.Throws<ServiceException>(() => sut.GetTodos(settings, ImageType.DDS, "{"));

		_loggerServiceMock.Verify(
			x => x.Log(It.IsAny<Action<ILogger, Exception?>>(), It.IsAny<Exception?>()),
			Times.Once);
	}

	[TestMethod]
	public void GetTodosDoneAutomaticWithDuplicatesSavesOnceAndWritesJson()
	{
		ConfigureCommonMocks();
		PngConvertSettings settings = CreateSettings(ConvertModeType.Automatic);

		_imageModelMock.SetupGet(x => x.Width).Returns(64);
		_imageModelMock.SetupGet(x => x.Heigth).Returns(32);

		TodoCollection todos = new();
		todos.Enqueue(new TodoModel("a.DDS", string.Empty, Path.Combine(settings.SourceFolder, "a.DDS"), settings.TargetFolder, "DUP_HASH"));
		todos.Enqueue(new TodoModel("b.DDS", string.Empty, Path.Combine(settings.SourceFolder, "b.DDS"), settings.TargetFolder, "DUP_HASH"));

		TodoService sut = CreateSut();

		sut.GetTodosDone(todos, settings, ImageType.DDS);

		string expectedSavePath = Path.Combine(settings.TargetFolder, "64", "DUP_HASH.PNG");
		string expectedJsonPath = Path.Combine(settings.TargetFolder, "Result.json");

		_imageModelMock.Verify(x => x.Save(expectedSavePath, settings), Times.Once);
		_fileProviderMock.Verify(x => x.WriteAllText(expectedJsonPath, It.IsAny<string>()), Times.Once);
	}

	[TestMethod]
	public void GetTodosDoneGroupingWithDuplicatesCopiesOnce()
	{
		ConfigureCommonMocks();
		PngConvertSettings settings = CreateSettings(ConvertModeType.Grouping);

		_imageModelMock.SetupGet(x => x.Width).Returns(128);
		_imageModelMock.SetupGet(x => x.Heigth).Returns(64);

		TodoCollection todos = new();
		todos.Enqueue(new TodoModel("x.DDS", string.Empty, Path.Combine(settings.SourceFolder, "x.DDS"), settings.TargetFolder, "DUP_HASH"));
		todos.Enqueue(new TodoModel("y.DDS", string.Empty, Path.Combine(settings.SourceFolder, "y.DDS"), settings.TargetFolder, "DUP_HASH"));

		TodoService sut = CreateSut();

		sut.GetTodosDone(todos, settings, ImageType.DDS, jsonExists: true);

		string expectedCopyPath = Path.Combine(settings.TargetFolder, "128x64", "x.DDS");

		_fileProviderMock.Verify(
			x => x.Copy(Path.Combine(settings.SourceFolder, "x.DDS"), expectedCopyPath),
			Times.Once);

		_imageModelMock.Verify(x => x.Save(It.IsAny<string>(), It.IsAny<ConvertSettingsBase>()), Times.Never);
		_fileProviderMock.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void GetTodosDoneWhenImageLoadThrowsThrowsServiceExceptionAndLogs()
	{
		ConfigureCommonMocks();
		PngConvertSettings settings = CreateSettings(ConvertModeType.Automatic);

		TodoCollection todos = new();
		todos.Enqueue(new TodoModel("a.DDS", string.Empty, Path.Combine(settings.SourceFolder, "a.DDS"), settings.TargetFolder, "HASH_A"));

		_imageModelMock
			.Setup(x => x.Load(It.IsAny<string>()))
			.Throws(new InvalidOperationException("boom"));

		TodoService sut = CreateSut();

		Assert.Throws<ServiceException>(() => sut.GetTodosDone(todos, settings, ImageType.DDS));

		_loggerServiceMock.Verify(
			x => x.Log(It.IsAny<Action<ILogger, Exception?>>(), It.IsAny<Exception?>()),
			Times.Once);
	}

	[TestMethod]
	public void GetTodosWhenProviderThrowsThrowsServiceExceptionAndLogs()
	{
		ConfigureCommonMocks();
		PngConvertSettings settings = CreateSettings(ConvertModeType.Automatic);

		_directoryProviderMock
			.Setup(x => x.GetFiles(settings.SourceFolder, "*.DDS", SearchOption.AllDirectories))
			.Throws(new InvalidOperationException("boom"));

		TodoService sut = CreateSut();

		Assert.Throws<ServiceException>(() => sut.GetTodos(settings, ImageType.DDS));

		_loggerServiceMock.Verify(
			x => x.Log(It.IsAny<Action<ILogger, Exception?>>(), It.IsAny<Exception?>()),
			Times.Once);
	}

	private TodoService CreateSut()
	{
		ServiceCollection services = new();

		services.AddSingleton(_directoryProviderMock.Object);
		services.AddSingleton(_fileProviderMock.Object);
		services.AddSingleton(_pathProviderMock.Object);
		services.AddKeyedSingleton<IImageModel>(ImageType.DDS, _imageModelMock.Object);
		services.AddKeyedSingleton<IImageModel>(ImageType.PNG, _imageModelMock.Object);

		ServiceProvider provider = services.BuildServiceProvider();

		return new TodoService(_loggerServiceMock.Object, provider);
	}

	private void ConfigureCommonMocks()
	{
		_pathProviderMock
			.Setup(x => x.Combine(It.IsAny<string>(), It.IsAny<string>()))
			.Returns((string a, string b) => Path.Combine(a, b));

		_pathProviderMock
			.Setup(x => x.Combine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
			.Returns((string a, string b, string c) => Path.Combine(a, b, c));

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
