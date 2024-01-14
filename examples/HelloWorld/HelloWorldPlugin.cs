using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using Microsoft.Extensions.Logging;

namespace HelloWorld;

[MinimumApiVersion(80)]
public class HelloWorldPlugin : BasePlugin
{
    public override string ModuleName => "Example: Hello World";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A simple plugin that says hello world!";

    public override void Load(bool hotReload)
    {
        Logger.LogInformation("Hello World! We are loading!");
    }
    
    public override void Unload(bool hotReload)
    {
        Logger.LogInformation("Hello World! We are unloading!");
    }
}