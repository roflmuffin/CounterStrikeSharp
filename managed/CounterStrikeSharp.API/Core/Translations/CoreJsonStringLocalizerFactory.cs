using CounterStrikeSharp.API.Core.Hosting;
using Microsoft.Extensions.Localization;

namespace CounterStrikeSharp.API.Core.Translations;

public class CoreJsonStringLocalizerFactory : IStringLocalizerFactory
{
    private IScriptHostConfiguration _scriptHostConfiguration;

    public CoreJsonStringLocalizerFactory(IScriptHostConfiguration scriptHostConfiguration)
    {
        _scriptHostConfiguration = scriptHostConfiguration;
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        return new JsonStringLocalizer(_scriptHostConfiguration.LanguagePath);
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        return new JsonStringLocalizer(_scriptHostConfiguration.LanguagePath);
    }
}
