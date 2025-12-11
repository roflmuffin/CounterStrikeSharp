/**
 * This project has been copied & modified from the demofile-net project under the MIT license.
 * See ACKNOWLEDGEMENTS file for more information.
 * https://github.com/saul/demofile-net
 */

using System.Collections.Immutable;
using System.Text;
using System.Text.Json;
using QuickGraph;
using QuickGraph.Algorithms.Search;

namespace CounterStrikeSharp.SchemaGen;

internal static partial class Program
{
    private static readonly IReadOnlySet<string> IgnoreClasses = new HashSet<string>
    {
        "GameTime_t",
        "GameTick_t",
        "AttachmentHandle_t",
        "CGameSceneNodeHandle",
        "HSequence",
        "CAttributeManager::cached_attribute_float_t",
        "QuestProgress::Reason",
        "IChoreoServices::ScriptState_t",
        "IChoreoServices::ChoreoState_t",
        "SpawnPointCoopEnemy::BotDefaultBehavior_t",
        "CLogicBranchList::LogicBranchListenerLastState_t",
        "SimpleConstraintSoundProfile::SimpleConstraintsSoundProfileKeypoints_t",
        "MoodAnimationLayer_t",
        "SoundeventPathCornerPairNetworked_t",
        "AISound_t",
        "CAttachmentNameSymbolWithStorage"
    };

    private static readonly IReadOnlySet<string> IgnoreClassWildcards = new HashSet<string>
    {
        "CResourceNameTyped",
        "CEntityOutputTemplate",
        "CVariantBase",
        "HSCRIPT",
        "KeyValues3",
        "Unknown"
    };

    public static string SanitiseTypeName(string typeName) =>
        typeName.Replace(":", "")
            .Replace("< ", "<")
            .Replace(" >", ">");

    private static (Dictionary<string, SchemaEnum>, Dictionary<string, SchemaClass>) ConvertNewSchemaToOld(NewSchemaModule newSchema)
    {
        var enums = new Dictionary<string, SchemaEnum>();
        var classes = new Dictionary<string, SchemaClass>();

        var defLookup = newSchema.defs.Select((def, idx) => new { def, idx }).ToDictionary(x => x.idx, x => x.def);

        for (int i = 0; i < newSchema.defs.Length; i++)
        {
            var def = newSchema.defs[i];
            if (def.type == "enum" && def.traits?.fields != null)
            {
                var enumItems = def.traits.fields.Select(f => new SchemaEnumItem(f.name, f.value)).ToList();
                enums[def.name] = new SchemaEnum(def.alignment ?? 4, enumItems);
            }
        }

        for (int i = 0; i < newSchema.defs.Length; i++)
        {
            var def = newSchema.defs[i];
            if (def.type == "class" && def.traits != null)
            {
                string? parentName = null;

                if (def.traits.baseclasses != null && def.traits.baseclasses.Length > 0)
                {
                    var parentIdx = def.traits.baseclasses[0].ref_idx;
                    if (defLookup.TryGetValue(parentIdx, out var parentDef))
                    {
                        parentName = parentDef.name;
                    }
                }

                var fields = new List<SchemaField>();

                if (def.traits.members != null)
                {
                    foreach (var member in def.traits.members)
                    {
                        if (member.traits?.subtype != null)
                        {
                            var fieldType = ConvertSubtypeToFieldType(member.traits.subtype, defLookup);
                            var metadata = member.traits.metatags?.ToDictionary(m => m.name, m => m.value ?? "") ??
                                           new Dictionary<string, string>();
                            fields.Add(new SchemaField(member.name, fieldType, metadata));
                        }
                    }
                }

                classes[def.name] = new SchemaClass(i, def.name, parentName, fields);
            }
        }

        return (enums, classes);
    }

