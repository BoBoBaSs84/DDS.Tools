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
	public void TestInitialize()
		=> s_context?.WriteLine($"Initialize {s_context.TestName} ..");

	[TestCleanup]
	public void TestCleanup()
		=> s_context?.WriteLine($"Cleanup {s_context.TestName} ..");

	public static IServiceProvider ServiceProvider { get; private set; } = default!;

	private static IHost CreateTestHost()
	{
		IHostBuilder host = Host.CreateDefaultBuilder().ConfigureServices((context, services)
			=> services.RegisterServices(context.HostingEnvironment));

		return host.Start();
	}
}
