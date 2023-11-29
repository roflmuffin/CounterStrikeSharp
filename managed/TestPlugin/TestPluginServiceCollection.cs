using System;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestPlugin;

public class TestInjectedClass
{
    private readonly ILogger<TestInjectedClass> _logger;

    public TestInjectedClass(ILogger<TestInjectedClass> logger)
    {
        _logger = logger;
    }

    public void Hello()
    {
        _logger.LogInformation("Hello World from Test Injected Class");
    }
}

public class TestPluginServiceCollection : IPluginServiceCollection<SamplePlugin>
{
    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<TestInjectedClass>();
    }
}