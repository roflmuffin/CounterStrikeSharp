using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using Microsoft.Extensions.Logging;

namespace PluginSignaturesTestPlugin;

/// <summary>
/// End-to-end fixture for the plugin signatures report: resolves one unique signature (read from core Gamedata by key),
/// one ambiguous and one that is not found, then logs every entry the query API returns.
/// </summary>
public class PluginSignaturesTestPlugin : BasePlugin
{
    public override string ModuleName => "PluginSignaturesTest";
    public override string ModuleVersion => "v1.0.0";

    // Matches many function prologues in libserver.so.
    private const string Ambiguous = "55 48 89 E5";
    private const string NotFound = "DE AD BE EF 13 37 C0 DE 42 42 99 88";

    public override void Load(bool hotReload)
    {
        _ = new MemoryFunctionVoid<nint>(GameData.GetSignature("Host_Say"));
        _ = new MemoryFunctionVoid<nint>(Ambiguous);
        _ = new MemoryFunctionVoid<nint>(NotFound);

        foreach (var entry in EngineFeatures.GetPluginSignatures())
            Logger.LogInformation("Plugin signature entry: {Entry} (match count {MatchCount})", entry, entry.MatchCount);
    }
}