    private static SchemaFieldType ConvertSubtypeToFieldType(SchemaSubtype subtype, Dictionary<int, SchemaDef> defLookup)
    {
        if (subtype.type == "ref" && subtype.ref_idx.HasValue)
        {
            if (defLookup.TryGetValue(subtype.ref_idx.Value, out var referencedDef))
            {
                return ConvertSubtypeToFieldType(new SchemaSubtype(
                    referencedDef.type == "class"
                        ? "declared_class"
                        : (referencedDef.type == "enum" ? "declared_enum" : referencedDef.type),
                    referencedDef.name,
                    referencedDef.size,
                    referencedDef.alignment,
                    null,
                    null,
                    null,
                    null,
                    null
                ), defLookup);
            }
        }

        SchemaTypeCategory category = subtype.type switch
        {
            "builtin" => SchemaTypeCategory.Builtin,
            "atomic" => SchemaTypeCategory.Atomic,
            "ptr" => SchemaTypeCategory.Ptr,
            "fixed_array" => SchemaTypeCategory.FixedArray,
            "declared_class" => SchemaTypeCategory.DeclaredClass,
            "declared_enum" => SchemaTypeCategory.DeclaredEnum,
            "bitfield" => SchemaTypeCategory.Bitfield,
            _ => SchemaTypeCategory.None
        };

        SchemaAtomicCategory? atomic = null;
        if (subtype.type == "atomic" && subtype.name != null)
        {
            if (subtype.name.Contains("CUtlVector") || subtype.name.Contains("CNetworkUtlVectorBase"))
            {
                atomic = SchemaAtomicCategory.Collection;
            }
            else if (subtype.name.Contains("CHandle") || subtype.name.Contains("CWeakHandle"))
            {
                atomic = SchemaAtomicCategory.T;
            }
            else
            {
                atomic = SchemaAtomicCategory.Basic;
            }
        }

        SchemaFieldType? innerType = null;

        if (subtype.template != null && subtype.template.Length > 0)
        {
            innerType = ConvertSubtypeToFieldType(subtype.template[0], defLookup);
        }
        else if (subtype.subtype != null)
        {
            innerType = ConvertSubtypeToFieldType(subtype.subtype, defLookup);
        }

        string typeName = subtype.name ?? "unknown";
        if (category == SchemaTypeCategory.FixedArray && subtype.count.HasValue && innerType != null)
        {
            typeName = $"{innerType.Name}[{subtype.count.Value}]";
        }

        return new SchemaFieldType(
            typeName,
            category,
            atomic,
            innerType
        );
    }

    private static StringBuilder GetTemplate(bool includeUsings)
    {
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated />");
        builder.AppendLine("#nullable enable");
        builder.AppendLine("#pragma warning disable CS1591");
        builder.AppendLine();
        builder.AppendLine("using System;");

        if (includeUsings)
        {
            builder.AppendLine("using System.Diagnostics;");
            builder.AppendLine("using System.Drawing;");
            builder.AppendLine("using CounterStrikeSharp;");
            builder.AppendLine("using CounterStrikeSharp.API.Modules.Events;");
            builder.AppendLine("using CounterStrikeSharp.API.Modules.Entities;");
            builder.AppendLine("using CounterStrikeSharp.API.Modules.Memory;");
            builder.AppendLine("using CounterStrikeSharp.API.Modules.Utils;");
            builder.AppendLine("using CounterStrikeSharp.API.Core.Attributes;");
        }

        builder.AppendLine();
        builder.AppendLine("namespace CounterStrikeSharp.API.Core;");

        return builder;
    }

