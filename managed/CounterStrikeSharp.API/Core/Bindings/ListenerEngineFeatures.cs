using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core.Bindings;

/// <summary>
/// Which Engine Feature fires each listener (by <see cref="ListenerNameAttribute"/> name). A listener on a disabled
/// Engine Feature still registers but never fires, so registering it logs once (ADR-0002). Listeners not listed here
/// are not backed by an Engine Feature.
/// </summary>
internal static class ListenerEngineFeatures
{
    private static readonly Dictionary<string, string> Features = new()
    {
        // ticket 09
        ["CheckTransmit"] = "cssharp.network/check-transmit",
        // ticket 10: fired from our game system (precache, entity think) and player think (button changes)
        ["OnServerPrecacheResources"] = "cssharp.server/game-system-events",
        ["OnServerPreEntityThink"] = "cssharp.server/game-system-events",
        ["OnServerPostEntityThink"] = "cssharp.server/game-system-events",
        ["OnPlayerButtonsChanged"] = "cssharp.server/game-system-events",
        // ticket 11
        ["OnEntityTakeDamagePre"] = "cssharp.entity/take-damage-hooks",
        ["OnEntityTakeDamagePost"] = "cssharp.entity/take-damage-hooks",
        ["OnPlayerTakeDamagePre"] = "cssharp.entity/take-damage-hooks",
        ["OnPlayerTakeDamagePost"] = "cssharp.entity/take-damage-hooks",
    };

    /// <summary>
    /// True if the listener can fire. Otherwise logs once per listener and registrant that it is registered on a
    /// disabled Engine Feature and returns false; the caller keeps the registration either way.
    /// </summary>
    public static bool CheckRegistration(EngineFeatureSet features, string listenerName, ILogger logger,
        object? registrant = null) =>
        !Features.TryGetValue(listenerName, out var feature) ||
        features.CheckRegistration(feature, $"{listenerName} listener", logger, registrant);
}
