using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Cvars;
using Microsoft.Extensions.Logging;

namespace WithConVars;

public class WithConVarsPlugin : BasePlugin
{
    public override string ModuleName => "Example: With ConVars";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A simple plugin that creates and accesses real console variables";

    // These are real engine ConVars, so they can be queried and set in the server console.
    // Owned ConVars declared as public fields on the plugin are automatically tracked
    // and deleted when the plugin is unloaded, including during hot reload.
    public ConVar<bool> Enabled = new("example_real_enabled", "Whether the example is enabled", true);
    public ConVar<string> Message = new("example_real_message", "The example message", "Hello world!");

    // Numeric bounds are optional. The engine clamps value assignments to this range.
    public ConVar<float> Speed = new(new ConVarCreationOptions<float>
    {
        Name = "example_real_speed",
        Description = "The example speed",
        DefaultValue = 10.0f,
        MinValue = 0.0f,
        MaxValue = 100.0f,
        Flags = ConVarFlags.FCVAR_NOTIFY
    });

    public override void Load(bool hotReload)
    {
        // The listener receives changes to all ConVars. Filter by the ConVar you need.
        RegisterListener<Listeners.OnConVarChanged>(conVar =>
        {
            if (!conVar.Equals(Speed)) return;

            // The callback provides a ConVarBase, you can use `As<T>()` to get typed access.
            Logger.LogInformation("{Name} changed to {Value}", conVar.Name, conVar.As<float>().Value);
        });

        // Value reads and writes use the same API for primitive values and strings.
        Logger.LogInformation("Example enabled: {Enabled}; message: {Message}", Enabled.Value, Message.Value);
        Message.Value = "Updated from C#!";
        Speed.Value = 150.0f;
        Logger.LogInformation("Speed after clamping: {Speed}", Speed.Value); // 100

        // Use TryGetMinValue/TryGetMaxValue when a bound might not exist.
        if (Speed.TryGetMinValue(out var minimum) && Speed.TryGetMaxValue(out var maximum))
        {
            Logger.LogInformation("Speed bounds: {Minimum} to {Maximum}", minimum, maximum);
        }

        // Find returns null if the ConVar does not exist. T must match its engine type;
        // Find does not convert types, and accessing Value with the wrong type will throw.
        var warmupTime = ConVar<float>.Find("mp_warmuptime");
        if (warmupTime is not null)
        {
            Logger.LogInformation("{Name} = {Value}", warmupTime.Name, warmupTime.Value);
        }

        // ConVars created in local variables are not automatically tracked by the plugin.
        // Delete them explicitly when finished. Only delete ConVars your plugin owns.
        var temporary = new ConVar<int>("example_real_temporary", "A temporary example ConVar", 42);
        try
        {
            Logger.LogInformation("{Name} = {Value}", temporary.Name, temporary.Value);
        }
        finally
        {
            temporary.Delete();
        }
    }
}
