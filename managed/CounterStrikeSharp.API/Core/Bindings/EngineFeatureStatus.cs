namespace CounterStrikeSharp.API.Core;

/// <summary>
/// Whether an Engine Feature is available on the running build, and if not, which Binding failed and why.
/// Obtain one from <see cref="EngineFeatures.GetStatus(string)"/> or <see cref="EngineFeatures.GetAll"/>.
/// </summary>
public sealed class EngineFeatureStatus
{
    internal EngineFeatureStatus(string name, bool isDeclared, string? failedBinding, BindingFailureReason reason,
        int matchCount)
    {
        Name = name;
        IsDeclared = isDeclared;
        FailedBinding = failedBinding;
        Reason = reason;
        MatchCount = matchCount;
    }

    internal static EngineFeatureStatus Unknown(string name) => new(name, false, null, BindingFailureReason.None, 0);

    /// <summary>The Engine Feature's stable, namespaced name, e.g. <c>cssharp.chat/say-hooks</c>.</summary>
    public string Name { get; }

    /// <summary>False if this CounterStrikeSharp version does not declare an Engine Feature by that name.</summary>
    public bool IsDeclared { get; }

    /// <summary>True if the Engine Feature is declared and every Binding it needs resolved.</summary>
    public bool IsAvailable => IsDeclared && FailedBinding == null;

    /// <summary>The (first) Binding that failed, as its Gamedata key; null when available or not declared.</summary>
    public string? FailedBinding { get; }

    /// <summary>Why <see cref="FailedBinding"/> failed; <see cref="BindingFailureReason.None"/> when available.</summary>
    public BindingFailureReason Reason { get; }

    /// <summary>Signature matches of <see cref="FailedBinding"/> (relevant for <see cref="BindingFailureReason.Ambiguous"/>).</summary>
    public int MatchCount { get; }

    /// <summary>
    /// Why not, in the wording of the startup report: <c>"Host_Say: not found"</c>,
    /// <c>"ClientPrint: ambiguous (2 matches)"</c>, <c>"available"</c> or <c>"unknown Engine Feature"</c>.
    /// </summary>
    public string Describe()
    {
        if (!IsDeclared) return "unknown Engine Feature";
        if (IsAvailable) return "available";
        return $"{FailedBinding}: {Reason.Describe(MatchCount)}";
    }

    /// <inheritdoc />
    public override string ToString() => IsAvailable ? $"{Name} available" : $"{Name} disabled ({Describe()})";
}
