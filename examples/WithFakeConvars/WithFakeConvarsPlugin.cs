using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;

namespace WithFakeConvars;

[MinimumApiVersion(175)]
public class WithFakeConvarsPlugin : BasePlugin
{
    public override string ModuleName => "Example: With Fake Convars";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "CounterStrikeSharp & Contributors";
    public override string ModuleDescription => "A simple plugin that registers some console variables";
    
    // FakeConVar is a class that can be used to create custom console variables.
    // You can specify a name, description, default value, and custom validators.
    public FakeConVar<bool> BoolCvar = new("example_bool", "An example boolean cvar", true);
    
    // Range validator is an inbuilt validator that can be used to ensure that a value is within a certain range.
    public FakeConVar<int> ExampleIntCvar  = new("example_int", "An example integer cvar", 10, flags: ConVarFlags.FCVAR_NONE, new RangeValidator<int>(0, 100));
    
    public FakeConVar<float> ExampleFloatCvar = new("example_float", "An example float cvar", 10, flags: ConVarFlags.FCVAR_NONE, new RangeValidator<float>(5, 20));
    public FakeConVar<string> ExampleStringCvar = new("example_string", "An example string cvar", "default");

    // Replicated, Cheat & Protected flags are supported.
    public FakeConVar<float> ExamplePublicCvar = new("example_public_float", "An example public float cvar", 5,
        ConVarFlags.FCVAR_REPLICATED);
    
    // Can only be changed if sv_cheats is enabled.
    public FakeConVar<float> ExampleCheatCvar = new("example_cheat_float", "An example cheat float cvar", 5,
        ConVarFlags.FCVAR_CHEAT);
    
    // Protected cvars do not output their value when queried.
    public FakeConVar<float> ExampleProtectedCvar = new("example_protected_float", "An example cheat float cvar", 5,
        ConVarFlags.FCVAR_PROTECTED);
    
    // You can create your own custom validators by implementing the IValidator interface.
    public FakeConVar<int> ExampleEvenNumberCvar = new("example_even_number", "An example even number cvar", 0, flags: ConVarFlags.FCVAR_NONE, new EvenNumberValidator());

    public FakeConVar<int> RequiresRestartCvar = new("example_requires_restart", "A cvar that requires a restart when changed");
    
    public override void Load(bool hotReload)
    {
        // You can subscribe to the ValueChanged event to execute code when the value of a cvar changes.
        // In this example, we restart the game when the value of RequiresRestartCvar is greater than 5.
        RequiresRestartCvar.ValueChanged += (sender, value) =>
        {
            if (value > 5)
            {
                Server.ExecuteCommand("mp_restartgame 1");
            }
        };
        
        RegisterFakeConVars(typeof(ConVars));
    }
}
