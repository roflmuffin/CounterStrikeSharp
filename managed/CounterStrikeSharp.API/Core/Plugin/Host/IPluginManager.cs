using System.Collections.Generic;

namespace CounterStrikeSharp.API.Core.Plugin.Host;

public interface IPluginManager
{
    public void Load();
    public void LoadPlugin(string path);
    public IEnumerable<PluginContext> GetLoadedPlugins();
}