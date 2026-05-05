// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using System.Reflection;

using DDS.Tools.Commands;
using DDS.Tools.Enumerators;
using DDS.Tools.Interfaces.Providers;
using DDS.Tools.Interfaces.Services;
using DDS.Tools.Models;
using DDS.Tools.Settings;
using DDS.Tools.Settings.Base;

using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace DDS.Tools.Tests.Commands;

[TestClass]
public sealed class PngConvertCommandTests
{
	[TestMethod]
	public void PngConvertCommandExecuteSuccessTest()
	{
		Mock<ILoggerService<DdsConvertCommand>> loggerMock = new();
		Mock<ITodoService> todoServiceMock = new();
		Mock<IDirectoryProvider> directoryProviderMock = new();
		Mock<IFileProvider> fileProviderMock = new();
		Mock<IPathProvider> pathProviderMock = new();

		PngConvertSettings settings = new()
		{
			SourceFolder = @"X:\Source",
			TargetFolder = @"X:\Target"
		};
		TodoCollection todos = CreateTodos();

		directoryProviderMock.Setup(x => x.Exists(settings.SourceFolder)).Returns(true);
		pathProviderMock.Setup(x => x.Combine(settings.SourceFolder, "Result.json")).Returns(@"X:\Source\Result.json");
		fileProviderMock.Setup(x => x.Exists(@"X:\Source\Result.json")).Returns(false);
		todoServiceMock.Setup(x => x.GetTodos(settings, ImageType.PNG)).Returns(todos);

		IServiceProvider serviceProvider = CreateServiceProvider(directoryProviderMock.Object, fileProviderMock.Object, pathProviderMock.Object);
		PngConvertCommand command = new(loggerMock.Object, todoServiceMock.Object, serviceProvider);

		int result = InvokeExecute(command, settings);

		Assert.AreEqual(0, result);
		todoServiceMock.Verify(x => x.GetTodos(settings, ImageType.PNG), Times.Once);
		todoServiceMock.Verify(x => x.GetTodosDone(todos, settings, ImageType.PNG, false), Times.Once);
		loggerMock.Verify(x => x.Log(It.IsAny<Action<Microsoft.Extensions.Logging.ILogger, Exception?>>(), It.IsAny<Exception?>()), Times.Never);
	}

	[TestMethod]
	public void PngConvertCommandExecuteExceptionTest()
	{
		Mock<ILoggerService<DdsConvertCommand>> loggerMock = new();
		Mock<ITodoService> todoServiceMock = new();
		Mock<IDirectoryProvider> directoryProviderMock = new();
		Mock<IFileProvider> fileProviderMock = new();
		Mock<IPathProvider> pathProviderMock = new();

		PngConvertSettings settings = new()
		{
			SourceFolder = @"X:\Missing",
			TargetFolder = @"X:\Target"
		};

		directoryProviderMock.Setup(x => x.Exists(settings.SourceFolder)).Returns(false);

		IServiceProvider serviceProvider = CreateServiceProvider(directoryProviderMock.Object, fileProviderMock.Object, pathProviderMock.Object);
		PngConvertCommand command = new(loggerMock.Object, todoServiceMock.Object, serviceProvider);

		int result = InvokeExecute(command, settings);

		Assert.AreEqual(1, result);
		loggerMock.Verify(x => x.Log(It.IsAny<Action<Microsoft.Extensions.Logging.ILogger, Exception?>>(), It.IsAny<Exception?>()), Times.Once);
		todoServiceMock.Verify(x => x.GetTodos(It.IsAny<ConvertSettingsBase>(), It.IsAny<ImageType>()), Times.Never);
		todoServiceMock.Verify(x => x.GetTodosDone(It.IsAny<TodoCollection>(), It.IsAny<ConvertSettingsBase>(), It.IsAny<ImageType>(), It.IsAny<bool>()), Times.Never);
	}

	private static int InvokeExecute(PngConvertCommand command, PngConvertSettings settings)
	{
		MethodInfo method = typeof(PngConvertCommand).GetMethod("Execute", BindingFlags.Instance | BindingFlags.NonPublic)!;
		object? result = method.Invoke(command, [null!, settings, CancellationToken.None]);
		return (int)result!;
	}

	private static TodoCollection CreateTodos()
	{
		TodoCollection todos = [];
		todos.Enqueue(new TodoModel("32.dds", string.Empty, @"X:\Source\32.dds", @"X:\Target", "HASH-1"));
		return todos;
	}

	private static ServiceProvider CreateServiceProvider(IDirectoryProvider directoryProvider, IFileProvider fileProvider, IPathProvider pathProvider)
	{
		ServiceCollection services = new();
		services.AddSingleton(directoryProvider);
		services.AddSingleton(fileProvider);
		services.AddSingleton(pathProvider);
		return services.BuildServiceProvider();
	}
}
