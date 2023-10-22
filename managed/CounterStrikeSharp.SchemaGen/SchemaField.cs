using System.Diagnostics.CodeAnalysis;

namespace CounterStrikeSharp.SchemaGen;

public record SchemaField(
    string Name,
    SchemaFieldType Type,
    IReadOnlyDictionary<string, string> Metadata)
{
    public bool TryGetMetadata(string name, [NotNullWhen(true)] out string? value) =>
        Metadata.TryGetValue(name, out value);
}
