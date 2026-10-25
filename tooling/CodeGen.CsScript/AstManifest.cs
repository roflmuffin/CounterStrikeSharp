using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using TypeScriptParser;
using TypeScriptParser.TreeSitter;

namespace CodeGen.CsScript;

public sealed class AstManifest
{
    private readonly string source;
    private readonly Dictionary<string, TSNode> declarations = new(StringComparer.Ordinal);
    private readonly Dictionary<string, EnumDefinition> enums = new(StringComparer.Ordinal);
    private readonly Dictionary<string, double> enumValues = new(StringComparer.Ordinal);
    private readonly List<TSNode> exports = [];

    private AstManifest(string source) => this.source = source;
    public static Manifest Parse(string source, string? url = null)
    {
        source = source.Replace("\r\n", "\n");
        using var parser = new Parser();
        using var tree = parser.ParseString(source);
        var root = tree.root_node();
        var visitor = new AstManifest(source);
        if (root.has_error())
        {
            var error = Descendants(root).FirstOrDefault(n => n.type() == "ERROR" || n.is_missing());
            throw visitor.Error(error, "TypeScript syntax error");
        }
        return visitor.Read(root, url);
    }

    private static IEnumerable<TSNode> Children(TSNode node)
    {
        for (uint i = 0; i < node.named_child_count(); i++)
            if (node.named_child(i).type() != "comment") yield return node.named_child(i);
    }
    private static IEnumerable<TSNode> Descendants(TSNode node)
    {
        yield return node;
        foreach (var child in Children(node))
            foreach (var descendant in Descendants(child)) yield return descendant;
    }
    private string Text(TSNode node) => node.is_null() ? "" : node.text(source);
    private static TSNode Field(TSNode node, string field) => node.child_by_field_name(field);
    private string Name(TSNode node) => Text(Field(node, "name"));
    private InvalidDataException Error(TSNode node, string message) =>
        new($"point_script.d.ts:{node.start_point().row + 1}:{node.start_point().column + 1}: {message}: {Text(node)[..Math.Min(Text(node).Length, 120)]}");

    private Manifest Read(TSNode root, string? url)
    {
        var modules = Descendants(root).Where(n => n.type() == "module" && Text(Field(n, "name")).Trim('"', '\'') == "cs_script/point_script").ToArray();
        if (modules.Length != 1) throw new InvalidDataException("Expected one cs_script/point_script module");
        foreach (var statement in Children(Field(modules[0], "body")))
        {
            var exported = statement.type() == "export_statement";
            var node = exported ? Children(statement).Single() : statement;
            if (node.type() is "class_declaration" or "interface_declaration" or "type_alias_declaration" or "enum_declaration")
            {
                if (!declarations.TryAdd(Name(node), node)) throw Error(node, "Duplicate declaration");
            }
            else if (node.type() != "lexical_declaration") throw Error(node, "Unsupported module declaration");
            if (exported) exports.Add(node);
        }
        foreach (var node in declarations.Values.Where(n => n.type() == "enum_declaration")) ReadEnum(node);
        var receivers = new List<Receiver>();
        var diagnostics = new List<Diagnostic>();
        foreach (var node in exports)
        {
            if (node.type() == "class_declaration")
            {
                if (IsEntity(Name(node))) receivers.Add(ReadReceiver(Name(node), Name(node), true, diagnostics));
                else diagnostics.Add(new(Name(node), "non-entity class ownership"));
            }
            else if (node.type() == "lexical_declaration")
            {
                foreach (var variable in Children(node))
                {
                    var type = Children(Field(variable, "type")).Single();
                    if (type.type() != "type_identifier") throw Error(variable, "Expected a named export API type");
                    receivers.Add(ReadReceiver(Name(variable), Text(type), false, diagnostics));
                }
            }
        }
        if (receivers.Count == 0) throw new InvalidDataException("No API receivers found");
        var bytes = Encoding.UTF8.GetBytes(source);
        var blob = Encoding.UTF8.GetBytes($"blob {bytes.Length}\0").Concat(bytes).ToArray();
        return new(2, "cs_script/point_script", new(Convert.ToHexStringLower(SHA1.HashData(blob)), url),
            exports.Where(n => n.type() == "enum_declaration").Select(n => enums[Name(n)]).ToList(), receivers, diagnostics);
    }

    private (string Documentation, bool Deprecated) Documentation(TSNode node)
    {
        if (node.parent().type() == "export_statement") node = node.parent();
        var comment = node.prev_named_sibling();
        if (comment.is_null() || comment.type() != "comment" || !Text(comment).StartsWith("/**")) return ("", false);
        var lines = Text(comment)[3..^2].Split('\n').Select(x => Regex.Replace(x, @"^\s*\* ?", "").TrimEnd()).ToArray();
        return (string.Join('\n', lines.TakeWhile(x => !x.TrimStart().StartsWith('@'))).Trim(),
            lines.Any(x => Regex.IsMatch(x, @"(^|\s)@deprecated\b")));
    }

