using System.Collections.Generic;

namespace CounterStrikeSharp.API.Core.Capabilities;

public sealed class PlayerCapability<T>
{
    public string Name { get; }
    internal static readonly Dictionary<string, List<Func<CEntityInstance, T?>>> Providers = new();

    public PlayerCapability(string name)
    {
        Name = name;
    }

    public T? Get(CEntityInstance entity)
    {
        Console.WriteLine($"{Providers.Count}");
        foreach (var provider in Providers[Name])
        {
            var ret = provider(entity);
            if (ret != null)
            {
                return ret;
            }
        }

        throw new InvalidOperationException($"No provider for {Name} found");
    }
}