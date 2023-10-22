namespace CounterStrikeSharp.SchemaGen;

public record SchemaEnum(
    int Align,
    IReadOnlyList<SchemaEnumItem> Items);
