using System.Text.Json;
using System.Reflection;
using System.Runtime.Serialization;
using CounterStrikeSharp.API.Modules.Config;
using Tomlyn;

namespace CounterStrikeSharp.API.Modules.Extensions;

public static class PluginConfigExtensions
{
    public static JsonSerializerOptions JsonSerializerOptions => ConfigManager.JsonSerializerOptions;

    /// <summary>
    /// Gets the configuration file path
    /// </summary>
    /// <typeparam name="T">Type of the plugin configuration.</typeparam>
    /// <param name="_">Current configuration instance</param>
    public static string GetConfigPath<T>(this T _) where T : BasePluginConfig, new()
    {
        string assemblyName = typeof(T).Assembly.GetName().Name ?? string.Empty;

        string[] configFilePaths =
        [
            Path.Combine(Server.GameDirectory, "csgo", "addons", "counterstrikesharp", "configs", "plugins", assemblyName,
                $"{assemblyName}.json"),
            Path.Combine(Server.GameDirectory, "csgo", "addons", "counterstrikesharp", "configs", "plugins", assemblyName,
                $"{assemblyName}.toml"),
        ];

        foreach (var path in configFilePaths)
        {
            if (File.Exists(path))
            {
                return path;
            }
        }

        return configFilePaths[0];
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

            switch (Path.GetExtension(configPath))
            {
                case ".json":
                {
                    writer.Write(JsonSerializer.Serialize(config, ConfigManager.JsonSerializerOptions));
                    break;
                }
                case ".toml":
                    writer.Write(Toml.FromModel(config, ConfigManager.TomlModelOptions));
                    break;
                default:
                    throw new NotSupportedException($"Configuration file type '{Path.GetExtension(configPath)}' is not supported.");
            }


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

            T? newConfig = null;
            switch (Path.GetExtension(configPath))
            {
                case ".json":
                    newConfig = JsonSerializer.Deserialize<T>(configContent, ConfigManager.JsonSerializerOptions)
                                ?? throw new JsonException($"Deserialization failed for configuration file '{configPath}'.");
                    break;
                case ".toml":
                    newConfig = Toml.ToModel<T>(configContent, options: ConfigManager.TomlModelOptions);
                    break;
            }

            if (newConfig is null)
            {
                throw new SerializationException($"Deserialization failed for configuration file '{configPath}'.");
            }

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
