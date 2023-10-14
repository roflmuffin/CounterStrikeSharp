using YamlDotNet.Serialization;

namespace CodeGen.Natives.Scripts;

public partial class Generators
{
    public readonly record struct ListenerDefinition(string Name, Dictionary<string, string> Arguments);

    public static void GenerateListeners()
    {
        var pathToSearch = Path.Join(Helpers.GetRootDirectory(), "src/scripting/listeners");

        var deserializer = new DeserializerBuilder()
            .Build();

        var listeners = new List<ListenerDefinition>();
        foreach (string file in Directory.EnumerateFiles(pathToSearch, "*yaml", SearchOption.AllDirectories)
                     .OrderBy(Path.GetFileName))
        {
            var deserialized = deserializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(file));

            foreach (var listenerName in deserialized.Keys)
            {
                var parameterString = deserialized[listenerName];

                var parameters = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(parameterString))
                {
                    // Get each parameter, then map its type to the dictionary
                    // i.e. callback: pointer, name: string
                    parameters = parameterString
                        .Split(',')
                        .Select(part => part.Split(':'))
                        .ToDictionary(
                            pair => pair[0].Trim(),
                            pair => pair[1].Trim()
                        );
                }

                listeners.Add(new(listenerName, parameters));
            }
        }


        var outputString = string.Join("\n", listeners.Select(listener =>
        {
            var arguments = string.Join(", ",
                listener.Arguments.Select(pair => $"{Mapping.GetCSharpType(pair.Value)} {pair.Key}")).Trim();

            return $@"
        [ListenerName(""{listener.Name}"")]
        public delegate void {listener.Name}({arguments});";
        }));


        var result = $@"
using System;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core
{{
    public partial class Listeners {{
        {outputString}
    }}
}}
";

        Console.WriteLine($"Generated C# bindings for {listeners.Count} listeners successfully.");

        File.WriteAllText(Path.Join(Helpers.GetRootDirectory(), "managed/CounterStrikeSharp.API/Core/Listeners.g.cs"),
            result);
    }
}