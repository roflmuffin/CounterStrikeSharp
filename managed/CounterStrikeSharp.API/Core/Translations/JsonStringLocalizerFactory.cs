using CounterStrikeSharp.API.Core.Hosting;
using CounterStrikeSharp.API.Core.Plugin;
using Microsoft.Extensions.Localization;

namespace CounterStrikeSharp.API.Core.Translations;

public class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IPluginContext _pluginContext;

    public JsonStringLocalizerFactory(IPluginContext pluginContext)
    {
        _pluginContext = pluginContext;
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        return new JsonStringLocalizer(Path.Join(Path.GetDirectoryName(_pluginContext.FilePath), "lang"));
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return new JsonStringLocalizer(Path.Join(Path.GetDirectoryName(_pluginContext.FilePath), "lang"));
    }
}