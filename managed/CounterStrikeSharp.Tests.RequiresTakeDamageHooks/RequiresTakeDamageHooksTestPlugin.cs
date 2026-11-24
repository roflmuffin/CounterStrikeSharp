using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using Microsoft.Extensions.Logging;

namespace RequiresTakeDamageHooksTestPlugin;

/// <summary>
/// End-to-end fixture for Required Engine Features (ADR-0002): a match-integrity style plugin that loads only when
/// <c>cssharp.entity/take-damage-hooks</c> is available, and is refused at load otherwise.
/// </summary>
[RequiredEngineFeatures("cssharp.entity/take-damage-hooks")]
public class RequiresTakeDamageHooksTestPlugin : BasePlugin
{
    public override string ModuleName => "Requires Take Damage Hooks Test";
    public override string ModuleVersion => "v1.0.0";

    public override void Load(bool hotReload)
    {
        Logger.LogInformation("Requires Take Damage Hooks Test loaded");
    }
}
