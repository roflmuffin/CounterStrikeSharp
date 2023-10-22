namespace CounterStrikeSharp.SchemaGen;

public enum SchemaTypeCategory
{
    Builtin = 0,
    Ptr = 1,
    Bitfield = 2,
    FixedArray = 3,
    Atomic = 4,
    DeclaredClass = 5,
    DeclaredEnum = 6,
    None = 7
};
