using CounterStrikeSharp.API.Modules.Cvars;

namespace WithFakeConvars;

public static class ConVars
{
    // This convar is registered from the plugin instance but can be used anywhere.
    public static FakeConVar<int> ExampleStaticCvar = new("example_static", "An example static cvar");
}