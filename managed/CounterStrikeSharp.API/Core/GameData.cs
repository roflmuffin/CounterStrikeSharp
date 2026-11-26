using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Core.Hosting;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core;

public class LoadedGameData
{
    [JsonPropertyName("signatures")] public Signatures? Signatures { get; set; }
    [JsonPropertyName("offsets")] public Offsets? Offsets { get; set; }

    /// <summary>
    /// Slot Binding fields (class, library, fingerprint window; see <c>src/core/binding_resolver.h</c>): present when the
    /// offset is a vtable slot index that native verifies at boot.
    /// </summary>
    [JsonInclude, JsonPropertyName("vtable")] internal JsonElement? Vtable { get; set; }
}

public class Signatures
{
    [JsonPropertyName("library")] public string Library { get; set; }

    [JsonPropertyName("windows")] public string Windows { get; set; }

    [JsonPropertyName("linux")] public string Linux { get; set; }
}

public class Offsets
{
    // Backing fields stay null when the entry is absent, so a missing per-OS offset is not mistaken for 0.
    private int? _windows;
    private int? _linux;

    [JsonPropertyName("windows")] public int Windows { get => _windows ?? 0; set => _windows = value; }

    [JsonPropertyName("linux")] public int Linux { get => _linux ?? 0; set => _linux = value; }

    internal int? WindowsOrNull => _windows;

    internal int? LinuxOrNull => _linux;
}

public sealed class GameDataProvider : IStartupService
{
    private readonly string _gameDataDirectoryPath;
    public Dictionary<string,LoadedGameData> Methods;
    private readonly ILogger<GameDataProvider> _logger;

    public GameDataProvider(IScriptHostConfiguration scriptHostConfiguration, ILogger<GameDataProvider> logger)
    {
        _logger = logger;
        _gameDataDirectoryPath = scriptHostConfiguration.GameDataPath;
    }
    
    public void Load()
    {
        try
        {
            Methods = new Dictionary<string, LoadedGameData>();

            foreach (string filePath in Directory.EnumerateFiles(_gameDataDirectoryPath, "*.json"))
            {
                string jsonContent = File.ReadAllText(filePath);
                Dictionary<string, LoadedGameData> loadedMethods = JsonSerializer.Deserialize<Dictionary<string, LoadedGameData>>(jsonContent)!;

                foreach (KeyValuePair<string, LoadedGameData> loadedMethod in loadedMethods)
                {
                    if (Methods.ContainsKey(loadedMethod.Key))
                    {
                        _logger.LogWarning("GameData Method \"{Key}\" loaded a duplicate entry from {filePath}.", loadedMethod.Key, filePath);
                    }
                    
                    Methods[loadedMethod.Key] = loadedMethod.Value;
                }
                
                if (loadedMethods != null)
                {
                    _logger.LogInformation("Successfully loaded {Count} game data entries from {Path}", loadedMethods.Count, filePath);
                }
                else
                {
                    _logger.LogWarning("Unable to load game data entries from {Path}, game data file is empty", filePath);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load game data");
        }
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

        // So the plugin signatures report names the key when a plugin resolves this signature.
        Bindings.PluginSignatures.NoteGamedataKey(signature, key);
        return signature;
    }

    /// <summary>
    /// The Gamedata offset <paramref name="key"/> for this OS. For a vtable slot CounterStrikeSharp verifies at boot (a
    /// Slot Binding, e.g. <c>CCSPlayerController_ChangeTeam</c>), the index the check verified or healed, which can differ
    /// from the file after a Game update; throws <see cref="InvalidOperationException"/> if it could not be verified.
    /// </summary>
    /// <remarks>
    /// A failed Slot Binding throws rather than falling back to the index in the file: after a Game update that index may
    /// hold another function, and calling it would be silently wrong. Catch the exception to degrade gracefully.
    /// </remarks>
    /// <exception cref="InvalidOperationException">The key is a Slot Binding whose Vtable Check failed on this build, or
    /// has no offset for this OS.</exception>
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

        if (methodMetadata.Vtable != null && VerifiedSlotIndex(key) is { } slot)
        {
            return slot;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return methodMetadata.Offsets.LinuxOrNull ??
                   throw new InvalidOperationException($"Offset {key} is missing for this OS (Linux) in gamedata.json");
        }

        return methodMetadata.Offsets.WindowsOrNull ??
               throw new InvalidOperationException($"Offset {key} is missing for this OS (Windows) in gamedata.json");
    }

    /// <summary>
    /// A Slot Binding's index as the boot Vtable Check verified or healed it (from the Resolution Report), so plugins
    /// reading it by key follow a Heal like CounterStrikeSharp's own helpers. null when native did not resolve the key as
    /// a Slot Binding (e.g. it is only in a plugin's own gamedata file) or has no index for it: Gamedata is read as before.
    /// Throws when the Slot Binding failed: its Gamedata index does not hold the function on this build.
    /// </summary>
    private static int? VerifiedSlotIndex(string key)
    {
        var binding = EngineFeatures.Current.Report.Find(key);
        if (binding is not { Kind: BindingKind.Slot }) return null;
        if (!binding.Resolved)
        {
            throw new InvalidOperationException(
                $"Offset {key} failed its Vtable Check on this build ({binding.Reason.Describe(binding.MatchCount)})");
        }

        return binding.Offset >= 0 ? checked((int)binding.Offset) : null;
    }
}