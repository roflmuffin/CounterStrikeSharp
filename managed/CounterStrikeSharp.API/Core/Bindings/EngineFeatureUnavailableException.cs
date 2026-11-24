namespace CounterStrikeSharp.API.Core;

/// <summary>
/// Thrown when calling into an Engine Feature that is disabled on the running build because one of its Bindings
/// failed to resolve (ADR-0002). Check <see cref="EngineFeatures.IsAvailable(string)"/> first to degrade gracefully.
/// </summary>
public class EngineFeatureUnavailableException : InvalidOperationException
{
    /// <summary>Creates the exception for a disabled Engine Feature.</summary>
    public EngineFeatureUnavailableException(string engineFeature, string? binding, BindingFailureReason reason,
        int matchCount = 0)
        : base(binding == null
            ? $"Engine Feature {engineFeature} is not declared"
            : $"Engine Feature {engineFeature} is disabled ({binding}: {reason.Describe(matchCount)})")
    {
        EngineFeature = engineFeature;
        Binding = binding;
        Reason = reason;
        MatchCount = matchCount;
    }

    internal EngineFeatureUnavailableException(EngineFeatureStatus status)
        : this(status.Name, status.FailedBinding, status.Reason, status.MatchCount)
    {
    }

    /// <summary>The Engine Feature's name, e.g. <c>cssharp.chat/client-print</c>.</summary>
    public string EngineFeature { get; }

    /// <summary>The Gamedata key of the Binding that failed; null if the Engine Feature is not declared.</summary>
    public string? Binding { get; }

    /// <summary>Why the Binding failed.</summary>
    public BindingFailureReason Reason { get; }

    /// <summary>Signature matches of the failed Binding (relevant for <see cref="BindingFailureReason.Ambiguous"/>).</summary>
    public int MatchCount { get; }
}
