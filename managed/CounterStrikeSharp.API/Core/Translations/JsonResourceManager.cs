using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;

namespace CounterStrikeSharp.API.Core.Translations
{
    public class JsonResourceManager
    {
        private static readonly JsonDocumentOptions _jsonDocumentOptions = new()
        {
            CommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        private ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _resourcesCache = new();

        public JsonResourceManager(string resourcesPath)
        {
            ResourcesPath = resourcesPath;
        }

        public string ResourcesPath { get; }
        
        public virtual ConcurrentDictionary<string, string> GetResourceSet(CultureInfo culture, bool tryParents)
        {
            TryLoadResourceSet(culture);

            if (tryParents)
            {
                var allResources = new ConcurrentDictionary<string, string>();
                do
                {
                    TryLoadResourceSet(culture);
                    if (_resourcesCache.TryGetValue(culture.Name, out ConcurrentDictionary<string, string> resources))
                    {
                        foreach (var entry in resources)
                        {
                            allResources.TryAdd(entry.Key, entry.Value);
                        }
                    }

                    culture = culture.Parent;
                } while (culture != CultureInfo.InvariantCulture);

                return allResources;
            }
            else
            {
                _resourcesCache.TryGetValue(culture.Name, out ConcurrentDictionary<string, string> resources);

                return resources;
            }
        }

        public virtual string GetString(string name)
        {
            var culture = CultureInfo.CurrentUICulture;
            GetResourceSet(culture, tryParents: true);

            if (_resourcesCache.Count == 0)
            {
                return null;
            }

            do
            {
                if (_resourcesCache.ContainsKey(culture.Name))
                {
                    if (_resourcesCache[culture.Name].TryGetValue(name, out string value))
                    {
                        return value;
                    }
                }

                culture = culture.Parent;
            } while (culture != culture.Parent);

            return null;
        }

        public virtual string? GetString(string name, CultureInfo culture)
        {
            var values = GetResourceSet(culture, tryParents: true);
            
            if (values.Count == 0)
            {
                return null;
            }

            return values.TryGetValue(name, out string value)
                ? value
                : null;
        }

        private void TryLoadResourceSet(CultureInfo culture)
        {
            if (!_resourcesCache.ContainsKey(culture.Name))
            {
                var file = Path.Combine(ResourcesPath, $"{culture.Name}.json");

                var resources = LoadJsonResources(file);
                
                _resourcesCache.TryAdd(culture.Name,
                    new ConcurrentDictionary<string, string>(resources.ToDictionary(r => r.Key, r => r.Value)));
            }
        }

        private static IDictionary<string, string> LoadJsonResources(string filePath)
        {
            var resources = new Dictionary<string, string>();
            if (File.Exists(filePath))
            {
                using var reader = new StreamReader(filePath);

                using var document = JsonDocument.Parse(reader.BaseStream, _jsonDocumentOptions);

                resources = document.RootElement.EnumerateObject().ToDictionary(e => e.Name, e => e.Value.ToString());
            }

            return resources;
        }
    }
}