    private IEnumerable<string> Bases(TSNode node) => Children(node)
        .Where(n => n.type() is "class_heritage" or "extends_type_clause")
        .SelectMany(Descendants).Where(n => n.type() is "type_identifier" or "identifier").Select(Text);

    private bool IsEntity(string name, HashSet<string>? path = null)
    {
        if (name == "Entity") return true;
        if (!declarations.TryGetValue(name, out var node)) return false;
        path = path is null ? [] : new(path);
        if (!path.Add(name)) throw Error(node, "Cyclic inheritance");
        return Bases(node).Any(parent => IsEntity(parent, path));
    }

    private List<(string Owner, TSNode Node)> Members(string name, HashSet<string>? path = null)
    {
        if (!declarations.TryGetValue(name, out var node)) throw new InvalidDataException($"Unknown receiver type: {name}");
        path = path is null ? [] : new(path);
        if (!path.Add(name)) throw Error(node, "Cyclic inheritance");
        var own = Children(Field(node, "body")).Select(n => (Owner: name, Node: n)).ToList();
        var names = own.Select(m => Name(m.Node)).ToHashSet(StringComparer.Ordinal);
        foreach (var parent in Bases(node))
            own.AddRange(Members(parent, path).Where(m => !names.Contains(Name(m.Node))));
        return own;
    }

    private Receiver ReadReceiver(string name, string type, bool entity, List<Diagnostic> diagnostics)
    {
        var methods = new List<Method>();
        foreach (var group in Members(type).GroupBy(m => Name(m.Node)))
        {
            var docs = group.Select(m => Documentation(m.Node)).ToArray();
            var documentation = string.Join('\n', docs.Select(d => d.Documentation).Where(d => d.Length > 0));
            var overload = 0;
            foreach (var (owner, member) in group)
            {
                if (member.type() != "method_signature")
                {
                    diagnostics.Add(new(name, "property access", group.Key));
                    continue;
                }
                var deprecated = Documentation(member).Deprecated;
                var generic = !Field(member, "type_parameters").is_null();
                var parameters = Children(Field(member, "parameters")).Select(p =>
                {
                    var pattern = Field(p, "pattern");
                    var rest = pattern.type() == "rest_pattern";
                    var optional = p.type() == "optional_parameter";
                    var shape = Resolve(Field(p, "type"));

                    if (optional && shape.Kind == "boolean") shape = new("union", Alternatives:
                        [new("undefined"), new("literal", Value: false), new("literal", Value: true)]);
                    if (optional && shape.Kind == "union")
                    {
                        var present = shape.Alternatives!.Where(s => s.Kind != "undefined").ToList();
                        if (present.Count == 1) shape = present[0];
                    }
                    if (optional && shape.Kind == "entity")
                        shape = shape with { Absence = shape.Absence is "null" or "null|undefined" ? "null|undefined" : "undefined" };
                    return new Parameter(rest ? Text(Children(pattern).Single()) : Text(pattern), optional, rest, shape);
                }).ToList();
                var returns = Resolve(Field(member, "return_type"));
                var supported = !generic && parameters.All(p => !p.Rest && p.Shape.Transportable) && (returns.Transportable || returns.Kind == "void");
                var minimum = parameters.FindLastIndex(p => !p.Optional && !p.Rest) + 1;
                methods.Add(new(group.Key, overload, documentation, deprecated, parameters, entity ? owner : null,
                    returns, minimum, parameters.Count, supported));
                if (!supported) diagnostics.Add(new(name, generic ? "generic" :
                    "shape requires additional marshalling/ownership support", group.Key, overload));
                overload++;
            }
        }
        return new(name, type, entity ? "entity" : "export", methods, Bases(declarations[type]).ToList());
    }

