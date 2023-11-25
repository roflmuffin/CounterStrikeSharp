using System.Linq;

namespace CounterStrikeSharp.API.Core.Plugin.Host;

public class PluginContextQueryHandler : IPluginContextQueryHandler
{
    private readonly IPluginManager _pluginManager;

    public PluginContextQueryHandler(IPluginManager pluginManager)
    {
        _pluginManager = pluginManager;
    }

    public IPluginContext? FindPluginByType(Type moduleClass)
    {
        return _pluginManager.GetLoadedPlugins().FirstOrDefault(x => x.Plugin.GetType() == moduleClass);
    }

    public IPluginContext? FindPluginById(int id)
    {
        return _pluginManager.GetLoadedPlugins().FirstOrDefault(x => x.PluginId == id);
    }

    public IPluginContext? FindPluginByModuleName(string name)
    {
        return _pluginManager.GetLoadedPlugins().FirstOrDefault(x => x.Plugin.ModuleName == name);
    }

    public IPluginContext? FindPluginByModulePath(string path)
    {
        return _pluginManager.GetLoadedPlugins().FirstOrDefault(x => x.Plugin.ModulePath == path);
    }

    public IPluginContext? FindPluginByIdOrName(string query)
    {
        return _pluginManager.GetLoadedPlugins().FirstOrDefault(x => x.PluginId.ToString() == query || x.Plugin.ModuleName == query);
    }
}