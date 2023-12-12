namespace CounterStrikeSharp.API.Core.Plugin;

public interface IPluginContext
{
    PluginState State { get; }
    IPlugin Plugin { get; }
    int PluginId { get; }

    string FilePath { get; }
    void Load(bool hotReload);
    void Unload(bool hotReload);
}