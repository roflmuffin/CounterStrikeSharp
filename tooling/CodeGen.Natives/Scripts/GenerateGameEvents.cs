using Gameloop.Vdf;
using Gameloop.Vdf.Linq;

namespace CodeGen.Natives.Scripts;

public partial class Generators
{
    public class GameEvent
    {
        public GameEvent(string name)
        {
            Name = name;
        }
        public string Name { get; init; }
        public string NamePascalCase => Name.ToPascalCase();
        public List<GameEventKey> Keys { get; set; } = new();
    }

    public class GameEventKey
    {
        public GameEventKey(string name, string type)
        {
            Name = name;
            Type = type;
        }
        public string Name { get; init; }
        public string Type { get; init; }
        public string NamePascalCase => Name.ToPascalCase();
        public string MappedType => Mapping.GetCSharpTypeFromGameEventType(Type);
        public string? Comment { get; set; }

        public string Getter
        {
            get
            {
                if (MappedType == "CCSPlayerController?")
                {
                    return $"GetPlayer(\"{Name}\")";
                }

                return $"Get<{MappedType}>(\"{Name}\")";
            }
        }

        public string Setter
        {
            get
            {
                if (MappedType == "CCSPlayerController?")
                {
                    return $"SetPlayer(\"{Name}\", value)";
                }

                return $"Set<{MappedType}>(\"{Name}\", value)";
            }
        }
    }

    private static HttpClient _httpClient = new HttpClient();
    private static string BaseUrl = "https://raw.githubusercontent.com/SteamDatabase/GameTracking-CS2/master/";

    private static List<string> GameEventFiles = new List<string>()
    {
        "game/core/pak01_dir/resource/core.gameevents",
        "game/csgo/pak01_dir/resource/game.gameevents",
        "game/csgo/pak01_dir/resource/mod.gameevents"
    };

    private static async Task<List<GameEvent>> GetGameEvents()
    {
        var allGameEvents = new Dictionary<string, GameEvent>();

        foreach (string url in GameEventFiles)
        // foreach (string file in Directory.EnumerateFiles(pathToSearch, "*.gameevents", SearchOption.AllDirectories).OrderBy(Path.GetFileName))
        {
            var file = await _httpClient.GetStringAsync($"{BaseUrl}/{url}");
            var deserialized = VdfConvert.Deserialize(file);

            var properties =
                deserialized.Value.Where(x => x.Type == VTokenType.Property);
            foreach (var token in properties)
            {
                var property = (VProperty)token;

                var eventName = property.Key;
                var eventProperties = property.Value as VObject;

                var gameEvent = new GameEvent(eventName);

                foreach (var kvp in eventProperties?.Children()!)
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
                            var gameEventKey = new GameEventKey(asKvp.Key, asKvp.Value.ToString().Trim());

                            gameEvent.Keys.Add(gameEventKey);
                        }
                    }
                }

                allGameEvents[gameEvent.Name] = gameEvent;
            }
        }

        return allGameEvents.Values.ToList();
    }



    public static async Task GenerateGameEvents()
    {
        var allGameEvents = await GetGameEvents();

        var gameEventsString = string.Join("\n", allGameEvents.OrderBy(x => x.NamePascalCase).Select(gameEvent =>
        {
            var propertyDefinition = gameEvent.Keys.Select(key =>
            {
                // Hack for now, since we some params with the same name as their parent.
                var propertyName = key.NamePascalCase == gameEvent.NamePascalCase
                    ? $"{key.NamePascalCase}Param"
                    : key.NamePascalCase;

                return $@"

                {(!string.IsNullOrEmpty(key.Comment) ? "// " + key.Comment : "")}
                public {key.MappedType} {propertyName}
                {{
                    get => {key.Getter};
                    set => {key.Setter};
                }}";
            });
            return $@"
            [EventName(""{gameEvent.Name}"")]
            public class Event{gameEvent.NamePascalCase} : GameEvent
            {{
                public Event{gameEvent.NamePascalCase}(IntPtr pointer) : base(pointer){{}}
                public Event{gameEvent.NamePascalCase}(bool force) : base(""{gameEvent.Name}"", force){{}}

                {string.Join("\n", propertyDefinition)}
            }}";
        }));


        var result = $@"
#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core
{{
    {gameEventsString}
}}
#nullable restore
";

        Console.WriteLine($"Generated C# bindings for {allGameEvents.Count} game events successfully.");

        File.WriteAllText(Path.Join(Helpers.GetRootDirectory(), "managed/CounterStrikeSharp.API/Core/GameEvents.g.cs"),
            result);
    }
}
