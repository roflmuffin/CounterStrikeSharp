using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;

namespace WithConfig;

public class SampleConfig : BasePluginConfig
{
    [JsonPropertyName("ChatPrefix")] public string ChatPrefix { get; set; } = "My Cool Plugin";

    [JsonPropertyName("ChatInterval")] public float ChatInterval { get; set; } = 60;
}

[MinimumApiVersion(80)]
public class WithConfigPlugin : BasePlugin, IPluginConfig<SampleConfig>
{
    public override string ModuleName => "Example: With Config";
    public override string ModuleVersion => "1.0.0";

    public SampleConfig Config { get; set; }

    public void OnConfigParsed(SampleConfig config)
    {
        // Do manual verification of the config and override any invalid values 
        if (config.ChatInterval > 60)
        {
            config.ChatInterval = 60;
        }

        if (config.ChatPrefix.Length > 25)
        {
            throw new Exception($"Invalid value has been set to config value 'ChatPrefix': {config.ChatPrefix}");
        }

        // Once we've validated the config, we can set it to the instance
        Config = config;
    }
}