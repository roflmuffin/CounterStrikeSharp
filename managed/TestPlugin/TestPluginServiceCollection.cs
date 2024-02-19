using System;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TestPlugin;

public class TestInjectedClass
{
    private readonly ILogger<TestInjectedClass> _logger;
    private readonly ICommandManager _commandManager;

    public TestInjectedClass(ILogger<TestInjectedClass> logger, ICommandManager commandManager)
    {
        _logger = logger;
        _commandManager = commandManager;
    }

    public void Hello()
    {
        _logger.LogInformation("Hello World from Test Injected Class");
        _commandManager.RegisterCommand(new CommandDefinition("cssharp_helloworld", "Hello World!", (player, info) =>
        {
            info.ReplyToCommand("Hello!");
        }));
    }
}

public class TestPluginServiceCollection : IPluginServiceCollection<SamplePlugin>
{
    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<TestInjectedClass>();
    }
}