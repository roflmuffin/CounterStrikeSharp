using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WithDependencyInjection;

[MinimumApiVersion(80)]
public class WithDependencyInjectionPlugin : BasePlugin
{
    public override string ModuleName => "Example: Dependency Injection";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "An example plugin that uses dependency injection.";

    private readonly TestInjectedClass _testInjectedClass;
    
    public WithDependencyInjectionPlugin(TestInjectedClass testInjectedClass)
    {
        _testInjectedClass = testInjectedClass;
    }
    
    public override void Load(bool hotReload)
    {
        _testInjectedClass.SayHello();
    }
}

public class WithDependencyInjectionPluginServiceCollection : IPluginServiceCollection<WithDependencyInjectionPlugin>
{
    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<TestInjectedClass>();
    }
}

public class TestInjectedClass
{
    private readonly ILogger<TestInjectedClass> _logger;

    public TestInjectedClass(ILogger<TestInjectedClass> logger)
    {
        _logger = logger;
    }

    public void SayHello()
    {
        _logger.LogInformation("Hello World from Test Injected Class");
    }
}