using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core.Bindings;

/// <summary>
/// Which Engine Feature fires each listener (by <see cref="Attributes.ListenerNameAttribute"/> name). A listener on a disabled
/// Engine Feature still registers but never fires, so registering it logs once (ADR-0002). Listeners not listed here
/// are not backed by an Engine Feature.
/// </summary>
internal static class ListenerEngineFeatures
{
    private static readonly Dictionary<string, string> Features = new()
    {
        // Chat (Host_Say)
        ["OnPlayerChat"] = EngineFeatureNames.ChatSayHooks,
        // Server lifecycle (GameFrame)
        ["OnTick"] = EngineFeatureNames.ServerGameFrame,
        // Server lifecycle (the other ISource2Server hooks)
        ["OnServerHibernationUpdate"] = EngineFeatureNames.ServerHibernationEvents,
        ["OnGameServerSteamAPIActivated"] = EngineFeatureNames.ServerSteamApiEvents,
        ["OnGameServerSteamAPIDeactivated"] = EngineFeatureNames.ServerSteamApiEvents,
        ["OnHostNameChanged"] = EngineFeatureNames.ServerHostnameChanged,
        ["OnPreFatalShutdown"] = EngineFeatureNames.ServerFatalShutdown,
        ["OnUpdateWhenNotInGame"] = EngineFeatureNames.ServerUpdateWhenNotInGame,
        ["OnServerPreWorldUpdate"] = EngineFeatureNames.ServerPreWorldUpdate,
        // Players and UI (IServerGameClients hooks). A player is authorized only once its connection was seen.
        ["OnClientConnect"] = EngineFeatureNames.PlayerConnectionEvents,
        ["OnClientConnected"] = EngineFeatureNames.PlayerConnectionEvents,
        ["OnClientPutInServer"] = EngineFeatureNames.PlayerConnectionEvents,
        ["OnClientDisconnect"] = EngineFeatureNames.PlayerConnectionEvents,
        ["OnClientDisconnectPost"] = EngineFeatureNames.PlayerConnectionEvents,
        ["OnClientAuthorized"] = EngineFeatureNames.PlayerConnectionEvents,
        ["OnClientVoice"] = EngineFeatureNames.PlayerVoiceEvents,
        ["OnCustomHudClicked"] = EngineFeatureNames.UiCustomHudLayout,
        // Networking
        ["CheckTransmit"] = EngineFeatureNames.NetworkCheckTransmit,
        // Fired from our game system (precache, entity think) and player think (button changes)
        ["OnServerPrecacheResources"] = EngineFeatureNames.ServerGameSystemEvents,
        ["OnServerPreEntityThink"] = EngineFeatureNames.ServerGameSystemEvents,
        ["OnServerPostEntityThink"] = EngineFeatureNames.ServerGameSystemEvents,
        ["OnPlayerButtonsChanged"] = EngineFeatureNames.ServerGameSystemEvents,
        // Take damage
        ["OnEntityTakeDamagePre"] = EngineFeatureNames.EntityTakeDamageHooks,
        ["OnEntityTakeDamagePost"] = EngineFeatureNames.EntityTakeDamageHooks,
        ["OnPlayerTakeDamagePre"] = EngineFeatureNames.EntityTakeDamageHooks,
        ["OnPlayerTakeDamagePost"] = EngineFeatureNames.EntityTakeDamageHooks,
        // ConVars (ICvar global change callback)
        ["OnConVarChanged"] = EngineFeatureNames.ConvarsChangeEvents,
    };

    /// <summary>
    /// True if the listener can fire. Otherwise logs once per listener and registrant that it is registered on a
    /// disabled Engine Feature and returns false; the caller keeps the registration either way.
    /// </summary>
    public static bool CheckRegistration(EngineFeatureSet features, string listenerName, ILogger logger,
        object? registrant = null) =>
        !Features.TryGetValue(listenerName, out var feature) ||
        features.CheckRegistration(feature, $"{listenerName} listener", logger, registrant);

    /// <summary>
    /// Command listeners (<see cref="BasePlugin.AddCommandListener"/>) see commands a player types through the
    /// IServerGameClients ClientCommand hook. On a disabled <c>cssharp.player/client-commands</c> they still register
    /// (and still see console-dispatched commands), but client-typed commands never reach them, so this logs once and
    /// returns false.
    /// </summary>
    public static bool CheckCommandListenerRegistration(EngineFeatureSet features, ILogger logger, object? registrant = null) =>
        features.CheckRegistration(EngineFeatureNames.PlayerClientCommands, "Command listener on client-typed commands", logger,
            registrant);
}
