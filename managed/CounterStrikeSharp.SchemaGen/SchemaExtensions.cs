
namespace CounterStrikeSharp.SchemaGen;

public static class SchemaExtensions
{
    public static bool IsNetworkEnabled(this SchemaField field)
    {
        return field.Metadata.ContainsKey("MNetworkEnable");
    }
}
