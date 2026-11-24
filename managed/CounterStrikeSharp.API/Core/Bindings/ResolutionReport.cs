using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CounterStrikeSharp.API.Core.Bindings;

/// <summary>How a Binding is found (mirrors <c>bindings::BindingKind</c> in native).</summary>
internal enum BindingKind
{
    Signature,
    Offset,
    RttiVtable,
}

/// <summary>One Binding in the Resolution Report: resolved (with its address or offset) or failed (with a reason).</summary>
internal sealed record BindingResult(
    string Key,
    BindingKind Kind,
    bool Resolved,
    BindingFailureReason Reason,
    int MatchCount,
    nint Address,
    long Offset,
    string Library);

/// <summary>
/// The immutable Resolution Report for the running build (GLOSSARY.md): the build id, every core Binding as native
/// resolved it at boot, and the native-owned Engine Features (name + Binding keys). Native keeps the authoritative
/// copy; managed reads it once through <see cref="FromJson"/>. Tests build it by hand.
/// </summary>
internal sealed class ResolutionReport
{
    private readonly Dictionary<string, BindingResult> _bindings;

    public ResolutionReport(string buildId, IEnumerable<BindingResult> bindings,
        IEnumerable<EngineFeatureDeclaration> nativeFeatures)
    {
        BuildId = buildId;
        Bindings = bindings.ToList();
        NativeFeatures = nativeFeatures.ToList();
        _bindings = new Dictionary<string, BindingResult>();
        foreach (var binding in Bindings) _bindings.TryAdd(binding.Key, binding);
    }

    public string BuildId { get; }

    public IReadOnlyList<BindingResult> Bindings { get; }

    /// <summary>Engine Features declared by native code; their Bindings are keys into this report.</summary>
    public IReadOnlyList<EngineFeatureDeclaration> NativeFeatures { get; }

    /// <summary>null when native did not resolve the key (it is not in core Gamedata and no native feature declares it).</summary>
    public BindingResult? Find(string key) => _bindings.GetValueOrDefault(key);

    /// <summary>Reads the document written by native <c>bindings::ResolutionReportToJson</c>.</summary>
    public static ResolutionReport FromJson(string json)
    {
        var dto = JsonSerializer.Deserialize<ReportDto>(json, JsonOptions)
                  ?? throw new JsonException("Resolution Report is empty");

        return new ResolutionReport(
            dto.BuildId,
            dto.Bindings.Select(b => new BindingResult(b.Key, b.Kind, b.Resolved, b.Reason, checked((int)b.MatchCount),
                unchecked((nint)b.Address), b.Offset, b.Library)),
            dto.Features.Select(f => new EngineFeatureDeclaration(f.Name, f.Bindings.Select(BindingDeclaration.Any).ToList())));
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() },
    };

    private sealed record ReportDto(string BuildId, List<BindingDto> Bindings, List<FeatureDto> Features);

    private sealed record BindingDto(
        string Key,
        BindingKind Kind,
        bool Resolved,
        BindingFailureReason Reason,
        ulong MatchCount,
        ulong Address,
        long Offset,
        string Library);

    private sealed record FeatureDto(string Name, List<string> Bindings);
}
