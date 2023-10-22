using System.Diagnostics.CodeAnalysis;

namespace CounterStrikeSharp.SchemaGen;

public record SchemaFieldType(
    string Name,
    SchemaTypeCategory Category,
    SchemaAtomicCategory? Atomic,
    SchemaFieldType? Inner,
    int? ArraySize)
{
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

    private static string AtomicToCsTypeName(string name, SchemaAtomicCategory atomic, SchemaFieldType? inner) => atomic switch
    {
        SchemaAtomicCategory.Basic => name switch
        {
            "CUtlString" or "CUtlSymbolLarge" => "NetworkedString",
            "CEntityHandle" => "CHandle<CEntityInstance>",
            "CNetworkedQuantizedFloat" => "float",
            _ => name
        },
        SchemaAtomicCategory.T => $"{name.Split('<')[0]}<{inner!.CsTypeName}>",
        SchemaAtomicCategory.Collection => $"NetworkedVector<{inner!.CsTypeName}>",
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
}
