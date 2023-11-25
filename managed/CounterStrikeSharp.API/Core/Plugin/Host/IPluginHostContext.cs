using System.Collections.Generic;

namespace CounterStrikeSharp.API.Core.Plugin.Host;

public interface IPluginHostContext
{
    public void Load();
    public IEnumerable<PluginContext> GetLoadedPlugins();
}