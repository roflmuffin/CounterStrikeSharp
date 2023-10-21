using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CounterStrikeSharp.API.Core;

class MethodMetadata
{
    [JsonPropertyName("signatures")]
    public Signatures? Signatures { get; set; }

}

public class Signatures
{
    [JsonPropertyName("library")]
    public string Library { get; set; }

    [JsonPropertyName("windows")]
    public string Windows { get; set; }

    [JsonPropertyName("linux")]
    public string Linux { get; set; }
}

public static class GameData
{
    private static Dictionary<string, MethodMetadata> _methods;
    public static void Load(string gameDataPath)
    {
        _methods = JsonSerializer.Deserialize<Dictionary<string, MethodMetadata>>(File.ReadAllText(gameDataPath));
        
        Console.WriteLine($"Loaded game data with {_methods.Count} methods.");
    }

    public static string GetSignature(string methodName)
    {
        if (!_methods.ContainsKey(methodName)) throw new Exception($"Method {methodName} not found in gamedata.json");

        var methodMetadata = _methods[methodName];
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return methodMetadata.Signatures.Linux;
        }

        return methodMetadata.Signatures.Windows;
    }
}