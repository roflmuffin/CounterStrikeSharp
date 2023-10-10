using Gameloop.Vdf;
using Gameloop.Vdf.JsonConverter;
using Gameloop.Vdf.Linq;
using YamlDotNet.Core.Tokens;

namespace CodeGen.Natives.Scripts;

public partial class Generators
{
    public class GameEvent
    {
        public string Name { get; set; }
        public string NamePascalCase => Name.ToPascalCase();
        public List<GameEventKey> Keys { get; set; } = new();
    }

    public class GameEventKey
    {
        public string Name { get; set; }
        public string NamePascalCase => Name.ToPascalCase();
        public string Type { get; set; }
        public string MappedType => Mapping.GetCSharpTypeFromGameEventType(Type);
        public string Comment { get; set; }
        public string AccessorPostfix => Mapping.GetEventAccessorPostfixFromType(MappedType);
    }

    private static List<GameEvent> GetGameEvents()
    {
        // temporary, not committing resource files directly to git for now
        var pathToSearch = @"/home/michael/Steam/cs2-ds/game/csgo/events/resource";
        if (!Directory.Exists(pathToSearch)) Environment.Exit(0);
        var allGameEvents = new Dictionary<string, GameEvent>();

        foreach (string file in Directory.EnumerateFiles(pathToSearch, "*.gameevents", SearchOption.AllDirectories).OrderBy(Path.GetFileName))
        {
            var deserialized = VdfConvert.Deserialize(File.ReadAllText(file));

            var properties =
                deserialized.Value.Where(x => x.Type == VTokenType.Property);
            foreach (var token in properties)
            {
                var property = (VProperty)token;

                var eventName = property.Key;
                var eventProperties = property.Value as VObject;

                var gameEvent = new GameEvent() { Name = eventName };

                foreach (var kvp in eventProperties.Children())
                {
                    if (kvp is VValue or VProperty)
                    {
                        if (kvp is VValue asComment)
                        {
                            if (gameEvent.Keys.Any())
                            {
                                gameEvent.Keys.Last().Comment = asComment.ToString().Trim();
                            }
                        }
                        else if (kvp is VProperty asKvp)
                        {
                            var gameEventKey = new GameEventKey
                            {
                                Name = asKvp.Key,
                                Type = asKvp.Value.ToString().Trim()
                            };

                            gameEvent.Keys.Add(gameEventKey);
                        }
                    }
                }

                allGameEvents[gameEvent.Name] = gameEvent;
            }
        }

        return allGameEvents.Values.ToList();
    }

    public static void GenerateGameEvents()
    {
        var allGameEvents = GetGameEvents();

        var gameEventsString = string.Join("\n", allGameEvents.OrderBy(x => x.NamePascalCase).Select(gameEvent =>
        {
            var propertyDefinition = gameEvent.Keys.Select(key =>
            {
                // Hack for now, since we some params with the same name as their parent.
                var propertyName = key.NamePascalCase == gameEvent.NamePascalCase
                    ? $"{key.NamePascalCase}Param"
                    : key.NamePascalCase;

                var postFix = key.AccessorPostfix;
                
                return $@"
                
                {(!string.IsNullOrEmpty(key.Comment) ? "// " + key.Comment : "")}
                public {key.MappedType} {propertyName} 
                {{
                    get => Get{postFix}(""{key.Name}"");
                    set => Set{postFix}(""{key.Name}"", value);
                }}";
            });
            return $@"
            public class {gameEvent.NamePascalCase} : GameEvent
            {{
                public {gameEvent.NamePascalCase}() : base(){{}}
                public {gameEvent.NamePascalCase}(bool force) : base(""{gameEvent.Name}"", force){{}}

                {string.Join("\n", propertyDefinition)}
            }}";
        }));
        

        var result = $@"
using System;
using CounterStrikeSharp.API.Modules.Events;

namespace CounterStrikeSharp.API.Core
{{
    {gameEventsString}
}}
";
        
        Console.WriteLine($"Generated C# bindings for {allGameEvents.Count} game events successfully.");

        File.WriteAllText(Path.Join(Helpers.GetRootDirectory(), "managed/CounterStrikeSharp.API/Core/GameEvents.g.cs"),
            result);
    }
}