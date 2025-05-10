using System.Text.RegularExpressions;

namespace CounterStrikeSharp.SchemaGen;

public partial record SchemaClass(
    int Index,
    string Name,
    string? Parent,
    IReadOnlyList<SchemaField> Fields)
{
    private Dictionary<string, SchemaField[]>? _fieldsByCsPropertyName = null;

    [GeneratedRegex("^m_(fl|a|n|i|isz|vec|us|u|ub|un|sz|b|f|clr|h|ang|af|ch|q|p|v|arr|bv|e|s)([A-Z])")]
    private static partial Regex HungarianNotationRegex();

    private static string RemoveMemberPrefix(string fieldName)
    {
        if (fieldName.StartsWith("m_"))
            fieldName = fieldName[2..];

        return $"{char.ToUpper(fieldName[0])}{fieldName[1..]}";
    }

    public string CsPropertyNameForField(string className, SchemaField field)
    {
        // When the hungarian notation prefix is removed, some fields shadow their parent's.
        // Because there are only a handful, we have some manual overrides.
        switch (className, field.Name)
        {
            case ("CFish", "m_waterLevel"): return "FishWaterLevel";
            case ("CBaseFire", "m_nFlags"): return "FireFlags";
            case ("CDynamicLight", "m_Flags"): return "DynamicLightFlags";
            case ("CEnvScreenOverlay", "m_bIsActive"): return "IsOverlayActive";
            case ("CPlayerSprayDecal", "m_nEntity"): return "DecalEntity";
            case ("CHandleTest", "m_Handle"): return "TestHandle";
            case ("FilterTeam", "m_iFilterTeam"): return "Value";
        }

        string CleanFieldName(string fieldName) =>
            RemoveMemberPrefix(HungarianNotationRegex().Replace(fieldName, "$2"));

        _fieldsByCsPropertyName ??= Fields
            .GroupBy(x => CleanFieldName(x.Name))
            .ToDictionary(g => g.Key, g => g.ToArray());

        var cleanName = CleanFieldName(field.Name);

        // If removing the hungarian notation prefix causes clashes with other fields,
        // then just remove the `m_` prefix and convert to title case.
        return _fieldsByCsPropertyName[cleanName].Length == 1
            ? cleanName
            : RemoveMemberPrefix(field.Name);
    }
}
