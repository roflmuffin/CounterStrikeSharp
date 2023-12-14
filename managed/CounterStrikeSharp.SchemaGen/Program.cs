/**
 * This project has been copied & modified from the demofile-net project under the MIT license.
 * See ACKNOWLEDGEMENTS file for more information.
 * https://github.com/saul/demofile-net
 */

using System.Collections.Immutable;
using System.Diagnostics;
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
        "MoodAnimationLayer_t"
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

    public static string SanitiseTypeName(string typeName) => typeName.Replace(":", "");

    public static void Main(string[] args)
    {
        var outputPath =
            args.SingleOrDefault() ??
            throw new Exception("Expected a single CLI argument: <output path .cs>");

        // Concat together all enums and classes
        var allEnums = new SortedDictionary<string, SchemaEnum>();
        var allClasses = new SortedDictionary<string, SchemaClass>();

        var schemaFiles = new[] { "server.json" };

        foreach (var schemaFile in schemaFiles)
        {
            var schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Schema", schemaFile);

            var schema = JsonSerializer.Deserialize<SchemaModule>(
                File.ReadAllText(schemaPath),
                SerializerOptions)!;

            foreach (var (enumName, schemaEnum) in schema.Enums)
            {
                allEnums[enumName] = schemaEnum;
            }

            foreach (var (className, schemaClass) in schema.Classes)
            {
                if (IgnoreClasses.Contains(className))
                    continue;

                allClasses[className] = schemaClass;
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

        // Only emit visited vertices from the search
        var builder = new StringBuilder();
        builder.AppendLine("// <auto-generated />");
        builder.AppendLine("#nullable enable");
        builder.AppendLine("#pragma warning disable CS1591");
        builder.AppendLine();
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Diagnostics;");
        builder.AppendLine("using System.Drawing;");
        builder.AppendLine("using CounterStrikeSharp;");
        builder.AppendLine("using CounterStrikeSharp.API.Modules.Events;");
        builder.AppendLine("using CounterStrikeSharp.API.Modules.Entities;");
        builder.AppendLine("using CounterStrikeSharp.API.Modules.Memory;");
        builder.AppendLine("using CounterStrikeSharp.API.Modules.Utils;");
        builder.AppendLine("using CounterStrikeSharp.API.Core.Attributes;");
        builder.AppendLine();
        builder.AppendLine("namespace CounterStrikeSharp.API.Core;");

        foreach (var (enumName, schemaEnum) in allEnums)
        {
            WriteEnum(builder, enumName, schemaEnum);
        }

        // Manually whitelist some classes
        visited.Add("CTakeDamageInfo");
        visited.Add("CEntitySubclassVDataBase");
        visited.Add("CFiringModeFloat");
        visited.Add("CFiringModeInt");
        visited.Add("CSkillFloat");
        visited.Add("CSkillInt");
        visited.Add("CRangeFloat");
        visited.Add("CNavLinkAnimgraphVar");

        var visitedClassNames = new HashSet<string>();
        foreach (var (className, schemaClass) in allClasses)
        {
            if (visited.Contains(className) || className.Contains("VData"))
            {
                var isPointeeType = pointeeTypes.Contains(className);

                WriteClass(builder, className, schemaClass, parentToChildMap, isPointeeType);
                visitedClassNames.Add(className);
            }
        }

        File.WriteAllText(outputPath, builder.ToString());
    }

    private static void WriteClass(
        StringBuilder builder,
        string schemaClassName,
        SchemaClass schemaClass,
        IReadOnlyDictionary<string, ImmutableList<KeyValuePair<string, SchemaClass>>> parentToChildMap,
        bool isPointeeType)
    {
        var isEntityClass =
            NetworkClasses.Names.Contains(schemaClassName)
            || NetworkClasses.Names.Contains(schemaClass.Parent ?? "");

        var classNameCs = SanitiseTypeName(schemaClassName);

        builder.AppendLine();
        builder.Append($"public partial class {classNameCs}");

        if (schemaClass.Parent != null)
            builder.Append($" : {schemaClass.Parent}");

        if (schemaClass.Parent == null)
        {
            builder.Append($" : NativeObject");
        }

        builder.AppendLine();
        builder.AppendLine("{");

        // All entity classes eventually derive from CEntityInstance,
        // which is the root networkable class.

        builder.AppendLine(
            $"    public {classNameCs} (IntPtr pointer) : base(pointer) {{}}");
        builder.AppendLine();

        foreach (var field in schemaClass.Fields)
        {
            if (IgnoreClassWildcards.Any(y => field.Type.Name.Contains(y)))
                continue;
            
            // Putting these in the too hard basket for now.
            if (field.Name == "m_VoteOptions" || field.Name == "m_aShootSounds" || field.Name == "m_pVecRelationships") continue;
            if (IgnoreClasses.Contains(field.Type.Name)) continue;
            if (field.Type.Category == SchemaTypeCategory.Bitfield) continue;

            if (field.Type is { Category: SchemaTypeCategory.Atomic, Atomic: SchemaAtomicCategory.Collection })
            {
                if (IgnoreClasses.Contains(field.Type.Inner!.Name)) continue;
            }

            var handleParams = $"this.Handle, \"{schemaClassName}\", \"{field.Name}\"";

            builder.AppendLine($"\t// {field.Name}");
            builder.AppendLine($"\t[SchemaMember(\"{schemaClassName}\", \"{field.Name}\")]");

            if (field.Type is { Category: SchemaTypeCategory.FixedArray, CsTypeName: "string" } or { Category: SchemaTypeCategory.Ptr, CsTypeName: "string" })
            {
                var getter = $"return Schema.GetString({handleParams});";
                var setter = $"Schema.SetString({handleParams}, value);";
                builder.AppendLine(
                    $"    public {SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)}");
                builder.AppendLine($"    {{");
                builder.AppendLine(
                    $"        get {{ {getter} }}");
                builder.AppendLine(
                    $"        set {{ {setter} }}");
                builder.AppendLine($"    }}");
                builder.AppendLine();
            }
            // Networked Strings require UTF8 encoding/decoding
            else if (field.Type is { Category: SchemaTypeCategory.Atomic, CsTypeName: "string" })
            {
                var getter = $"return Schema.GetUtf8String({handleParams});";
                var setter = $"Schema.SetString({handleParams}, value);";
                builder.AppendLine(
                    $"    public {SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)}");
                builder.AppendLine($"    {{");
                builder.AppendLine(
                    $"        get {{ {getter} }}");
                builder.AppendLine(
                    $"        set {{ {setter} }}");
                builder.AppendLine($"    }}");
                builder.AppendLine();
            }
            else if (field.Type.Category == SchemaTypeCategory.FixedArray)
            {
                var getter =
                    $"Schema.GetFixedArray<{SanitiseTypeName(field.Type.Inner!.CsTypeName)}>({handleParams}, {field.Type.ArraySize});";
                builder.AppendLine(
                    $"    public Span<{SanitiseTypeName(field.Type.Inner!.CsTypeName)}> {schemaClass.CsPropertyNameForField(schemaClassName, field)} => {getter}");
                builder.AppendLine();
            }
            else if (field.Type.Category == SchemaTypeCategory.DeclaredClass &&
                     !IgnoreClasses.Contains(field.Type.Name))
            {
                var getter = $"Schema.GetDeclaredClass<{SanitiseTypeName(field.Type.CsTypeName)}>({handleParams});";
                builder.AppendLine(
                    $"    public {SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)} => {getter}");
                builder.AppendLine();
            }
            else if ((field.Type.Category == SchemaTypeCategory.Builtin ||
                      field.Type.Category == SchemaTypeCategory.DeclaredEnum) &&
                     !IgnoreClasses.Contains(field.Type.Name))
            {
                var getter = $"ref Schema.GetRef<{SanitiseTypeName(field.Type.CsTypeName)}>({handleParams});";
                builder.AppendLine(
                    $"    public ref {SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)} => {getter}");
                builder.AppendLine();
            }
            else if (field.Type.Category == SchemaTypeCategory.Ptr)
            {
                var inner = field.Type.Inner!;
                if (inner.Category != SchemaTypeCategory.DeclaredClass) continue;

                builder.AppendLine(
                    $"    public {SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)} => Schema.GetPointer<{SanitiseTypeName(inner.CsTypeName)}>({handleParams});");
                builder.AppendLine();
            }
            else if (field.Type is { Category: SchemaTypeCategory.Atomic, Name: "Color" })
            {
                var getter = $"return Schema.GetCustomMarshalledType<{field.Type.CsTypeName}>({handleParams});";
                var setter = $"Schema.SetCustomMarshalledType<{field.Type.CsTypeName}>({handleParams}, value);";
                builder.AppendLine(
                    $"    public {SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)}");
                builder.AppendLine($"    {{");
                builder.AppendLine(
                    $"        get {{ {getter} }}");
                builder.AppendLine(
                    $"        set {{ {setter} }}");
                builder.AppendLine($"    }}");
                builder.AppendLine();
            }
            else if (field.Type.Category == SchemaTypeCategory.Atomic)
            {
                var getter = $"Schema.GetDeclaredClass<{SanitiseTypeName(field.Type.CsTypeName)}>({handleParams});";
                builder.AppendLine(
                    $"    public {SanitiseTypeName(field.Type.CsTypeName)} {schemaClass.CsPropertyNameForField(schemaClassName, field)} => {getter}");
                builder.AppendLine();
            }
        }

        // Write decoder method
        // builder.AppendLine($"    internal {(schemaClass.Parent == null ? "" : "new ")}static SendNodeDecoder<{classNameCs}> CreateFieldDecoder(SerializableField field, DecoderSet decoderSet)");
        // builder.AppendLine("    {");

        foreach (var field in schemaClass.Fields)
        {
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

        var maxValue = schemaEnum.Align switch
        {
            1 => byte.MaxValue,
            2 => ushort.MaxValue,
            4 => uint.MaxValue,
            8 => ulong.MaxValue,
            _ => throw new ArgumentOutOfRangeException()
        };

        // Write enum items
        foreach (var enumItem in schemaEnum.Items)
        {
            var value = enumItem.Value < maxValue ? enumItem.Value : maxValue;
            builder.AppendLine($"    {enumItem.Name} = 0x{value:X},");
        }

        builder.AppendLine("}");
    }
}