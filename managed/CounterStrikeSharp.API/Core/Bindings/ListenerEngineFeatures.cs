using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core.Bindings;

/// <summary>
/// Which Engine Feature fires each native listener (ADR-0002). A listener on a disabled Engine Feature still registers,
/// but never fires; registering it logs that once.
/// </summary>
internal static class ListenerEngineFeatures
{
    private static readonly Dictionary<string, string> FeatureByListener = new()
    {
        // ticket 10: fired from our game system (precache, entity think) and player think (button changes)
        ["OnServerPrecacheResources"] = EngineFeatureNames.ServerGameSystemEvents,
        ["OnServerPreEntityThink"] = EngineFeatureNames.ServerGameSystemEvents,
        ["OnServerPostEntityThink"] = EngineFeatureNames.ServerGameSystemEvents,
        ["OnPlayerButtonsChanged"] = EngineFeatureNames.ServerGameSystemEvents,
    };

    /// <summary>
    /// False (logged once per listener) if the listener's Engine Feature is disabled; true for an available one or a
    /// listener no Engine Feature backs.
    /// </summary>
    public static bool CheckRegistration(EngineFeatureSet features, string listenerName, ILogger logger) =>
        !FeatureByListener.TryGetValue(listenerName, out var feature) ||
        features.CheckRegistration(feature, $"Listener {listenerName}", logger);
}
