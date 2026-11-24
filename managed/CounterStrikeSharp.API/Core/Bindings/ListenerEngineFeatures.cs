namespace CounterStrikeSharp.API.Core.Bindings;

/// <summary>
/// Which Engine Feature's hooks fire a global listener (by its <see cref="Attributes.ListenerNameAttribute"/> name).
/// <see cref="BasePlugin.RegisterListener{T}"/> gates on it: a listener on a disabled Engine Feature stays registered,
/// logs once and never fires (ADR-0002). Listeners not listed here do not depend on a Binding.
/// </summary>
internal static class ListenerEngineFeatures
{
    private static readonly Dictionary<string, string> Features = new()
    {
        // ticket 09
        ["CheckTransmit"] = "cssharp.network/check-transmit",
    };

    /// <summary>The Engine Feature backing the listener, or null if it needs none.</summary>
    public static string? For(string listenerName) => Features.GetValueOrDefault(listenerName);
}
