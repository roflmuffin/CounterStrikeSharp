using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Localization;

namespace CounterStrikeSharp.API.Core.Translations;

internal class StringLocalizer : IStringLocalizer
{
    private IStringLocalizer _localizer;

    public StringLocalizer(IStringLocalizerFactory factory)
    {
        var type = typeof(StringLocalizer);
        var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
        _localizer = factory.Create(string.Empty, assemblyName.FullName);
    }

    public LocalizedString this[string name] => _localizer[name];

    public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _localizer.GetAllStrings(includeParentCultures);
}