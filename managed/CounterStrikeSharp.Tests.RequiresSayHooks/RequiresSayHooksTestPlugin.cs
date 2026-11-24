using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using Microsoft.Extensions.Logging;

namespace RequiresSayHooksTestPlugin;

/// <summary>
/// End-to-end fixture for Required Engine Features (ADR-0002): loads only when <c>cssharp.chat/say-hooks</c> is
/// available, and is refused at load otherwise (e.g. with <c>Host_Say</c> broken in Gamedata).
/// </summary>
[RequiredEngineFeatures("cssharp.chat/say-hooks")]
public class RequiresSayHooksTestPlugin : BasePlugin
{
    public override string ModuleName => "Requires Say Hooks Test";
    public override string ModuleVersion => "v1.0.0";

    public override void Load(bool hotReload)
    {
        Logger.LogInformation("Requires Say Hooks Test loaded");
    }
}
