namespace CounterStrikeSharp.API.Core;

/// <summary>
/// A Heal (ADR-0003): a Slot Binding whose method moved in its class's vtable, found at boot at a new index because its
/// fingerprint window matched exactly once nearby. The Engine Feature using it stays available; the server runs on a
/// location Gamedata does not state. Read from <see cref="EngineFeatureStatus.Heals"/>.
/// </summary>
public sealed class SlotHeal
{
    internal SlotHeal(string binding, string className, string library, int oldIndex, int newIndex, IReadOnlyList<string?> window)
    {
        Binding = binding;
        ClassName = className;
        Library = library;
        OldIndex = oldIndex;
        NewIndex = newIndex;
        Window = window;
    }

    /// <summary>The Slot Binding's Gamedata key, e.g. <c>ISource2Server::GameFrame</c>.</summary>
    public string Binding { get; }

    /// <summary>The RTTI class whose vtable holds the slot, e.g. <c>CSource2Server</c>.</summary>
    public string ClassName { get; }

    /// <summary>The module the class lives in, named as in Gamedata (<c>server</c>, <c>engine</c>, <c>tier0</c>).</summary>
    public string Library { get; }

    /// <summary>The slot index Gamedata states.</summary>
    public int OldIndex { get; }

    /// <summary>The slot index the method was found at, and is used at.</summary>
    public int NewIndex { get; }

    /// <summary>
    /// The evidence: the fingerprints Gamedata recorded for the slot and its neighbours (index - 1, index, index + 1;
    /// null for a neighbour outside the vtable), which matched the live vtable at <see cref="NewIndex"/> and nowhere else
    /// nearby.
    /// </summary>
    public IReadOnlyList<string?> Window { get; }

    /// <inheritdoc />
    public override string ToString() => $"{Binding}: slot {OldIndex} -> {NewIndex}";
}
