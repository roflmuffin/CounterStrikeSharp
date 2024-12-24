using System.Text.Json;
using System.Reflection;

namespace CounterStrikeSharp.API.Modules.Extensions;

public static class PluginConfigExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public static JsonSerializerOptions JsonSerializerOptions => _jsonSerializerOptions;

    /// <summary>
    /// Gets the configuration file path
    /// </summary>
    /// <typeparam name="T">Type of the plugin configuration.</typeparam>
    /// <param name="_">Current configuration instance</param>
    public static string GetConfigPath<T>(this T _) where T : BasePluginConfig, new()
    {
        string assemblyName = typeof(T).Assembly.GetName().Name ?? string.Empty;
        return Path.Combine(Server.GameDirectory, "csgo", "addons", "counterstrikesharp", "configs", "plugins", assemblyName, $"{assemblyName}.json");
    }

    /// <summary>
    /// Updates the configuration file
    /// </summary>
    /// <typeparam name="T">Type of the plugin configuration.</typeparam>
    /// <param name="config">Current configuration instance</param>
    public static void Update<T>(this T config) where T : BasePluginConfig, new()
    {
        var configPath = config.GetConfigPath();

        try
        {
            using var stream = new FileStream(configPath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new StreamWriter(stream);
            writer.Write(JsonSerializer.Serialize(config, JsonSerializerOptions));
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update configuration file at '{configPath}'.", ex);
        }
    }

    /// <summary>
    /// Reloads the configuration file and updates current configuration instance.
    /// </summary>
    /// <typeparam name="T">Type of the plugin configuration.</typeparam>
    /// <param name="config">Current configuration instance</param>
    public static void Reload<T>(this T config) where T : BasePluginConfig, new()
    {
        var configPath = config.GetConfigPath();

        try
        {
            if (!File.Exists(configPath))
            {
                throw new FileNotFoundException($"Configuration file '{configPath} not found.");
            }

            var configContent = File.ReadAllText(configPath);

            var newConfig = JsonSerializer.Deserialize<T>(configContent)
                ?? throw new JsonException($"Deserialization failed for configuration file '{configPath}'.");

            foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.CanWrite)
                {
                    property.SetValue(config, property.GetValue(newConfig));
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to reload configuration file at '{configPath}'.", ex);
        }
    }
}
