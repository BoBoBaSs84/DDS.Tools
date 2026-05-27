// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Commands.Base;
using DDS.Tools.Enumerators;
using DDS.Tools.Exceptions;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Settings;
using DDS.Tools.Settings.Base;

using Moq;

using Spectre.Console.Cli;

namespace DDS.Tools.Tests.Commands.Base;

[TestClass]
public sealed class ConvertCommandBaseTests
{
	[TestMethod]
	public void ActionSourceFolderMissingThrowsCommandException()
	{
		Mock<ITodoService> todoServiceMock = new();
		Mock<IDirectoryProvider> directoryProviderMock = new();
		Mock<IFileProvider> fileProviderMock = new();
		Mock<IPathProvider> pathProviderMock = new();

		DdsConvertSettings settings = new() { SourceFolder = @"X:\Missing", TargetFolder = @"X:\Target" };
		directoryProviderMock.Setup(x => x.Exists(settings.SourceFolder)).Returns(false);

		TestConvertCommand command = new(todoServiceMock.Object, directoryProviderMock.Object, fileProviderMock.Object, pathProviderMock.Object);

		Assert.Throws<CommandException>(() => command.InvokeAction(settings, ImageType.DDS));

		todoServiceMock.Verify(x => x.GetTodos(It.IsAny<ConvertSettingsBase>(), It.IsAny<ImageType>()), Times.Never);
		todoServiceMock.Verify(x => x.GetTodos(It.IsAny<ConvertSettingsBase>(), It.IsAny<ImageType>(), It.IsAny<string>()), Times.Never);
		todoServiceMock.Verify(x => x.GetTodosDone(It.IsAny<TodoCollection>(), It.IsAny<ConvertSettingsBase>(), It.IsAny<ImageType>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void ActionWithoutJsonAndWithoutTodosReturnsOne()
	{
		Mock<ITodoService> todoServiceMock = new();
		Mock<IDirectoryProvider> directoryProviderMock = new();
		Mock<IFileProvider> fileProviderMock = new();
		Mock<IPathProvider> pathProviderMock = new();

		DdsConvertSettings settings = new() { SourceFolder = @"X:\Source", TargetFolder = @"X:\Target" };
		TodoCollection todos = [];

		directoryProviderMock.Setup(x => x.Exists(settings.SourceFolder)).Returns(true);
		pathProviderMock.Setup(x => x.Combine(settings.SourceFolder, "Result.json")).Returns(@"X:\Source\Result.json");
		fileProviderMock.Setup(x => x.Exists(@"X:\Source\Result.json")).Returns(false);
		todoServiceMock.Setup(x => x.GetTodos(settings, ImageType.DDS)).Returns(todos);

		TestConvertCommand command = new(todoServiceMock.Object, directoryProviderMock.Object, fileProviderMock.Object, pathProviderMock.Object);

		int result = command.InvokeAction(settings, ImageType.DDS);

		Assert.AreEqual(1, result);

		todoServiceMock.Verify(x => x.GetTodos(settings, ImageType.DDS), Times.Once);
		todoServiceMock.Verify(x => x.GetTodos(It.IsAny<ConvertSettingsBase>(), It.IsAny<ImageType>(), It.IsAny<string>()), Times.Never);
		todoServiceMock.Verify(x => x.GetTodosDone(It.IsAny<TodoCollection>(), It.IsAny<ConvertSettingsBase>(), It.IsAny<ImageType>(), It.IsAny<bool>()), Times.Never);
	}

	[TestMethod]
	public void ActionWithJsonUsesJsonOverloadAndReturnsZero()
	{
		Mock<ITodoService> todoServiceMock = new();
		Mock<IDirectoryProvider> directoryProviderMock = new();
		Mock<IFileProvider> fileProviderMock = new();
		Mock<IPathProvider> pathProviderMock = new();

		DdsConvertSettings settings = new() { SourceFolder = @"X:\Source", TargetFolder = @"X:\Target" };
		TodoCollection todos = [];
		todos.Enqueue(new TodoModel("a.dds", string.Empty, @"X:\Source\a.dds", @"X:\Target", "HASH"));

		const string JsonContent = "[{}]";
		const string JsonPath = @"X:\Source\Result.json";

		directoryProviderMock.Setup(x => x.Exists(settings.SourceFolder)).Returns(true);
		pathProviderMock.Setup(x => x.Combine(settings.SourceFolder, "Result.json")).Returns(JsonPath);
		fileProviderMock.Setup(x => x.Exists(JsonPath)).Returns(true);
		fileProviderMock.Setup(x => x.ReadAllText(JsonPath)).Returns(JsonContent);
		todoServiceMock.Setup(x => x.GetTodos(settings, ImageType.DDS, JsonContent)).Returns(todos);

		TestConvertCommand command = new(todoServiceMock.Object, directoryProviderMock.Object, fileProviderMock.Object, pathProviderMock.Object);

		int result = command.InvokeAction(settings, ImageType.DDS);

		Assert.AreEqual(0, result);

		todoServiceMock.Verify(x => x.GetTodos(It.IsAny<ConvertSettingsBase>(), It.IsAny<ImageType>()), Times.Never);
		todoServiceMock.Verify(x => x.GetTodos(settings, ImageType.DDS, JsonContent), Times.Once);
		todoServiceMock.Verify(x => x.GetTodosDone(todos, settings, ImageType.DDS, true), Times.Once);
	}

	private sealed class TestConvertCommand(
		ITodoService todoService,
		IDirectoryProvider directoryProvider,
		IFileProvider fileProvider,
		IPathProvider pathProvider)
		: ConvertCommandBase<DdsConvertSettings>(todoService, directoryProvider, fileProvider, pathProvider)
	{
		public int InvokeAction(ConvertSettingsBase settings, ImageType imageType)
			=> Action(settings, imageType);

		protected override int Execute(CommandContext context, DdsConvertSettings settings, CancellationToken cancellationToken)
			=> 0;
	}
}
