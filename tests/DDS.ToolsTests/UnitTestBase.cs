using DDS.Tools.Extensions;

using Microsoft.Extensions.Hosting;

namespace DDS.ToolsTests;

[TestClass]
public abstract class UnitTestBase
{
	private static TestContext? s_context;

	[AssemblyInitialize]
	public static void AssemblyInitialize(TestContext context)
	{
		s_context = context;
		IHost host = CreateTestHost();
		ServiceProvider = host.Services;
	}

	[TestInitialize]
	public virtual void TestInitialize()
		=> s_context?.WriteLine($"Initialize {s_context.TestName} ..");

	[TestCleanup]
	public virtual void TestCleanup()
		=> s_context?.WriteLine($"Cleanup {s_context.TestName} ..");

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
