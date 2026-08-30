// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Imaging;
using DDS.Tools.Interfaces.Imaging;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Services;
using DDS.Tools.Settings;

using Microsoft.Extensions.Logging;

using Moq;

namespace DDS.Tools.Tests.Services;

[TestClass]
public sealed class TodoServiceTests
{
	private readonly Mock<IDirectoryProvider> _directoryProviderMock = new();
	private readonly Mock<IFileProvider> _fileProviderMock = new();
	private readonly Mock<ILoggerService<TodoService>> _loggerServiceMock = new();
	private readonly Mock<IPathProvider> _pathProviderMock = new();
	private readonly Mock<IImageCodecRegistry> _codecRegistryMock = new();
	private readonly Mock<IImageDecoder> _decoderMock = new();
	private readonly Mock<IImageEncoder> _encoderMock = new();

	[TestMethod]
	public void GetTodosNoFilesFoundReturnsEmptyCollection()
	{
		ConfigureCommonMocks();
		ConvertSettings settings = CreateSettings(ConvertModeType.Automatic);

		_directoryProviderMock
			.Setup(x => x.GetFiles(settings.SourceFolder, "*.dds", SearchOption.AllDirectories))
			.Returns([]);

		TodoService sut = CreateSut();

		TodoCollection result = sut.GetTodos(settings);

		Assert.IsEmpty(result);
		_fileProviderMock.Verify(x => x.ReadAllBytes(It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void GetTodosFilesFoundReturnsTodos()
	{
		ConfigureCommonMocks();
		ConvertSettings settings = CreateSettings(ConvertModeType.Automatic);

		string[] files =
		[
			Path.Combine(settings.SourceFolder, "Blue", "64.dds"),
			Path.Combine(settings.SourceFolder, "Red", "64A.dds")
		];

		_directoryProviderMock
			.Setup(x => x.GetFiles(settings.SourceFolder, "*.dds", SearchOption.AllDirectories))
			.Returns(files);

		TodoService sut = CreateSut();

		TodoCollection result = sut.GetTodos(settings);

		Assert.HasCount(2, result);
		_fileProviderMock.Verify(x => x.ReadAllBytes(It.IsAny<string>()), Times.Exactly(2));
	}

	[TestMethod]
	public void GetTodosWithoutFromDiscoversEveryFormatAndTagsEachTodo()
	{
		ConfigureCommonMocks();
		ConvertSettings settings = CreateSettings(ConvertModeType.Manual);
		settings.From = null;
		settings.RetainStructure = true;

		_directoryProviderMock
			.Setup(x => x.GetFiles(settings.SourceFolder, "*.tga", SearchOption.AllDirectories))
			.Returns([Path.Combine(settings.SourceFolder, "32.tga")]);
		_directoryProviderMock
			.Setup(x => x.GetFiles(settings.SourceFolder, "*.jpg", SearchOption.AllDirectories))
			.Returns([Path.Combine(settings.SourceFolder, "32.jpg")]);

		TodoService sut = CreateSut();

		TodoCollection result = sut.GetTodos(settings);

		Assert.HasCount(2, result);
		Assert.IsTrue(result.Any(todo => todo.SourceType == ImageType.TGA && todo.OriginalName == "32.tga"));
		Assert.IsTrue(result.Any(todo => todo.SourceType == ImageType.JPG && todo.OriginalName == "32.jpg"));
	}

	[TestMethod]
	[DataRow("relative", DisplayName = "Relative source folder")]
	[DataRow("trailing", DisplayName = "Trailing directory separator")]
	[DataRow("casing", DisplayName = "Differing casing")]
	public void GetTodosNormalizesSourceFolderForTheRelativePath(string variant)
	{
		ConfigureCommonMocks();

		string rootPath = Path.Combine(Environment.CurrentDirectory, $"dds-tools-tests-{Guid.NewGuid():N}");
		string sourceFolder = Path.Combine(rootPath, "source");

		ConvertSettings settings = new()
		{
			SourceFolder = variant switch
			{
				"relative" => Path.GetRelativePath(Environment.CurrentDirectory, sourceFolder),
				"trailing" => $"{sourceFolder}{Path.DirectorySeparatorChar}",
				_ => sourceFolder.ToUpperInvariant()
			},
			TargetFolder = Path.Combine(rootPath, "target"),
			From = ImageType.DDS,
			To = ImageType.PNG,
			ConvertMode = ConvertModeType.Manual
		};

		_directoryProviderMock
			.Setup(x => x.GetFiles(settings.SourceFolder, "*.dds", SearchOption.AllDirectories))
			.Returns([Path.Combine(sourceFolder, "Blue", "64.dds")]);

		TodoService sut = CreateSut();

		TodoCollection result = sut.GetTodos(settings);

		Assert.AreEqual($"{Path.DirectorySeparatorChar}Blue", result.Single().RelativePath);
	}

	[TestMethod]
	public void GetTodosFromJsonMapsLedgerEntriesBackToTheirOriginalFormat()
	{
		ConfigureCommonMocks();
		ConvertSettings settings = CreateSettings(ConvertModeType.Manual);
		settings.From = null;
		settings.Restore = true;

		string jsonContent = """
			[
				{
					"fileName":"64.PNG",
					"relativePath":"\\Blue",
					"fileHash":"ABC123",
					"sourceType":"TGA",
					"originalName":"64.tga"
				}
			]
			""";

		TodoService sut = CreateSut();

		TodoCollection result = sut.GetTodos(settings, jsonContent);

		TodoModel todo = result.Single();
		Assert.AreEqual("64.tga", todo.FileName);
		Assert.AreEqual("64.tga", todo.OriginalName);
		Assert.AreEqual(ImageType.PNG, todo.SourceType);
		Assert.AreEqual(ImageType.TGA, todo.TargetType);
		Assert.AreEqual(Path.Combine($"{settings.SourceFolder}\\Blue", "64.PNG"), todo.FullPathName);
	}

	[TestMethod]
	public void GetTodosFromJsonWithoutOriginalFormatThrowsServiceExceptionAndLogs()
	{
		ConfigureCommonMocks();
		ConvertSettings settings = CreateSettings(ConvertModeType.Manual);
		settings.Restore = true;

		string jsonContent = """
			[
				{ "fileName":"64.PNG", "relativePath":"", "fileHash":"ABC123" }
			]
			""";

		TodoService sut = CreateSut();

		Assert.Throws<ServiceException>(() => sut.GetTodos(settings, jsonContent));

		_loggerServiceMock.Verify(
			x => x.Log(It.IsAny<Action<ILogger, Exception?>>(), It.IsAny<Exception?>()),
			Times.Once);
	}

	[TestMethod]
	public void GetTodosFromJsonWhenJsonIsInvalidThrowsServiceExceptionAndLogs()
	{
		ConfigureCommonMocks();
		ConvertSettings settings = CreateSettings(ConvertModeType.Automatic);
		TodoService sut = CreateSut();

		Assert.Throws<ServiceException>(() => sut.GetTodos(settings, "{"));

		_loggerServiceMock.Verify(
			x => x.Log(It.IsAny<Action<ILogger, Exception?>>(), It.IsAny<Exception?>()),
			Times.Once);
	}

	[TestMethod]
	public void GetTodosDoneAutomaticWithDuplicatesEncodesOnceAndWritesJson()
	{
		ConfigureCommonMocks();
		SetupCanvas(64, 32);
		ConvertSettings settings = CreateSettings(ConvertModeType.Automatic);

		TodoCollection todos = new();
		todos.Enqueue(new TodoModel("a.DDS", string.Empty, Path.Combine(settings.SourceFolder, "a.DDS"), settings.TargetFolder, "DUP_HASH", ImageType.DDS, "a.DDS"));
		todos.Enqueue(new TodoModel("b.DDS", string.Empty, Path.Combine(settings.SourceFolder, "b.DDS"), settings.TargetFolder, "DUP_HASH", ImageType.DDS, "b.DDS"));

		TodoService sut = CreateSut();

		sut.GetTodosDone(todos, settings);

		string expectedSavePath = Path.Combine(settings.TargetFolder, "64", "DUP_HASH.PNG");
		string expectedJsonPath = Path.Combine(settings.TargetFolder, "Result.json");

		_encoderMock.Verify(x => x.Encode(It.IsAny<ImageCanvas>(), CompressionLevel.Balanced), Times.Once);
		_fileProviderMock.Verify(x => x.WriteAllBytes(expectedSavePath, It.IsAny<byte[]>()), Times.Once);
		_fileProviderMock.Verify(x => x.WriteAllText(expectedJsonPath, It.IsAny<string>()), Times.Once);
	}

	[TestMethod]
	public void GetTodosDoneGroupingWithDuplicatesCopiesOnce()
	{
		ConfigureCommonMocks();
		SetupCanvas(128, 64);
		ConvertSettings settings = CreateSettings(ConvertModeType.Grouping);

		TodoCollection todos = new();
		todos.Enqueue(new TodoModel("x.DDS", string.Empty, Path.Combine(settings.SourceFolder, "x.DDS"), settings.TargetFolder, "DUP_HASH", ImageType.DDS, "x.DDS"));
		todos.Enqueue(new TodoModel("y.DDS", string.Empty, Path.Combine(settings.SourceFolder, "y.DDS"), settings.TargetFolder, "DUP_HASH", ImageType.DDS, "y.DDS"));

		TodoService sut = CreateSut();

		sut.GetTodosDone(todos, settings, jsonExists: true);

		string expectedCopyPath = Path.Combine(settings.TargetFolder, "128x64", "x.DDS");

		_fileProviderMock.Verify(x => x.Copy(Path.Combine(settings.SourceFolder, "x.DDS"), expectedCopyPath), Times.Once);
		_encoderMock.Verify(x => x.Encode(It.IsAny<ImageCanvas>(), It.IsAny<CompressionLevel>()), Times.Never);
		_fileProviderMock.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void GetTodosDoneManualWithRetainStructureKeepsFolderAndFileNameAndWritesJson()
	{
		ConfigureCommonMocks();
		SetupCanvas(64, 64);
		ConvertSettings settings = CreateSettings(ConvertModeType.Manual);
		settings.RetainStructure = true;

		string relativePath = $"{Path.DirectorySeparatorChar}Blue";
		TodoCollection todos = new();
		todos.Enqueue(new TodoModel("64.DDS", relativePath, Path.Combine(settings.SourceFolder, "Blue", "64.DDS"), settings.TargetFolder, "HASH_A", ImageType.DDS, "64.DDS"));

		TodoService sut = CreateSut();

		sut.GetTodosDone(todos, settings);

		string expectedSavePath = Path.Combine($"{settings.TargetFolder}{relativePath}", "64.PNG");
		string expectedJsonPath = Path.Combine(settings.TargetFolder, "Result.json");

		_fileProviderMock.Verify(x => x.WriteAllBytes(expectedSavePath, It.IsAny<byte[]>()), Times.Once);
		_fileProviderMock.Verify(x => x.WriteAllText(expectedJsonPath, It.IsAny<string>()), Times.Once);
	}

	[TestMethod]
	public void GetTodosDoneManualWithSeparateBySizeUsesWidthFolderAndHashName()
	{
		ConfigureCommonMocks();
		SetupCanvas(64, 64);
		ConvertSettings settings = CreateSettings(ConvertModeType.Manual);
		settings.SeparateBySize = true;

		TodoCollection todos = new();
		todos.Enqueue(new TodoModel("64.DDS", string.Empty, Path.Combine(settings.SourceFolder, "64.DDS"), settings.TargetFolder, "HASH_A", ImageType.DDS, "64.DDS"));

		TodoService sut = CreateSut();

		sut.GetTodosDone(todos, settings);

		string expectedSavePath = Path.Combine(settings.TargetFolder, "64", "HASH_A.PNG");

		_fileProviderMock.Verify(x => x.WriteAllBytes(expectedSavePath, It.IsAny<byte[]>()), Times.Once);
	}

	[TestMethod]
	public void GetTodosDoneManualWithoutOptionsUsesTargetFolderAndHashName()
	{
		ConfigureCommonMocks();
		SetupCanvas(64, 64);
		ConvertSettings settings = CreateSettings(ConvertModeType.Manual);

		TodoCollection todos = new();
		todos.Enqueue(new TodoModel("64.DDS", string.Empty, Path.Combine(settings.SourceFolder, "64.DDS"), settings.TargetFolder, "HASH_A", ImageType.DDS, "64.DDS"));

		TodoService sut = CreateSut();

		sut.GetTodosDone(todos, settings);

		string expectedSavePath = Path.Combine(settings.TargetFolder, "HASH_A.PNG");

		_fileProviderMock.Verify(x => x.WriteAllBytes(expectedSavePath, It.IsAny<byte[]>()), Times.Once);

		// The result json is only persisted in automatic or name-preserving manual mode.
		_fileProviderMock.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void GetTodosDoneManualWithDuplicatesSavesEveryTodo()
	{
		ConfigureCommonMocks();
		SetupCanvas(64, 64);
		ConvertSettings settings = CreateSettings(ConvertModeType.Manual);

		TodoCollection todos = new();
		todos.Enqueue(new TodoModel("a.DDS", string.Empty, Path.Combine(settings.SourceFolder, "a.DDS"), settings.TargetFolder, "DUP_HASH", ImageType.DDS, "a.DDS"));
		todos.Enqueue(new TodoModel("b.DDS", string.Empty, Path.Combine(settings.SourceFolder, "b.DDS"), settings.TargetFolder, "DUP_HASH", ImageType.DDS, "b.DDS"));

		TodoService sut = CreateSut();

		sut.GetTodosDone(todos, settings);

		// Manual mode does not deduplicate by hash, so both todos are saved.
		string expectedSavePath = Path.Combine(settings.TargetFolder, "DUP_HASH.PNG");

		_fileProviderMock.Verify(x => x.WriteAllBytes(expectedSavePath, It.IsAny<byte[]>()), Times.Exactly(2));
	}

	[TestMethod]
	public void GetTodosDoneRestoreProcessesEveryLedgerEntryEvenOnDuplicateHashes()
	{
		ConfigureCommonMocks();
		SetupCanvas(64, 64);
		ConvertSettings settings = CreateSettings(ConvertModeType.Automatic);
		settings.Restore = true;

		TodoCollection todos = new();
		todos.Enqueue(new TodoModel("a.tga", "\\Blue", Path.Combine(settings.SourceFolder, "Blue", "a.PNG"), settings.TargetFolder, "DUP_HASH", ImageType.PNG, "a.tga") { TargetType = ImageType.TGA });
		todos.Enqueue(new TodoModel("b.jpg", "\\Red", Path.Combine(settings.SourceFolder, "Red", "b.PNG"), settings.TargetFolder, "DUP_HASH", ImageType.PNG, "b.jpg") { TargetType = ImageType.JPG });

		TodoService sut = CreateSut();

		sut.GetTodosDone(todos, settings, jsonExists: true);

		_fileProviderMock.Verify(x => x.WriteAllBytes(Path.Combine($"{settings.TargetFolder}\\Blue", "a.TGA"), It.IsAny<byte[]>()), Times.Once);
		_fileProviderMock.Verify(x => x.WriteAllBytes(Path.Combine($"{settings.TargetFolder}\\Red", "b.JPG"), It.IsAny<byte[]>()), Times.Once);
		_fileProviderMock.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void GetTodosDoneWhenDecodeThrowsThrowsServiceExceptionAndLogs()
	{
		ConfigureCommonMocks();
		ConvertSettings settings = CreateSettings(ConvertModeType.Automatic);

		TodoCollection todos = new();
		todos.Enqueue(new TodoModel("a.DDS", string.Empty, Path.Combine(settings.SourceFolder, "a.DDS"), settings.TargetFolder, "HASH_A", ImageType.DDS, "a.DDS"));

		_decoderMock.Setup(x => x.Decode(It.IsAny<byte[]>())).Throws(new InvalidOperationException("boom"));

		TodoService sut = CreateSut();

		Assert.Throws<ServiceException>(() => sut.GetTodosDone(todos, settings));

		_loggerServiceMock.Verify(
			x => x.Log(It.IsAny<Action<ILogger, Exception?>>(), It.IsAny<Exception?>()),
			Times.Once);
	}

	[TestMethod]
	public void GetTodosWhenProviderThrowsThrowsServiceExceptionAndLogs()
	{
		ConfigureCommonMocks();
		ConvertSettings settings = CreateSettings(ConvertModeType.Automatic);

		_directoryProviderMock
			.Setup(x => x.GetFiles(settings.SourceFolder, "*.dds", SearchOption.AllDirectories))
			.Throws(new InvalidOperationException("boom"));

		TodoService sut = CreateSut();

		Assert.Throws<ServiceException>(() => sut.GetTodos(settings));

		_loggerServiceMock.Verify(
			x => x.Log(It.IsAny<Action<ILogger, Exception?>>(), It.IsAny<Exception?>()),
			Times.Once);
	}

	private TodoService CreateSut()
		=> new(
			_loggerServiceMock.Object,
			_directoryProviderMock.Object,
			_fileProviderMock.Object,
			_pathProviderMock.Object,
			_codecRegistryMock.Object);

	private void SetupCanvas(int width, int height)
		=> _decoderMock.Setup(x => x.Decode(It.IsAny<byte[]>())).Returns(new ImageCanvas(width, height, new byte[width * height * 4]));

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

		_pathProviderMock
			.Setup(x => x.GetFullPath(It.IsAny<string>()))
			.Returns((string path) => Path.GetFullPath(path));

		_pathProviderMock
			.Setup(x => x.TrimEndingDirectorySeparator(It.IsAny<string>()))
			.Returns((string path) => Path.TrimEndingDirectorySeparator(path));

		_fileProviderMock
			.Setup(x => x.ReadAllBytes(It.IsAny<string>()))
			.Returns([0, 0, 0, 0]);

		_decoderMock
			.Setup(x => x.Decode(It.IsAny<byte[]>()))
			.Returns(new ImageCanvas(1, 1, [0, 0, 0, 0]));

		_encoderMock
			.Setup(x => x.Encode(It.IsAny<ImageCanvas>(), It.IsAny<CompressionLevel>()))
			.Returns([1, 2, 3]);

		_codecRegistryMock.Setup(x => x.GetDecoder(It.IsAny<ImageType>())).Returns(_decoderMock.Object);
		_codecRegistryMock.Setup(x => x.GetEncoder(It.IsAny<ImageType>())).Returns(_encoderMock.Object);
	}

	private static ConvertSettings CreateSettings(ConvertModeType convertMode)
	{
		string rootPath = Path.Combine(Path.GetTempPath(), $"dds-tools-tests-{Guid.NewGuid():N}");

		return new ConvertSettings
		{
			SourceFolder = Path.Combine(rootPath, "source"),
			TargetFolder = Path.Combine(rootPath, "target"),
			From = ImageType.DDS,
			To = ImageType.PNG,
			ConvertMode = convertMode
		};
	}
}
