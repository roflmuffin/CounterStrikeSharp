using System;
using System.Linq;
using System.Reflection;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Bindings;

namespace CounterStrikeSharp.API.Core.Plugin;

/// <summary>
/// The plugin load gate for <see cref="RequiredEngineFeatures"/> (ADR-0002): a pure function of the plugin type's
/// declarations and an <see cref="EngineFeatureSet"/>, so it is testable without a plugin loader or native.
/// </summary>
internal static class RequiredEngineFeatureCheck
{
    /// <summary>Whether <paramref name="pluginType"/> declares any Required Engine Features.</summary>
    public static bool DeclaresAny(Type pluginType) => pluginType.GetCustomAttributes<RequiredEngineFeatures>().Any();

    /// <summary>
    /// Null if every Required Engine Feature of <paramref name="pluginType"/> is available (or it declares none);
    /// otherwise the refusal message, naming each unavailable one with its failed Binding and reason.
    /// </summary>
    public static string? GetRefusal(Type pluginType, string pluginFileName, EngineFeatureSet features)
    {
        var unavailable = pluginType.GetCustomAttributes<RequiredEngineFeatures>()
            .SelectMany(a => a.Names)
            .Distinct(StringComparer.Ordinal)
            .Select(features.GetStatus)
            .Where(s => !s.IsAvailable)
            .Select(s => s.IsDeclared
                ? s.ToString()
                : $"{s.Name} is not an Engine Feature of this CounterStrikeSharp version")
            .ToList();

        if (unavailable.Count == 0) return null;

        return $"Plugin \"{pluginFileName}\" requires Engine Features that are not available on build {features.BuildId}: " +
               string.Join("; ", unavailable) + ".";
    }
}