    private Shape Resolve(TSNode node, HashSet<string>? seen = null)
    {
        if (node.is_null()) throw new InvalidDataException("Missing explicit type annotation");
        seen = seen is null ? [] : new(seen);
        switch (node.type())
        {
            case "type_annotation": case "parenthesized_type": return Resolve(Children(node).Single(), seen);
            case "predefined_type":
                return Text(node) is "number" or "string" or "boolean" or "void" or "undefined" or "null"
                    ? new(Text(node)) : new("unsupported", Reason: Text(node));
            case "type_identifier":
                var name = Text(node);
                if (enums.ContainsKey(name)) return new("enum", EnumName: name);
                if (IsEntity(name)) return new("entity", EntityType: name);
                if (!declarations.TryGetValue(name, out var declaration)) return new("unsupported", Reason: $"unresolved type: {name}");
                if (!seen.Add(name)) return new("unsupported", Reason: "recursive shape");
                if (declaration.type() == "type_alias_declaration") return Resolve(Field(declaration, "value"), seen);
                return new("object", Name: name, Properties: Members(name).Where(m => m.Node.type() == "property_signature")
                    .Select(m => ReadProperty(m.Node, seen)).ToList());
            case "generic_type":
                return new("unsupported", Reason: Text(Field(node, "name")) == "Promise" ? "Promise lifetime" : Text(node));
            case "function_type": return new("unsupported", Reason: "function ownership");
            case "array_type": return new("array", Element: Resolve(Children(node).Single(), seen));
            case "union_type":
                var alternatives = Children(node).Select(n => Resolve(n, seen))
                    .SelectMany(s => s.Kind == "union" ? s.Alternatives! : [s]).ToList();
                if (alternatives.All(s => s.Kind == "literal" && s.Value is bool) && alternatives.Select(s => s.Value).Distinct().Count() == 2) return new("boolean");
                var entityTypes = alternatives.Where(s => s.Kind == "entity").ToList();
                if (entityTypes.Count == 1 && alternatives.All(s => s.Kind is "entity" or "undefined" or "null"))
                {
                    var absent = alternatives.Where(s => s.Kind != "entity").Select(s => s.Kind)
                        .Concat((entityTypes[0].Absence ?? "").Split('|', StringSplitOptions.RemoveEmptyEntries))
                        .Distinct().Order(StringComparer.Ordinal);
                    return entityTypes[0] with { Absence = string.Join('|', absent) };
                }
                return new("union", Alternatives: alternatives);
            case "intersection_type":
                var parts = Children(node).Select(n => Resolve(n, seen)).ToList();
                return parts.All(s => s.Kind == "object") ? new("object", Properties: parts.SelectMany(s => s.Properties!).ToList())
                    : new("unsupported", Reason: Text(node));
            case "object_type":
                if (Children(node).Any(n => n.type() != "property_signature")) return new("unsupported", Reason: Text(node));
                return new("object", Properties: Children(node).Select(n => ReadProperty(n, seen)).ToList());
            case "literal_type":
                var literal = Text(node);
                if (literal is "null" or "undefined") return new(literal);
                if (literal is "true" or "false") return new("literal", Value: literal == "true");
                return new("literal", Value: literal);
            default: return new("unsupported", Reason: $"{node.type()}: {Text(node)}");
        }
    }

    private Property ReadProperty(TSNode node, HashSet<string> seen)
    {
        var optional = Enumerable.Range(0, (int)node.child_count()).Any(i => node.child((uint)i).type() == "?");
        return new(Name(node), optional, Resolve(Field(node, "type"), seen));
    }

    private void ReadEnum(TSNode node)
    {
        var members = new List<EnumMember>();
        double next = 0;
        foreach (var member in Children(Field(node, "body")))
        {
            var name = member.type() == "enum_assignment" ? Name(member) : Text(member);
            var value = member.type() == "enum_assignment" ? Evaluate(Field(member, "value")) : next;
            if (!double.IsFinite(value) || Math.Truncate(value) != value || Math.Abs(value) > 9007199254740991d)
                throw Error(member, "Enum value is not a safe integer");
            enumValues[name] = enumValues[$"{Name(node)}.{name}"] = value;
            members.Add(new(name, (long)value));
            next = value + 1;
        }
        var (doc, deprecated) = Documentation(node);
        enums.Add(Name(node), new(Name(node), doc, deprecated, members));
    }

    private double Evaluate(TSNode node)
    {
        switch (node.type())
        {
            case "number":
                var text = Text(node).Replace("_", "");
                if (text.StartsWith("0x", StringComparison.OrdinalIgnoreCase)) return Convert.ToInt64(text[2..], 16);
                if (text.StartsWith("0b", StringComparison.OrdinalIgnoreCase)) return Convert.ToInt64(text[2..], 2);
                if (text.StartsWith("0o", StringComparison.OrdinalIgnoreCase)) return Convert.ToInt64(text[2..], 8);
                return double.Parse(text, CultureInfo.InvariantCulture);
            case "identifier": case "member_expression":
                return enumValues.TryGetValue(Text(node), out var value) ? value : throw Error(node, "Unknown enum constant");
            case "parenthesized_expression": return Evaluate(Children(node).Single());
            case "unary_expression":
                var operand = Evaluate(Field(node, "argument"));
                return Text(Field(node, "operator")) switch { "+" => operand, "-" => -operand, "~" => ~ToInt32(operand), _ => throw Error(node, "Unsupported enum operator") };
            case "binary_expression":
                var left = Evaluate(Field(node, "left"));
                var right = Evaluate(Field(node, "right"));
                return Text(Field(node, "operator")) switch
                {
                    "+" => left + right, "-" => left - right, "*" => left * right, "/" => left / right, "%" => left % right,
                    "**" => Math.Pow(left, right), "<<" => ToInt32(left) << ToInt32(right), ">>" => ToInt32(left) >> ToInt32(right),
                    ">>>" => (uint)ToInt32(left) >> ToInt32(right), "|" => ToInt32(left) | ToInt32(right),
                    "&" => ToInt32(left) & ToInt32(right), "^" => ToInt32(left) ^ ToInt32(right),
                    _ => throw Error(node, "Unsupported enum operator")
                };
            default: throw Error(node, "Unsupported enum initializer");
        }
    }

    private static int ToInt32(double value) => double.IsFinite(value)
        ? unchecked((int)(uint)(long)(Math.Truncate(value) % 4294967296d)) : 0;
}
