using System.IO;
using Microsoft.Extensions.Hosting;

namespace CounterStrikeSharp.API.Core.Hosting;

internal sealed class ScriptHostConfiguration : IScriptHostConfiguration
{
    public string RootPath { get; }
    public string PluginPath { get; }
    public string ConfigsPath { get; }
    public string GameDataPath { get; }

    public ScriptHostConfiguration(IHostEnvironment hostEnvironment)
    {
        RootPath = Path.Join(new[] { hostEnvironment.ContentRootPath });
        PluginPath = Path.Join(new[] { hostEnvironment.ContentRootPath, "plugins" });
        ConfigsPath = Path.Join(new[] { hostEnvironment.ContentRootPath, "configs" });
        GameDataPath = Path.Join(new[] { hostEnvironment.ContentRootPath, "gamedata" });
    }
}