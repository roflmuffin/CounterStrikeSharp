using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core.Bindings;

/// <summary>
/// The signatures plugins resolved, one <see cref="PluginSignatureEntry"/> per plugin, module, signature and strict
/// mode. Kept apart from the Resolution Report: plugin signatures never disable core Engine Features and never trigger
/// the operator's FailMode. The "plugin signatures" block lists the ones not resolved to exactly one match; it is
/// logged once after all plugins have loaded and again after each later plugin (re)load. A plugin's entries are
/// replaced when it loads again and dropped when it is unloaded for good.
/// </summary>
internal static class PluginSignatures
{
    private static readonly ConcurrentDictionary<(string Plugin, string Module, string Signature, bool Strict), PluginSignatureEntry>
        Entries = new();

    // Plugin load contexts by plugin name, so code in any of a plugin's assemblies is attributed to the plugin.
    private static readonly ConcurrentDictionary<AssemblyLoadContext, string> PluginContexts = new();

    // The Gamedata keys each signature was read under (GameData.GetSignature); several keys may share one signature.
    private static readonly ConcurrentDictionary<string, ImmutableSortedSet<string>> GamedataKeys = new();

    /// <summary>Which plugin is resolving a signature right now; null for CounterStrikeSharp itself. Tests replace it.</summary>
    internal static Func<string?> CurrentPlugin = FindCallingPlugin;

    /// <summary>Set once the initial plugin load finished; from then on every plugin (re)load logs the block again.</summary>
    internal static bool AllPluginsLoaded { get; set; }

    /// <summary>Every recorded entry, ordered by plugin, module and signature.</summary>
    internal static IReadOnlyList<PluginSignatureEntry> All =>
        Entries.Values.OrderBy(e => e.Plugin, StringComparer.Ordinal).ThenBy(e => e.Module, StringComparer.Ordinal)
            .ThenBy(e => e.Signature, StringComparer.Ordinal).ThenBy(e => e.Strict).ToList();

    /// <summary>A plugin starts loading: forget its previous entries, and attribute code in its load context to it.</summary>
    internal static void BeginPluginLoad(string plugin, Assembly pluginAssembly)
    {
        ForgetPlugin(plugin);
        if (AssemblyLoadContext.GetLoadContext(pluginAssembly) is { } context && context != AssemblyLoadContext.Default)
            PluginContexts[context] = plugin;
    }

    /// <summary>A plugin is gone: drop its entries and its load context.</summary>
    internal static void ForgetPlugin(string? plugin)
    {
        if (plugin == null) return;
        foreach (var key in Entries.Keys.Where(k => k.Plugin == plugin)) Entries.TryRemove(key, out _);
        foreach (var stale in PluginContexts.Where(p => p.Value == plugin).Select(p => p.Key)) PluginContexts.TryRemove(stale, out _);
    }

    internal static void NoteGamedataKey(string signature, string key) =>
        GamedataKeys.AddOrUpdate(signature, _ => [key], (_, keys) => keys.Add(key));

    /// <summary>Records how a signature resolved, if a plugin (not CounterStrikeSharp itself) resolved it.</summary>
    internal static void Record(string binaryPath, string signature, bool strict, int matchCount, PluginSignatureOutcome outcome)
    {
        if (CurrentPlugin() is not { } plugin) return;

        var module = Path.GetFileName(binaryPath);
        var keys = GamedataKeys.TryGetValue(signature, out var found) ? string.Join(", ", found) : null;
        Entries[(plugin, module, signature, strict)] =
            new PluginSignatureEntry(plugin, signature, keys, module, Math.Max(matchCount, 0), outcome, strict);
    }

    /// <summary>The plugin signatures block, or null when every one resolved to exactly one match.</summary>
    internal static string? FormatReport(IReadOnlyList<PluginSignatureEntry> entries)
    {
        var problems = entries.Where(e => !e.IsResolved).ToList();
        if (problems.Count == 0) return null;

        var text = new StringBuilder($"Plugin signatures: {problems.Count} of {entries.Count} ambiguous or failed:");
        foreach (var entry in problems) text.Append("\n  ").Append(entry);
        return text.ToString();
    }

    /// <summary>Logs the plugin signatures block as a warning, if any plugin signature is ambiguous or failed.</summary>
    internal static void LogReport(ILogger logger)
    {
        if (FormatReport(All) is { } report) logger.LogWarning("{PluginSignatures}", report);
    }
    internal static void Reset()
    {
        Entries.Clear();
        PluginContexts.Clear();
        GamedataKeys.Clear();
        AllPluginsLoaded = false;
    }

    // CounterStrikeSharp's own load context (the host may load it apart from the default one, where the runtime lives).
    private static readonly AssemblyLoadContext? CoreContext = AssemblyLoadContext.GetLoadContext(typeof(PluginSignatures).Assembly);

    // The first caller outside CounterStrikeSharp's and the runtime's load contexts: plugins load in their own.
    private static string? FindCallingPlugin()
    {
        foreach (var frame in new StackTrace(1, false).GetFrames())
        {
            var assembly = frame.GetMethod()?.DeclaringType?.Assembly;
            if (assembly == null || AssemblyLoadContext.GetLoadContext(assembly) is not { } context ||
                context == AssemblyLoadContext.Default || context == CoreContext) continue;

            return PluginContexts.TryGetValue(context, out var plugin) ? plugin : assembly.GetName().Name;
        }

        return null;
    }
}
