namespace CounterStrikeSharp.SchemaGen;

public record SchemaModule(
    IReadOnlyDictionary<string, SchemaEnum> Enums,
    IReadOnlyDictionary<string, SchemaClass> Classes);
