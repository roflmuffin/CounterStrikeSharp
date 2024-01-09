using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CounterStrikeSharp.API;

public partial class PlayerValuesManager
{
    private readonly Dictionary<string, Func<int, string>> _getters = new();

    public void AddValueGetter(string name, Func<int, string> getter)
    {
        if (!_getters.TryAdd(name, getter))
        {
            throw new Exception("There's already a getter like this");
        }
    }

    public string GetValue(int clientIndex, string name)
    {
        if (_getters.TryGetValue(name, out var getter))
        {
            return getter.Invoke(clientIndex);
        }

        throw new Exception($"Value getter for '{name}' not found.");
    }

    public string ReplacePlayerValues(int clientIndex, string input)
    {
        var matches = PlayerValueRegex().Matches(input);

        foreach (var value in matches.Select(m => m.Groups[1].Value).Distinct())
        {
            if (_getters.TryGetValue(value, out var func))
            {
                input = input.Replace($"{{{value}}}", func.Invoke(clientIndex));
            }
        }

        return input;
    }

    [GeneratedRegex("{(.*?)}")]
    private static partial Regex PlayerValueRegex();
}