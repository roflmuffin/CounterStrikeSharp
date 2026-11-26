using CounterStrikeSharp.API.Core.Bindings;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core;

/// <summary>
/// Which Engine Features work on the running CS2 build (ADR-0002). Bindings are resolved once at boot; an Engine
/// Feature is available exactly when all of its Bindings resolved. Ask by the stable string name
/// (e.g. <c>cssharp.chat/say-hooks</c>) or by the generated <see cref="EngineFeature"/> enum.
/// </summary>
public static class EngineFeatures
{
    private static readonly object Lock = new();
    private static EngineFeatureSet? _current;

    /// <summary>
    /// The process-wide Engine Feature set, built once from the native Resolution Report on first use
    /// (normally at startup, before plugins load). Tests install their own and <see cref="Reset"/> afterwards.
    /// </summary>
    internal static EngineFeatureSet Current
    {
        get
        {
            lock (Lock)
            {
                return _current ??= new EngineFeatureSet(ResolutionReport.FromJson(NativeAPI.GetResolutionReport()),
                    ManagedEngineFeatures.All);
            }
        }
        set
        {
            lock (Lock) _current = value;
        }
    }

    internal static void Reset()
    {
        lock (Lock) _current = null;
    }

    /// <summary>The CS2 build the Bindings were resolved against, e.g. <c>1.42.0.1/2000950</c>.</summary>
    public static string BuildId => Current.BuildId;

    /// <summary>True if the named Engine Feature is declared and all of its Bindings resolved.</summary>
    public static bool IsAvailable(string name) => Current.IsAvailable(name);

    /// <inheritdoc cref="IsAvailable(string)"/>
    public static bool IsAvailable(EngineFeature feature) => GetStatus(feature).IsAvailable;

    /// <summary>The status of the named Engine Feature: available, or which Binding failed and why.</summary>
    public static EngineFeatureStatus GetStatus(string name) => Current.GetStatus(name);

    /// <inheritdoc cref="GetStatus(string)"/>
    public static EngineFeatureStatus GetStatus(EngineFeature feature) =>
        feature.GetName() is { } name ? GetStatus(name) : EngineFeatureStatus.Unknown($"#{(int)feature}");

    /// <summary>Every Engine Feature this version declares, with its status, ordered by name.</summary>
    public static IReadOnlyList<EngineFeatureStatus> GetAll() => Current.GetAll();

    /// <summary>
    /// Every signature plugins resolved so far (memory functions and virtual functions created from a raw signature or a
    /// plugin's Gamedata), one entry per plugin, module and signature, ordered by plugin. Separate from the Engine
    /// Features: a plugin signature never disables one. A plugin's entries are replaced when it loads again.
    /// </summary>
    public static IReadOnlyList<PluginSignatureEntry> GetPluginSignatures() => PluginSignatures.All;

    /// <summary>Builds the set from the native report and logs the startup report (one block).</summary>
    internal static void LogStartupReport(ILogger logger)
    {
        var current = Current;
        logger.Log(current.AnyDisabled ? LogLevel.Warning : LogLevel.Information, "{StartupReport}",
            current.FormatStartupReport());
    }

    /// <summary>Stops the server (native). Tests replace it.</summary>
    internal static Action StopServer = NativeAPI.FailModeStopServer;

    /// <summary>
    /// The operator's <see cref="FailMode"/>, applied after the startup report: with <see cref="FailMode.Crash"/> and any
    /// core Engine Feature disabled, logs the fatal error naming them and stops the server. Returns false if it did not
    /// stop the server (normally, the server keeps running).
    /// </summary>
    internal static bool ApplyFailMode(ILogger logger, FailMode failMode)
    {
        if (Current.FormatFailModeError(failMode) is not { } error) return false;

        logger.LogCritical("{FailModeError}", error);
        try
        {
            StopServer();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Could not stop the server for FailMode crash; it keeps running with the Engine Features above disabled");
            return false;
        }
    }
}
