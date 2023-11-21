using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

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

            GlobalContext.Instance.Logger.LogInformation("Loaded game data with {Count} methods.", _methods.Count);
        }
        catch (Exception ex)
        {
            GlobalContext.Instance.Logger.LogError(ex, "Failed to load game data");
        }
    }

    public static string GetSignature(string key)
    {
        GlobalContext.Instance.Logger.LogDebug("Getting signature: {Key}", key);
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