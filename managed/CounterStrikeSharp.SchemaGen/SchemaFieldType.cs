using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace CounterStrikeSharp.SchemaGen;

public record SchemaFieldType
{
    public SchemaFieldType(string Name,
        SchemaTypeCategory Category,
        SchemaAtomicCategory? Atomic,
        SchemaFieldType? Inner)
    {
        this.Name = Name;
        this.Category = Category;
        this.Atomic = Atomic;
        this.Inner = Inner;

        if (this.Name == "GameTime_t")
        {
            this.Category = SchemaTypeCategory.Builtin;
            this.Name = "float32";
        }
        else if
            (this.Name == "CPlayerSlot" || this.Name == "HSequence" || this.Name == "CSplitScreenSlot" || this.Name == "GameTick_t")
        {
            this.Category = SchemaTypeCategory.Builtin;
            this.Name = "int32";
        }
        else if (this.Name == "CBitVec< 64 >")
        {
            this.Category = SchemaTypeCategory.FixedArray;
            this.Inner = new SchemaFieldType("uint8", SchemaTypeCategory.Builtin, null, null);
            this.Name = "uint8[8]";
        }
    }

    public bool IsString =>
        Category == SchemaTypeCategory.FixedArray
        && Inner!.Category == SchemaTypeCategory.Builtin
        && Inner.Name == "char";

    public bool IsDeclared => Category is SchemaTypeCategory.DeclaredClass or SchemaTypeCategory.DeclaredEnum;

    public bool TryGetArrayElementType([NotNullWhen(true)] out SchemaFieldType? elementType)
    {
        if (Category == SchemaTypeCategory.FixedArray && !IsString)
        {
            elementType = Inner!;
            return true;
        }
        else
        {
            elementType = null;
            return false;
        }
    }

    private static string BuiltinToCsKeyword(string name) => name switch
    {
        "float32" => "float",
        "float64" => "double",
        "int8" => "sbyte",
        "int16" => "Int16",
        "int32" => "Int32",
        "int64" => "Int64",
        "uint8" => "byte",
        "uint16" => "UInt16",
        "uint32" => "UInt32",
        "uint64" => "UInt64",
        "bool" => "bool",
        _ => throw new ArgumentOutOfRangeException(nameof(name), name, $"Unknown built-in: {name}")
    };

    private static string AtomicToCsTypeName(string name, SchemaAtomicCategory atomic, SchemaFieldType? inner) =>
        atomic switch
        {
            SchemaAtomicCategory.Basic => name switch
            {
                "CUtlString" or "CUtlSymbolLarge" => "string",
                "CEntityHandle" => "CHandle<CEntityInstance>",
                "CNetworkedQuantizedFloat" => "float",
                "RotationVector" => "Vector",
                _ => name
            },
            SchemaAtomicCategory.T => $"{name.Split('<')[0]}<{inner!.CsTypeName}>",
            SchemaAtomicCategory.Collection => $"NetworkedVector<{inner!.CsTypeName}>",
            SchemaAtomicCategory.Unknown => "CBitVec",
            _ => throw new ArgumentOutOfRangeException(nameof(atomic), atomic, $"Unsupported atomic: {atomic}")
        };

    public string CsTypeName => Category switch
    {
        SchemaTypeCategory.Builtin => BuiltinToCsKeyword(Name),
        SchemaTypeCategory.Ptr => $"{Inner!.CsTypeName}?",
        SchemaTypeCategory.FixedArray => IsString
            ? "string"
            : $"{Inner!.CsTypeName}[]",
        SchemaTypeCategory.Atomic => AtomicToCsTypeName(Name, Atomic!.Value, Inner),
        SchemaTypeCategory.DeclaredClass => Name,
        SchemaTypeCategory.DeclaredEnum => Name,
        _ => throw new ArgumentOutOfRangeException(nameof(Category), Category, $"Unsupported type category: {Category}")
    };

    public string Name { get; init; }
    public SchemaTypeCategory Category { get; init; }
    public SchemaAtomicCategory? Atomic { get; init; }
    public SchemaFieldType? Inner { get; init; }

    public int? ArraySize
    {
        get
        {
            if (Category == SchemaTypeCategory.FixedArray)
            {
                // Extract number from name, e.g. `uint16[2]`
                var match = Regex.Match(this.Name, @"\[(\d+)\]$");
                return match.Success ? int.Parse(match.Groups[1].Value) : null;
            }

            return null;
        }
    }

    public void Deconstruct(out string Name, out SchemaTypeCategory Category, out SchemaAtomicCategory? Atomic,
        out SchemaFieldType? Inner, out int? ArraySize)
    {
        Name = this.Name;
        Category = this.Category;
        Atomic = this.Atomic;
        Inner = this.Inner;
        ArraySize = this.ArraySize;
    }
}