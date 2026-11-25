using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core.Bindings;

/// <summary>
/// Which Engine Feature runs the tasks each <see cref="Server"/> scheduling API queues (by method name). A task queued
/// while that Engine Feature is disabled is kept but never runs, so queueing one logs once (ADR-0002), like a listener.
/// </summary>
internal static class FrameTaskEngineFeatures
{
    private static readonly Dictionary<string, string> Features = new()
    {
        // Run from the GameFrame hook (the managed OnTick listener and the native tick scheduler)
        [nameof(Server.NextFrame)] = EngineFeatureNames.ServerGameFrame,
        [nameof(Server.RunOnTick)] = EngineFeatureNames.ServerGameFrame,
        // Run from the PreWorldUpdate hook (the managed OnServerPreWorldUpdate listener)
        [nameof(Server.NextWorldUpdate)] = EngineFeatureNames.ServerPreWorldUpdate,
    };

    /// <summary>
    /// True if tasks queued through <paramref name="api"/> run. Otherwise logs once per API and logger that they never
    /// will and returns false; the caller queues the task either way.
    /// </summary>
    public static bool CheckRegistration(EngineFeatureSet features, string api, ILogger logger) =>
        features.CheckRegistration(Features[api], $"{api} task", logger);
}
