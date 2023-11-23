using System.Collections.Generic;

namespace CounterStrikeSharp.API.Core.Plugin;

public interface IPluginHostContext
{
    public void Load();
    public IEnumerable<PluginContext> GetLoadedPlugins();
}