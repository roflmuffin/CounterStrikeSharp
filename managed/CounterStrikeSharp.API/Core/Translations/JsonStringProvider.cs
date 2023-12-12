using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace CounterStrikeSharp.API.Core.Translations;

public class JsonStringProvider
{
    private readonly ConcurrentDictionary<string, IList<string>> _resourceNamesCache = new();
    private readonly JsonResourceManager _jsonResourceManager;

    public JsonStringProvider(JsonResourceManager jsonResourceManager)
    {
        _jsonResourceManager = jsonResourceManager;
    }

    private string GetResourceCacheKey(CultureInfo culture)
    {
        return $"Culture={culture.Name}";
    }

    public IList<string> GetAllResourceStrings(CultureInfo culture, bool throwOnMissing)
    {
        var cacheKey = GetResourceCacheKey(culture);

        return _resourceNamesCache.GetOrAdd(cacheKey, _ =>
        {
            var resourceSet = _jsonResourceManager.GetResourceSet(culture, tryParents: false);
            if (resourceSet == null)
            {
                if (throwOnMissing)
                {
                    throw new MissingManifestResourceException($"The manifest resource for the culture '{culture.Name}' is missing.");
                }
                else
                {
                    return null;
                }
            }

            var names = new List<string>();
            foreach (var entry in resourceSet)
            {
                names.Add(entry.Key);
            }

            return names;
        });
    }
}