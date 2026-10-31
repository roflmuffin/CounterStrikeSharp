using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace CodeGen.CsScript;

public static class SchemaEnums
{
    public static IReadOnlyDictionary<string, string> Resolve(Manifest manifest, string directory)
    {
        var schemas = Directory.EnumerateFiles(directory, "*.g.cs")
            .Select(path => Read(File.ReadAllText(path))).OfType<SchemaEnum>().ToArray();
        var result = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var definition in manifest.Enums.Where(e => e.Members.Count > 0))
        {
            // Do not reuse unrelated enums just because they happen to have the same values.
            var matches = schemas.Where(s => TypeKey(s.Name) == TypeKey(definition.Name) && Matches(definition, s)).ToArray();
            if (matches.Length == 1)
                result.Add(definition.Name, $"global::CounterStrikeSharp.API.Core.{matches[0].Name}");
        }
        return result;
    }

    private static string TypeKey(string name)
    {
        if (name.StartsWith("CS", StringComparison.Ordinal)) name = name[2..];
        if (name.EndsWith("_t", StringComparison.Ordinal)) name = name[..^2];
        return name.Replace("_", "").ToUpperInvariant();
    }

    private static bool Matches(EnumDefinition definition, SchemaEnum schema)
    {
        if (definition.Members.Count != schema.Members.Count) return false;
        var first = definition.Members[0];
        // Schema generators commonly add a uniform prefix, e.g. WEAPONTYPE_ or
        // CUSTOM_CAMERA_MODE_. Require the entire member set and values to agree.
        foreach (var name in schema.Members.Keys.Where(n => n.EndsWith(first.Name, StringComparison.OrdinalIgnoreCase)))
        {
            var prefix = name[..^first.Name.Length];
            if (prefix.Length > 0 && !prefix.EndsWith('_')) continue;
            if (definition.Members.All(m => schema.Members.TryGetValue(prefix + m.Name, out var value) && value == m.Value))
                return true;
        }
        return false;
    }

    private sealed record SchemaEnum(string Name, Dictionary<string, BigInteger> Members);

    private static SchemaEnum? Read(string source)
    {
        // This intentionally accepts only the schema generator's literal-only format.
        // Unknown syntax must fall back to a script enum, never a partial match.
        var declaration = Regex.Match(source,
            @"\bnamespace CounterStrikeSharp\.API\.Core;\s+public enum (?<name>\w+) : (?<type>byte|ushort|uint|ulong)\s*\{(?<body>[^{}]*)\}\s*\z");
        if (!declaration.Success) return null;
        var members = new Dictionary<string, BigInteger>(StringComparer.OrdinalIgnoreCase);
        var entries = declaration.Groups["body"].Value.Trim().TrimEnd(',').Split(',');
        var bits = declaration.Groups["type"].Value switch { "byte" => 8, "ushort" => 16, "uint" => 32, _ => 64 };
        foreach (var entry in entries)
        {
            var member = Regex.Match(entry, @"\A\s*@?(?<name>[A-Za-z_][A-Za-z0-9_]*)\s*=\s*(?<value>0[xX][0-9a-fA-F]+|[0-9]+)\s*\z");
            if (!member.Success) return null;
            var literal = member.Groups["value"].Value;
            var value = literal.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                ? BigInteger.Parse("0" + literal[2..], NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture)
                : BigInteger.Parse(literal, CultureInfo.InvariantCulture);
            if (value >= BigInteger.One << bits || !members.TryAdd(member.Groups["name"].Value, value)) return null;
        }
        return new(declaration.Groups["name"].Value, members);
    }
}
