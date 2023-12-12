using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using Microsoft.Extensions.Localization;

namespace CounterStrikeSharp.API.Core.Translations;

public class JsonStringLocalizer : IStringLocalizer
{
    private readonly JsonResourceManager _resourceManager;
    private readonly JsonStringProvider _resourceStringProvider;
    
    public JsonStringLocalizer(string langPath)
    {
        _resourceManager = new JsonResourceManager(langPath);
        _resourceStringProvider = new(_resourceManager);
    }
    
    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return GetAllStrings(includeParentCultures, CultureInfo.CurrentUICulture);
    }

    public LocalizedString this[string name]
    {
        get
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var value = GetStringSafely(name);

            return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var format = GetStringSafely(name);
            var value = string.Format(format ?? name, arguments);

            return new LocalizedString(name, value, resourceNotFound: format == null);
        }
    }
    
    protected string? GetStringSafely(string name, CultureInfo? culture = null)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        culture = culture ?? CultureInfo.CurrentUICulture;

        var result = _resourceManager.GetString(name, culture);
        
        // Fallback to en if running in invariant mode.
        if (result == null && culture.Equals(CultureInfo.InvariantCulture))
        {
            result = _resourceManager.GetFallbackString(name);
        }
        
        // Fallback to the default culture (en-US) if the resource is not found for the current culture.
        if (result == null && !culture.Equals(CultureInfo.DefaultThreadCurrentUICulture))
        {
            result = _resourceManager.GetString(name, CultureInfo.DefaultThreadCurrentUICulture!);
        }

        return result?.ReplaceColorTags();
    }
    
    protected virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures, CultureInfo culture)
    {
        if (culture == null)
        {
            throw new ArgumentNullException(nameof(culture));
        }

        var resourceNames = includeParentCultures
            ? GetResourceNamesFromCultureHierarchy(culture)
            : _resourceStringProvider.GetAllResourceStrings(culture, true);

        foreach (var name in resourceNames)
        {
            var value = GetStringSafely(name, culture);
            yield return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
        }
    }
    
    private IEnumerable<string> GetResourceNamesFromCultureHierarchy(CultureInfo startingCulture)
    {
        var currentCulture = startingCulture;
        var resourceNames = new HashSet<string>();

        while (currentCulture != currentCulture.Parent)
        {
            var cultureResourceNames = _resourceStringProvider.GetAllResourceStrings(currentCulture, false);

            if (cultureResourceNames != null)
            {
                foreach (var resourceName in cultureResourceNames)
                {
                    resourceNames.Add(resourceName);
                }
            }

            currentCulture = currentCulture.Parent;
        }

        return resourceNames;
    }
}