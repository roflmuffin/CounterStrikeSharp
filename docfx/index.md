---
_layout: landing
---

# CounterStrikeSharp

CounterStrikeSharp is a simpler way to write CS2 server plugins.

```csharp
using CounterStrikeSharp.API.Core;

namespace HelloWorldPlugin;

public class HelloWorldPlugin : BasePlugin
{
    public override string ModuleName => "Hello World Plugin";

    public override string ModuleVersion => "0.0.1";

    public override string ModuleAuthor => "roflmuffin";

    public override string ModuleDescription => "Simple hello world plugin";

    public override void Load(bool hotReload)
    {
        Logger.LogInformation("Plugin loaded successfully!");
    }

    [GameEventHandler]
    public HookResult OnPlayerConnect(EventPlayerConnect @event, GameEventInfo info)
    {
        // Userid will give you a reference to a CCSPlayerController class
        Logger.LogInformation("Player {Name} has connected!", @event.Userid.PlayerName);

        return HookResult.Continue;
    }

    [ConsoleCommand("issue_warning", "Issue warning to player")]
    public void OnCommand(CCSPlayerController? player, CommandInfo command)
    {
        Logger.LogWarning("Player shouldn't be doing that");
    }
}
```