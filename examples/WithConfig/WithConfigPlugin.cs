using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Config;
using CounterStrikeSharp.API.Modules.Extensions;

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

    [ConsoleCommand("css_reload_config", "Reloads the plugin config")]
    public void OnReloadConfig(CCSPlayerController? player, CommandInfo commandInfo)
    {
        commandInfo.ReplyToCommand("Chat Interval before reload: " + Config.ChatInterval);
        Config.Reload();
        commandInfo.ReplyToCommand("Chat Interval after reload: " + Config.ChatInterval);
    }

    [ConsoleCommand("css_reset_config", "Resets the plugin config")]
    public void OnResetConfig(CCSPlayerController? player, CommandInfo commandInfo)
    {
        commandInfo.ReplyToCommand("Chat Interval before reset: " + Config.ChatInterval);
        Config.ChatInterval = 60;
        Config.Update();
        commandInfo.ReplyToCommand("Chat Interval after reset: " + Config.ChatInterval);
    }
}
