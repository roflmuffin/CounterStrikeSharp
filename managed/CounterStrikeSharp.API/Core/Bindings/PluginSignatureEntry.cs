namespace CounterStrikeSharp.API.Core;

/// <summary>How a signature a plugin passed in (directly, or from a Gamedata file it ships) resolved.</summary>
public enum PluginSignatureOutcome
{
    /// <summary>Matched exactly once.</summary>
    Resolved,

    /// <summary>Matched more than once; the first match is used.</summary>
    Ambiguous,

    /// <summary>Matched more than once and the plugin asked for strict mode, so it failed.</summary>
    AmbiguousRejected,

    /// <summary>Matched nowhere (or its module is not loaded).</summary>
    NotFound,

    /// <summary>Not a valid signature pattern.</summary>
    InvalidPattern,

    /// <summary>Matched, but no function could be created from it.</summary>
    Unbound,
}

/// <summary>
/// One signature a plugin resolved: which plugin, the signature (and its Gamedata key if it came from one), the module,
/// how often it matched and the outcome. Plugin signatures are reported next to the Engine Features but never disable
/// them. Read them with <see cref="EngineFeatures.GetPluginSignatures"/>.
/// </summary>
/// <param name="Plugin">The plugin's assembly name, e.g. <c>MyPlugin</c>.</param>
/// <param name="Signature">The signature as passed in.</param>
/// <param name="GamedataKey">The Gamedata key the signature was read from with <see cref="GameData.GetSignature"/>; null for a raw signature.</param>
/// <param name="Module">The module file searched, e.g. <c>libserver.so</c>.</param>
/// <param name="MatchCount">Matches in the module; 0 when not found, invalid or not counted.</param>
/// <param name="Outcome">How it resolved.</param>
public sealed record PluginSignatureEntry(
    string Plugin,
    string Signature,
    string? GamedataKey,
    string Module,
    int MatchCount,
    PluginSignatureOutcome Outcome)
{
    /// <summary>True when the signature resolved to exactly one match.</summary>
    public bool IsUnique => Outcome == PluginSignatureOutcome.Resolved;

    /// <summary>
    /// The outcome in the report's wording: <c>"resolved"</c>, <c>"ambiguous (3 matches), first match used"</c>,
    /// <c>"ambiguous (3 matches), rejected (strict)"</c>, <c>"not found"</c>, <c>"invalid pattern"</c> or
    /// <c>"could not be bound"</c>.
    /// </summary>
    public string Describe() => Outcome switch
    {
        PluginSignatureOutcome.Resolved => "resolved",
        PluginSignatureOutcome.Ambiguous => $"ambiguous ({MatchCount} matches), first match used",
        PluginSignatureOutcome.AmbiguousRejected => $"ambiguous ({MatchCount} matches), rejected (strict)",
        PluginSignatureOutcome.NotFound => "not found",
        PluginSignatureOutcome.InvalidPattern => "invalid pattern",
        _ => "could not be bound",
    };

    /// <summary>E.g. <c>MyPlugin: MyKey "55 48 89 E5" in libserver.so: ambiguous (3 matches), first match used</c>.</summary>
    public override string ToString() =>
        $"{Plugin}: {(GamedataKey != null ? GamedataKey + " " : "")}\"{Signature}\" in {Module}: {Describe()}";
}
