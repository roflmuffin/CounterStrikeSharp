using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CounterStrikeSharp.API.Core;

class LoadedGameData
{
    [JsonPropertyName("signatures")] public Signatures? Signatures { get; set; }


    [JsonPropertyName("offsets")] public Offsets? Offsets { get; set; }
}

public class Signatures
{
    [JsonPropertyName("library")] public string Library { get; set; }

    [JsonPropertyName("windows")] public string Windows { get; set; }

    [JsonPropertyName("linux")] public string Linux { get; set; }
}

public class Offsets
{
    [JsonPropertyName("windows")] public int Windows { get; set; }

    [JsonPropertyName("linux")] public int Linux { get; set; }
}

public static class GameData
{
    private static Dictionary<string, LoadedGameData> _methods;

    public static void Load(string gameDataPath)
    {
        _methods = JsonSerializer.Deserialize<Dictionary<string, LoadedGameData>>(File.ReadAllText(gameDataPath));

        Console.WriteLine($"Loaded game data with {_methods.Count} methods.");
    }

    public static string GetSignature(string key)
    {
        if (!_methods.ContainsKey(key)) throw new Exception($"Method {key} not found in gamedata.json");
        if (_methods[key].Signatures == null) throw new Exception($"No signatures found for {key} in gamedata.json");

        var methodMetadata = _methods[key];
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return methodMetadata.Signatures!.Linux;
        }

        return methodMetadata.Signatures!.Windows;
    }

    public static int GetOffset(string key)
    {
        if (!_methods.ContainsKey(key)) throw new Exception($"Method {key} not found in gamedata.json");
        if (_methods[key].Offsets == null) throw new Exception($"No offsets found for {key} in gamedata.json");

        var methodMetadata = _methods[key];
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return methodMetadata.Offsets!.Linux;
        }

        return methodMetadata.Offsets!.Windows;
    }
}