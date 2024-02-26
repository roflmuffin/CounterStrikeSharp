using System.Collections.Generic;

namespace CounterStrikeSharp.API.Core.Capabilities;

public sealed class PluginCapability<T>
{
    public string Name { get; }
    internal static readonly Dictionary<string, List<Func<T>>> Providers = new();

    public PluginCapability(string name)
    {
        Name = name;
    }

    public T? Get()
    {
        foreach (var provider in Providers[Name])
        {
            return provider();
        }

        return default;
    }
}