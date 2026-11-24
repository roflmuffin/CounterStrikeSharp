using System;
using System.Collections.Generic;
using System.Linq;

namespace CounterStrikeSharp.API.Core.Attributes;

/// <summary>
/// Declares Engine Features this plugin cannot run without (ADR-0002). At plugin load, if any of them is disabled on
/// the running CS2 build (or unknown to this CounterStrikeSharp version), the plugin is refused with a message naming
/// the Engine Feature, the failed Binding and the reason. Plugins that declare nothing load as before.
/// May be repeated.
/// </summary>
/// <example><c>[RequiredEngineFeatures("cssharp.chat/say-hooks")]</c> or <c>[RequiredEngineFeatures(EngineFeature.ChatSayHooks)]</c></example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class RequiredEngineFeatures : System.Attribute
{
    /// <summary>The stable string names of the required Engine Features.</summary>
    public IReadOnlyList<string> Names { get; }

    /// <param name="names">Engine Feature names, e.g. <c>cssharp.chat/say-hooks</c>.</param>
    public RequiredEngineFeatures(params string[] names)
    {
        Names = names;
    }

    /// <param name="features">Engine Features by their generated enum value.</param>
    public RequiredEngineFeatures(params EngineFeature[] features)
    {
        // A value this version does not know (plugin built against a newer CounterStrikeSharp) stays unknown.
        Names = features.Select(f => f.GetName() ?? $"#{(int)f}").ToArray();
    }
}
