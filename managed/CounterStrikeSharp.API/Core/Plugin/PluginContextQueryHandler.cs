using System;
using System.Linq;

namespace CounterStrikeSharp.API.Core.Plugin;

public class PluginContextQueryHandler : IPluginContextQueryHandler
{
    private readonly IPluginHostContext _pluginHostContext;

    public PluginContextQueryHandler(IPluginHostContext pluginHostContext)
    {
        _pluginHostContext = pluginHostContext;
    }

    public PluginContext? FindPluginByType(Type moduleClass)
    {
        return _pluginHostContext.GetLoadedPlugins().FirstOrDefault(x => x.Plugin.GetType() == moduleClass);
    }

    public PluginContext? FindPluginById(int id)
    {
        return _pluginHostContext.GetLoadedPlugins().FirstOrDefault(x => x.PluginId == id);
    }

    public PluginContext? FindPluginByModuleName(string name)
    {
        return _pluginHostContext.GetLoadedPlugins().FirstOrDefault(x => x.Plugin.ModuleName == name);
    }

    public PluginContext? FindPluginByModulePath(string path)
    {
        return _pluginHostContext.GetLoadedPlugins().FirstOrDefault(x => x.Plugin.ModulePath == path);
    }

    public PluginContext? FindPluginByIdOrName(string query)
    {
        return _pluginHostContext.GetLoadedPlugins().FirstOrDefault(x => x.PluginId.ToString() == query || x.Plugin.ModuleName == query);
    }
}