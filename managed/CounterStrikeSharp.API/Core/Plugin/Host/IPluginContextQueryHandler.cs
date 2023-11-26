namespace CounterStrikeSharp.API.Core.Plugin.Host;

public interface IPluginContextQueryHandler
{
    IPluginContext? FindPluginByType(Type moduleClass);

    IPluginContext? FindPluginById(int id);

    IPluginContext? FindPluginByModuleName(string name);

    IPluginContext? FindPluginByModulePath(string path);

    IPluginContext? FindPluginByIdOrName(string query);
}