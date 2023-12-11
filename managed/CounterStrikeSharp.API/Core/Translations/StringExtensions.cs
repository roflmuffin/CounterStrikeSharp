using System.Reflection;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core.Translations;

public static class StringExtensions
{
    public static string ReplaceColorTags(this string message)
    {
        var modifiedValue = message;
        foreach (var field in typeof(ChatColors).GetFields())
        {
            string pattern = $"{{{field.Name}}}";
            if (modifiedValue.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                modifiedValue = modifiedValue.Replace(pattern, field.GetValue(null)?.ToString() ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }
        }

        return modifiedValue.Equals(message) ? message : $" {modifiedValue}";
    }
}