using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CodeGen.CsScript;

public static class Json
{

    public static readonly JsonSerializerOptions Compact = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    public static readonly JsonSerializerOptions Indented = new(Compact) { WriteIndented = true };
}

public sealed record Shape(string Kind, string? EnumName = null, string? Reason = null,
    string? EntityType = null, Shape? Element = null, List<Shape>? Alternatives = null,
    string? Name = null, List<Property>? Properties = null, object? Value = null, string? Absence = null)
{
    [JsonIgnore] public bool Scalar => Kind is "number" or "string" or "boolean" or "enum";
    [JsonIgnore] public int Slots => Kind switch
    {
        "vector" or "qangle" => Absence is null ? 3 : 4,
        "transform" => 13,
        _ => 1
    };
    [JsonIgnore] public bool Transportable => Scalar || Kind is "entity" or "vector" or "qangle" or "transform";
}
public sealed record Property(string Name, bool Optional, Shape Shape);
public sealed record Parameter(string Name, bool Optional, bool Rest, Shape Shape);
public sealed record Method(string Member, int Overload, string Documentation, bool Deprecated,
    List<Parameter> Parameters, string? DeclaringReceiver, Shape Returns,
    int MinimumArgumentCount, int MaximumArgumentCount, bool Supported);
public sealed record Receiver(string ExportName, string ReceiverType, string Kind, List<Method> Methods, List<string> Bases);
public sealed record EnumMember(string Name, long Value);
public sealed record EnumDefinition(string Name, string Documentation, bool Deprecated, List<EnumMember> Members);
public sealed record Diagnostic(string Receiver, string Reason, string? Member = null, int? Overload = null);
public sealed record Source(string GitBlobSha, string? Url = null);
public sealed record Manifest(int SchemaVersion, string Module, Source Source,
    List<EnumDefinition> Enums, List<Receiver> Receivers, List<Diagnostic> Diagnostics);
public sealed record Descriptor(uint Id, string Identity, Receiver Receiver, Method Method);
public sealed record Output(string CSharp, string Entities, string Native, List<Descriptor> Descriptors);
