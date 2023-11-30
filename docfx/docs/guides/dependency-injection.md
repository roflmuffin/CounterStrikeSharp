# Dependency Injection

How to make use of dependency injection in CounterStrikeSharp

`CounterStrikeSharp` uses a standard <a href="https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-8.0" target="_blank">`IServiceCollection`</a> to allow for dependency injection in plugins.

There are a handful of standard services that are predefined for you (`ILogger` for logging for instance), with more to come in the future. To add your own scoped & singleton services to the container, you can create a new class that implements the `IPluginServiceCollection<T>` interface for your plugin.

```csharp
public class TestPlugin : BasePlugin
{
  // Plugin code...
}

public class TestPluginServiceCollection : IPluginServiceCollection<TestPlugin>
{
    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ExampleInjectedClass>();
        serviceCollection.AddLogging(builder => ...);
    }
}
```

CounterStrikeSharp will search your assembly for any implementations of `IPlugin` and then any implementations of `IPluginServiceCollection<T>` where `T` is your plugin. It will then configure the service provider and then request a singleton instance of your plugin before proceeding to the load step.

In this way, any dependencies that are listed in your plugin class constructor will automatically get injected at instantation time (before load).

### Example

```csharp
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

public class SamplePlugin : BasePlugin
{
    private readonly TestInjectedClass _testInjectedClass;
    public SamplePlugin(TestInjectedClass testInjectedClass)
    {
        _testInjectedClass = testInjectedClass;
    }

    public override void Load(bool hotReload)
    {
        _testInjectedClass.Hello();
    }
}
```
