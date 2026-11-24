using System.Collections.Generic;

namespace CounterStrikeSharp.API.Core.Bindings;

/// <summary>
/// A Binding an Engine Feature needs: a core Gamedata key and the kind of entry it must be. Native resolves every core
/// Gamedata key at boot by its section ("signatures" or "offsets"); a key that is absent, or present as the other kind,
/// counts as missing from Gamedata.
/// </summary>
/// <param name="Kind">null for Bindings of native-owned Engine Features: native declared and resolved them itself.</param>
internal sealed record BindingDeclaration(string Key, BindingKind? Kind)
{
    public static BindingDeclaration Signature(string key) => new(key, BindingKind.Signature);

    public static BindingDeclaration Offset(string key) => new(key, BindingKind.Offset);

    internal static BindingDeclaration Any(string key) => new(key, null);
}

/// <summary>
/// An Engine Feature: its permanent, namespaced name (never renamed or reused) and the Bindings it needs. It is available
/// exactly when every Binding resolved. Managed-consumed ones are declared in <see cref="ManagedEngineFeatures"/>.
/// </summary>
internal sealed record EngineFeatureDeclaration(string Name, IReadOnlyList<BindingDeclaration> Bindings);
