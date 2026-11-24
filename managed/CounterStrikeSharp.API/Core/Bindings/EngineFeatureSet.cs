using System.Text;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core.Bindings;

/// <summary>
/// Engine Feature availability as a pure function of one <see cref="ResolutionReport"/> and the catalog (the native-owned
/// Engine Features carried in the report plus the managed ones). Immutable; every query, the typed exception, the
/// startup report and the helpers' address/offset lookups are derived from it. The process-wide instance is
/// <see cref="EngineFeatures.Current"/>; tests build their own from hand-built reports.
/// </summary>
internal sealed class EngineFeatureSet
{
    private readonly Dictionary<string, EngineFeatureDeclaration> _declarations = new();
    private readonly Dictionary<string, EngineFeatureStatus> _statuses = new();
    private readonly List<EngineFeatureStatus> _all;
    private readonly HashSet<(string Name, string What)> _warnedRegistrations = new();

    public EngineFeatureSet(ResolutionReport report, IEnumerable<EngineFeatureDeclaration> managedFeatures)
    {
        Report = report;

        // Names match by string across native and managed. If both sides declare one, it needs the Bindings of both.
        foreach (var feature in report.NativeFeatures.Concat(managedFeatures))
        {
            _declarations[feature.Name] = _declarations.TryGetValue(feature.Name, out var existing)
                ? existing with { Bindings = existing.Bindings.Concat(feature.Bindings).ToList() }
                : feature;
        }

        foreach (var feature in _declarations.Values)
        {
            _statuses[feature.Name] = Evaluate(feature);
        }

        _all = _statuses.Values.OrderBy(s => s.Name, StringComparer.Ordinal).ToList();
    }

    public ResolutionReport Report { get; }

    public string BuildId => Report.BuildId;

    /// <summary>Every declared Engine Feature with its status, ordered by name.</summary>
    public IReadOnlyList<EngineFeatureStatus> GetAll() => _all;

    public bool AnyDisabled => _all.Any(s => !s.IsAvailable);

    public bool IsAvailable(string name) => GetStatus(name).IsAvailable;

    public EngineFeatureStatus GetStatus(string name) =>
        _statuses.TryGetValue(name, out var status) ? status : EngineFeatureStatus.Unknown(name);

    /// <summary>Throws <see cref="EngineFeatureUnavailableException"/> unless the Engine Feature is available.</summary>
    public void ThrowIfUnavailable(string name)
    {
        var status = GetStatus(name);
        if (!status.IsAvailable) throw new EngineFeatureUnavailableException(status);
    }

    /// <summary>
    /// Gate for registering a hook or listener backed by an Engine Feature (ADR-0002): registration must still succeed
    /// (never throw out of plugin load), but a handler on a disabled Engine Feature never fires. Returns true when the
    /// Engine Feature is available. Otherwise logs one warning per Engine Feature and kind of registration
    /// (<paramref name="what"/>, e.g. "OnTakeDamage listener") and returns false; callers then keep the handler
    /// registered but skip installing anything native.
    /// </summary>
    public bool CheckRegistration(string name, string what, ILogger logger)
    {
        var status = GetStatus(name);
        if (status.IsAvailable) return true;

        lock (_warnedRegistrations)
        {
            if (!_warnedRegistrations.Add((name, what))) return false;
        }

        logger.LogWarning("{What} registered on {Status}; it will never fire", what, status);
        return false;
    }

    /// <summary>
    /// Address of a Signature (or RTTI vtable) Binding of <paramref name="feature"/>. Throws
    /// <see cref="EngineFeatureUnavailableException"/> if the Engine Feature is disabled, so a helper never calls through
    /// a Binding whose Engine Feature is off, even if that particular Binding resolved.
    /// </summary>
    public nint GetAddress(EngineFeatureDeclaration feature, string key) => GetBinding(feature, key).Address;

    /// <summary>Value of an Offset Binding of <paramref name="feature"/>; throws like <see cref="GetAddress"/>.</summary>
    public int GetOffset(EngineFeatureDeclaration feature, string key) => checked((int)GetBinding(feature, key).Offset);

    /// <summary>
    /// The startup report: one block with the build id, how many Engine Features are available, and each disabled one
    /// with its failed Binding and reason. Same wording as native ("Engine Feature X disabled (Key: reason)").
    /// </summary>
    public string FormatStartupReport()
    {
        var disabled = _all.Where(s => !s.IsAvailable).ToList();
        var header = $"Engine Features for build {BuildId}: ";
        if (disabled.Count == 0) return header + $"all {_all.Count} available";

        var text = new StringBuilder(header)
            .Append($"{_all.Count - disabled.Count} of {_all.Count} available, {disabled.Count} disabled:");
        foreach (var status in disabled)
        {
            text.Append("\n  ").Append(status);
        }

        return text.ToString();
    }

    private BindingResult GetBinding(EngineFeatureDeclaration feature, string key)
    {
        if (!feature.Bindings.Any(b => b.Key == key))
            throw new ArgumentException($"{key} is not a Binding of Engine Feature {feature.Name}", nameof(key));

        ThrowIfUnavailable(feature.Name);
        return Report.Find(key)!;
    }

    private EngineFeatureStatus Evaluate(EngineFeatureDeclaration feature)
    {
        foreach (var declaration in feature.Bindings)
        {
            var binding = Report.Find(declaration.Key);
            if (binding == null || (declaration.Kind != null && binding.Kind != declaration.Kind))
                return new EngineFeatureStatus(feature.Name, true, declaration.Key, BindingFailureReason.MissingFromGamedata, 0);
            if (!binding.Resolved)
                return new EngineFeatureStatus(feature.Name, true, binding.Key, binding.Reason, binding.MatchCount);
        }

        return new EngineFeatureStatus(feature.Name, true, null, BindingFailureReason.None, 0);
    }
}
