using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core.Hosting;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core;

public class LoadedGameData
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

public sealed class GameDataProvider : IStartupService
{
    private readonly string _gameDataFilePath;
    public Dictionary<string,LoadedGameData> Methods;
    private readonly ILogger<GameDataProvider> _logger;

    public GameDataProvider(IScriptHostConfiguration scriptHostConfiguration, ILogger<GameDataProvider> logger)
    {
        _logger = logger;
        _gameDataFilePath = Path.Join(scriptHostConfiguration.GameDataPath, "gamedata.json");
    }
    
    public void Load()
    {
        try
        {
            Methods = JsonSerializer.Deserialize<Dictionary<string, LoadedGameData>>(File.ReadAllText(_gameDataFilePath))!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load game data");
        }
        
        _logger.LogInformation("Successfully loaded {Count} game data entries from {Path}", Methods.Count, _gameDataFilePath);
    }
} 

public static class GameData
{
    internal static GameDataProvider GameDataProvider { get; set; } = null!;
    public static string GetSignature(string key)
    {
        Application.Instance.Logger.LogDebug("Getting signature: {Key}", key);
        if (!GameDataProvider.Methods.ContainsKey(key))
        {
            throw new ArgumentException($"Method {key} not found in gamedata.json");
        }

        var methodMetadata = GameDataProvider.Methods[key];
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
        if (!GameDataProvider.Methods.ContainsKey(key))
        {
            throw new Exception($"Method {key} not found in gamedata.json");
        }

        var methodMetadata = GameDataProvider.Methods[key];

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