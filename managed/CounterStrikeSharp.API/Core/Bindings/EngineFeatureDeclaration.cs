using System.Collections.Generic;

namespace CounterStrikeSharp.API.Core.Bindings;

/// <summary>
/// A Binding an Engine Feature needs: a core Gamedata key and the kind of entry it must be. Native resolves every core
/// Gamedata key at boot by its section ("signatures" or "offsets"; game-class virtuals and keys with "vtable" as Slot
/// Bindings); a key that is absent, or present as another kind, counts as missing from Gamedata.
/// </summary>
/// <param name="Kind">null for Bindings of native-owned Engine Features: native declared and resolved them itself.</param>
internal sealed record BindingDeclaration(string Key, BindingKind? Kind)
{
    public static BindingDeclaration Signature(string key) => new(key, BindingKind.Signature);

    public static BindingDeclaration Offset(string key) => new(key, BindingKind.Offset);

    /// <summary>
    /// A Slot Binding (ADR-0003): a vtable slot index from Gamedata, verified at boot against the live vtable, healed if
    /// it moved. Its offset in the report is the index to call (the healed one after a Heal).
    /// </summary>
    public static BindingDeclaration Slot(string key) => new(key, BindingKind.Slot);

    internal static BindingDeclaration Any(string key) => new(key, null);
}

/// <summary>
/// An Engine Feature: its permanent, namespaced name (never renamed or reused) and the Bindings it needs. It is available
/// exactly when every Binding resolved. Managed-consumed ones are declared in <see cref="ManagedEngineFeatures"/>.
/// </summary>
internal sealed record EngineFeatureDeclaration(string Name, IReadOnlyList<BindingDeclaration> Bindings);
