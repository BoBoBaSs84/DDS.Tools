// -----------------------------------------------------------------------------
// Copyright:	Robert Peter Meyer
// License:		MIT
//
// This source code is licensed under the MIT license found in the
// LICENSE file in the root directory of this source tree.
// -----------------------------------------------------------------------------
using DDS.Tools.Extensions;

using Microsoft.Extensions.Hosting;

namespace DDS.ToolsTests;

[TestClass]
public abstract class UnitTestBase
{
	[AssemblyInitialize]
	public static void AssemblyInitialize(TestContext context)
	{
		IHost host = CreateTestHost();
		ServiceProvider = host.Services;
	}

	public static IServiceProvider ServiceProvider { get; private set; } = default!;

	private static IHost CreateTestHost()
	{
		IHostBuilder host = Host.CreateDefaultBuilder().ConfigureServices((context, services)
			=> services.RegisterServices(context.HostingEnvironment));

		return host.Start();
	}
}

public static class TestConstants
{
	public static readonly string ResourcePath = Path.Combine(Environment.CurrentDirectory, "Resources");
	public static readonly string PngResourcePath = Path.Combine(ResourcePath, "PNG");
	public static readonly string PngResultPath = Path.Combine(ResourcePath, "PngResult");
	public static readonly string DdsResourcePath = Path.Combine(ResourcePath, "DDS");
	public static readonly string DdsResultPath = Path.Combine(ResourcePath, "DdsResult");
	public static readonly string JsonResourcePath = Path.Combine(ResourcePath, "JSON");
}
