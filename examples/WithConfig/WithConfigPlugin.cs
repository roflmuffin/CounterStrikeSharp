using System.ComponentModel.DataAnnotations;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using Microsoft.Extensions.Options;

namespace WithConfig;

public class SampleConfig
{
    [MaxLength(25)]
    public string ChatPrefix { get; set; } = "My Cool Plugin";

    [Range(0, 60)]
    public float ChatInterval { get; set; } = 60;
}

[MinimumApiVersion(80)]
public class WithConfigPlugin : BasePlugin
{
    public override string ModuleName => "Example: With Config";
    public override string ModuleVersion => "1.0.0";

    public WithConfigPlugin(IOptions<SampleConfig> config)
    {
        Config = config.Value;
    }

    public SampleConfig Config { get; set; }
}