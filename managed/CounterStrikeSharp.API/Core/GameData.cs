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
        try
        {
            _methods = JsonSerializer.Deserialize<Dictionary<string, LoadedGameData>>(File.ReadAllText(gameDataPath))!;

            Console.WriteLine($"Loaded game data with {_methods.Count} methods.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load game data: {ex}");
        }
    }

    public static string GetSignature(string key)
    {
        Console.WriteLine($"Getting signature: {key}");
        if (!_methods.ContainsKey(key))
        {
            throw new ArgumentException($"Method {key} not found in gamedata.json");
        }

        var methodMetadata = _methods[key];
        if (methodMetadata.Signatures == null)
        {
            throw new InvalidOperationException($"No signatures found for {key} in gamedata.json");
        }

        string signature;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            signature = methodMetadata.Signatures?.Linux ?? throw new InvalidOperationException($"No Linux signature for {key} in gamedata.json");
        }
        else
        {
            signature = methodMetadata.Signatures?.Windows ?? throw new InvalidOperationException($"No Windows signature for {key} in gamedata.json");
        }

        return signature;
    }

    public static int GetOffset(string key)
    {
        if (!_methods.ContainsKey(key))
        {
            throw new Exception($"Method {key} not found in gamedata.json");
        }

        var methodMetadata = _methods[key];

        if (methodMetadata.Offsets == null)
        {
            throw new Exception($"No offsets found for {key} in gamedata.json");
        }

        int offset;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            offset = methodMetadata.Offsets?.Linux ?? throw new InvalidOperationException($"No Linux offset for {key} in gamedata.json");
        }
        else
        {
            offset = methodMetadata.Offsets?.Windows ?? throw new InvalidOperationException($"No Windows offset for {key} in gamedata.json");
        }

        return offset;
    }
}