    public static void Main(string[] args)
    {
        var outputPath =
            args.FirstOrDefault() ??
            "../CounterStrikeSharp.API/Generated/Schema";

        // Concat together all enums and classes
        var allEnums = new SortedDictionary<string, SchemaEnum>();
        var allClasses = new SortedDictionary<string, SchemaClass>();

        var schemaFiles = new[] { "server.json" };

        foreach (var schemaFile in schemaFiles)
        {
            var schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Schema", schemaFile);

            var newSchema = JsonSerializer.Deserialize<NewSchemaModule>(
                File.ReadAllText(schemaPath),
                SerializerOptions)!;

            var (enums, classes) = ConvertNewSchemaToOld(newSchema);

            foreach (var (enumName, schemaEnum) in enums)
            {
                allEnums[enumName] = schemaEnum;
            }

            foreach (var (className, schemaClass) in classes)
            {
                if (IgnoreClasses.Contains(className))
                    continue;

                allClasses[className] = schemaClass with { Name = className };
            }
        }

        var parentToChildMap = allClasses.Where(kvp => kvp.Value.Parent != null)
            .GroupBy(kvp => kvp.Value.Parent!)
            .ToDictionary(g => g.Key, g => g.ToImmutableList());

        // Generate graph of classes -> fields
        var graph = new AdjacencyGraph<string, Edge<string>>();

        // Types used as pointers
        var pointeeTypes = new HashSet<string>();

        foreach (var (className, schemaClass) in allClasses)
        {
            if (schemaClass.Parent != null)
                graph.AddVerticesAndEdge(new Edge<string>(className, schemaClass.Parent));

            foreach (var field in schemaClass.Fields)
            {
                var currentType = field.Type;
                while (currentType != null)
                {
                    if (currentType.IsDeclared)
                    {
                        graph.AddVerticesAndEdge(new Edge<string>(className, currentType.Name));
                    }

                    currentType = currentType.Inner;
                }

                // Pointers mean we need to add references to the child classes of referenced type
                if (field.Type.Category == SchemaTypeCategory.Ptr)
                {
                    var childClasses = parentToChildMap.GetValueOrDefault(
                        field.Type.Inner!.Name,
                        ImmutableList<KeyValuePair<string, SchemaClass>>.Empty);

                    var queue = new Queue<(string, string)>(childClasses.Select(x => (className, x.Key)));

                    while (queue.Count > 0)
                    {
                        var (parent, childClass) = queue.Dequeue();

                        graph.AddVerticesAndEdge(new Edge<string>(parent, childClass));

                        var myChildren = parentToChildMap.GetValueOrDefault(
                            childClass,
                            ImmutableList<KeyValuePair<string, SchemaClass>>.Empty);
                        foreach (var (toAdd, _) in myChildren)
                        {
                            queue.Enqueue((childClass, toAdd));
                        }
                    }

                    pointeeTypes.Add(field.Type.Inner!.Name);
                }
            }
        }

        // Do a search from NetworkClasses.Names
        var visited = new HashSet<string>();
        var search = new BreadthFirstSearchAlgorithm<string, Edge<string>>(graph);
        search.FinishVertex += node => { visited.Add(node); };

        foreach (var networkClassName in NetworkClasses.Names)
        {
            search.Compute(networkClassName);
        }

        // Clear output directory
        if (Directory.Exists(outputPath))
        {
            string[] files = Directory.GetFiles(outputPath, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        Directory.CreateDirectory(Path.Combine(outputPath, "Enums"));
        Directory.CreateDirectory(Path.Combine(outputPath, "Classes"));

        var enumBuilder = GetTemplate(false);
        foreach (var (enumName, schemaEnum) in allEnums)
        {
            var newBuilder = new StringBuilder(enumBuilder.ToString());
            WriteEnum(newBuilder, enumName, schemaEnum);
            File.WriteAllText(Path.Combine(outputPath, "Enums", $"{SanitiseTypeName(enumName)}.g.cs"),
                newBuilder.ToString().ReplaceLineEndings("\r\n"));
        }

        // Manually whitelist some classes
        visited.Add("CTakeDamageInfo");
        visited.Add("CTakeDamageResult");
        visited.Add("CEntitySubclassVDataBase");
        visited.Add("CFiringModeFloat");
        visited.Add("CFiringModeInt");
        visited.Add("CSkillFloat");
        visited.Add("CSkillInt");
        visited.Add("CRangeFloat");
        visited.Add("CNavLinkAnimgraphVar");
        visited.Add("DecalGroupOption_t");
        visited.Add("DestructibleHitGroupToDestroy_t");

        var classBuilder = GetTemplate(true);

        var visitedClassNames = new HashSet<string>();
        foreach (var (className, schemaClass) in allClasses)
        {
            if (visited.Contains(className) || className.Contains("VData"))
            {
                var isPointeeType = pointeeTypes.Contains(className);

                var newBuilder = new StringBuilder(classBuilder.ToString());
                WriteClass(newBuilder, className, schemaClass, allClasses, isPointeeType);
                visitedClassNames.Add(className);

                File.WriteAllText(Path.Combine(outputPath, "Classes", $"{SanitiseTypeName(className)}.g.cs"),
                    newBuilder.ToString().ReplaceLineEndings("\r\n"));
            }
        }
    }

    private static IEnumerable<(SchemaClass clazz, SchemaField field)> GetAllParentFields(
        SchemaClass schemaClass,
        SortedDictionary<string, SchemaClass> allClasses)
    {
        while (schemaClass.Parent != null)
        {
            allClasses.TryGetValue(schemaClass.Parent, out var parentClass);
            if (parentClass == null)
                break;

            foreach (var field in parentClass.Fields)
            {
                yield return (parentClass, field);
            }

            schemaClass = parentClass;
        }
    }

    private static void WriteClass(
        StringBuilder builder,
        string schemaClassName,
        SchemaClass schemaClass,
        SortedDictionary<string, SchemaClass> allClasses,
        bool isPointeeType)
    {
        var isEntityClass =
            NetworkClasses.Names.Contains(schemaClassName)
            || NetworkClasses.Names.Contains(schemaClass.Parent ?? "");

        var classNameCs = SanitiseTypeName(schemaClassName);

        builder.AppendLine();
        builder.Append($"public partial class {classNameCs}");

        (SchemaClass clazz, SchemaField field)[] parentFields = [];
        if (schemaClass.Parent != null)
        {
            builder.Append($" : {schemaClass.Parent}");
            parentFields = GetAllParentFields(schemaClass, allClasses).ToArray();
        }

        if (schemaClass.Parent == null && classNameCs != "CEntityInstance")
        {
            builder.Append($" : NativeObject");
        }
        else if (classNameCs == "CEntityInstance")
        {
            builder.Append($" : NativeEntity");
        }

        builder.AppendLine();
        builder.AppendLine("{");

        // All entity classes eventually derive from CEntityInstance,
        // which is the root networkable class.
        if (classNameCs != "CEntityInstance")
        {
            builder.AppendLine(
                $"    public {classNameCs} (IntPtr pointer) : base(pointer) {{}}");
            builder.AppendLine();
        }

        foreach (var field in schemaClass.Fields)
        {
            if (IgnoreClassWildcards.Any(y => field.Type.Name.Contains(y)))
                continue;

            // Putting these in the too hard basket for now.
            if (field.Name == "m_VoteOptions" || field.Name == "m_aShootSounds" ||
                field.Name == "m_pVecRelationships") continue;
            if (IgnoreClasses.Contains(field.Type.Name)) continue;
            if (field.Type.Category == SchemaTypeCategory.Bitfield) continue;

            if (field.Type is { Category: SchemaTypeCategory.Atomic, Atomic: SchemaAtomicCategory.Collection })
            {
                if (IgnoreClasses.Contains(field.Type.Inner!.Name)) continue;
            }

            var requiresNewKeyword = parentFields.Any(x =>
                x.clazz.CsPropertyNameForField(x.clazz.Name, x.field) == schemaClass.CsPropertyNameForField(schemaClassName, field));

            var handleParams = $"this.Handle, \"{schemaClassName}\", \"{field.Name}\"";

            builder.AppendLine($"\t// {field.Name}");
            builder.AppendLine($"\t[SchemaMember(\"{schemaClassName}\", \"{field.Name}\")]");

            if (field.Type is { Category: SchemaTypeCategory.Ptr, CsTypeName: "string" })
            {
                var getter = $"return Schema.GetString({handleParams});";
                var setter = $"Schema.SetString({handleParams}, value{(field.Type.ArraySize != null ? ", " + field.Type.ArraySize : "")});";
                builder.AppendLine(
                    $"\tpublic {(requiresNewKeyword ? "new " : "")}{SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)}");
                builder.AppendLine($"\t{{");
                builder.AppendLine(
                    $"\t\tget {{ {getter} }}");
                builder.AppendLine(
                    $"\t\tset {{ {setter} }}");
                builder.AppendLine($"\t}}");
                builder.AppendLine();
            }

            if (field.Type is { Category: SchemaTypeCategory.FixedArray, CsTypeName: "string" })
            {
                var getter = $"return Schema.GetString({handleParams});";
                var setter = $"Schema.SetStringBytes({handleParams}, value, {field.Type.ArraySize});";
                builder.AppendLine(
                    $"\tpublic {(requiresNewKeyword ? "new " : "")}{SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)}");
                builder.AppendLine($"\t{{");
                builder.AppendLine(
                    $"\t\tget {{ {getter} }}");
                builder.AppendLine(
                    $"\t\tset {{ {setter} }}");
                builder.AppendLine($"\t}}");
                builder.AppendLine();
            }
            // Networked Strings require UTF8 encoding/decoding
            else if (field.Type is { Category: SchemaTypeCategory.Atomic, CsTypeName: "string" })
            {
                var getter = $"return Schema.GetUtf8String({handleParams});";
                var setter = $"Schema.SetString({handleParams}, value);";
                builder.AppendLine(
                    $"\tpublic {(requiresNewKeyword ? "new " : "")}{SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)}");
                builder.AppendLine($"\t{{");
                builder.AppendLine(
                    $"\t\tget {{ {getter} }}");
                builder.AppendLine(
                    $"\t\tset {{ {setter} }}");
                builder.AppendLine($"\t}}");
                builder.AppendLine();
            }
            else if (field.Type.Category == SchemaTypeCategory.FixedArray)
            {
                var getter =
                    $"Schema.GetFixedArray<{SanitiseTypeName(field.Type.Inner!.CsTypeName)}>({handleParams}, {field.Type.ArraySize});";
                builder.AppendLine(
                    $"\tpublic {(requiresNewKeyword ? "new " : "")}Span<{SanitiseTypeName(field.Type.Inner!.CsTypeName)}> {schemaClass.CsPropertyNameForField(schemaClassName, field)} => {getter}");
                builder.AppendLine();
            }
            else if (field.Type.Category == SchemaTypeCategory.DeclaredClass &&
                     !IgnoreClasses.Contains(field.Type.Name))
            {
                var getter = $"Schema.GetDeclaredClass<{SanitiseTypeName(field.Type.CsTypeName)}>({handleParams});";
                builder.AppendLine(
                    $"\tpublic {(requiresNewKeyword ? "new " : "")}{SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)} => {getter}");
                builder.AppendLine();
            }
            else if ((field.Type.Category == SchemaTypeCategory.Builtin ||
                      field.Type.Category == SchemaTypeCategory.DeclaredEnum) &&
                     !IgnoreClasses.Contains(field.Type.Name))
            {
                var getter = $"ref Schema.GetRef<{SanitiseTypeName(field.Type.CsTypeName)}>({handleParams});";
                builder.AppendLine(
                    $"\tpublic {(requiresNewKeyword ? "new " : "")}ref {SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)} => {getter}");
                builder.AppendLine();
            }
            else if (field.Type.Category == SchemaTypeCategory.Ptr)
            {
                var inner = field.Type.Inner!;
                if (inner.Category != SchemaTypeCategory.DeclaredClass) continue;

                builder.AppendLine(
                    $"\tpublic {(requiresNewKeyword ? "new " : "")}{SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)} => Schema.GetPointer<{SanitiseTypeName(inner.CsTypeName)}>({handleParams});");
                builder.AppendLine();
            }
            else if (field.Type is { Category: SchemaTypeCategory.Atomic, Name: "Color" })
            {
                var getter = $"return Schema.GetCustomMarshalledType<{field.Type.CsTypeName}>({handleParams});";
                var setter = $"Schema.SetCustomMarshalledType<{field.Type.CsTypeName}>({handleParams}, value);";
                builder.AppendLine(
                    $"\tpublic {(requiresNewKeyword ? "new " : "")}{SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)}");
                builder.AppendLine($"\t{{");
                builder.AppendLine(
                    $"\t\tget {{ {getter} }}");
                builder.AppendLine(
                    $"\t\tset {{ {setter} }}");
                builder.AppendLine($"\t}}");
                builder.AppendLine();
            }
            else if (field.Type.Category == SchemaTypeCategory.Atomic)
            {
                var getter = $"Schema.GetDeclaredClass<{SanitiseTypeName(field.Type.CsTypeName)}>({handleParams});";
                builder.AppendLine(
                    $"\tpublic {(requiresNewKeyword ? "new " : "")}{SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)} => {getter}");
                builder.AppendLine();
            }
        }

        builder.AppendLine($"}}");
    }

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        AllowTrailingCommas = true
    };

    private static string EnumType(int alignment) =>
        alignment switch
        {
            1 => "byte",
            2 => "ushort",
            4 => "uint",
            8 => "ulong",
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
        };

    private static void WriteEnum(StringBuilder builder, string enumName, SchemaEnum schemaEnum)
    {
        builder.AppendLine();
        builder.AppendLine($"public enum {SanitiseTypeName(enumName)} : {EnumType(schemaEnum.Align)}");
        builder.AppendLine("{");

        // Write enum items
        foreach (var enumItem in schemaEnum.Items)
        {
            string value;
            if (schemaEnum.Align == 8)
            {
                value = unchecked((ulong)enumItem.Value).ToString("X");
            }
            else if (schemaEnum.Align == 4)
            {
                value = unchecked((uint)enumItem.Value).ToString("X");
            }
            else if (schemaEnum.Align == 2)
            {
                value = unchecked((ushort)enumItem.Value).ToString("X");
            }
            else
            {
                value = unchecked((byte)enumItem.Value).ToString("X");
            }

            builder.AppendLine($"\t{enumItem.Name} = 0x{value},");
        }

        builder.AppendLine("}");
    }
}
