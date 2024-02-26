using System.Collections.Generic;

namespace CounterStrikeSharp.API.Core.Capabilities;

public sealed class PlayerCapability<T> where T : class
{
    public string Name { get; }
    internal static readonly Dictionary<string, List<Func<CCSPlayerController, T?>>> Providers = new();

    public PlayerCapability(string name)
    {
        Name = name;
    }

    public T? Get(CCSPlayerController entity)
    {
        foreach (var provider in Providers[Name])
        {
            var ret = provider(entity);
            if (ret != null)
            {
                return ret;
            }
        }

        return null;
    }
}