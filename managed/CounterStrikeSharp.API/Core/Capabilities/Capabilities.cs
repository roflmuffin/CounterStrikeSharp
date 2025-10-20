using System.Reflection;

namespace CounterStrikeSharp.API.Core.Capabilities;

public static class Capabilities
{
    private static readonly Dictionary<Assembly, List<(string Name, Type Type, bool IsPlayer)>> Registered = new();

    public static void RegisterPluginCapability<T>(PluginCapability<T> capability, Func<T> supplier)
    {
        if (!PluginCapability<T>.Providers.TryGetValue(capability.Name, out var list))
            PluginCapability<T>.Providers[capability.Name] = list = new();

        var asm = supplier.Method.DeclaringType?.Assembly;

        var index = list.FindIndex(f => f.Method.DeclaringType?.Assembly == asm);
        if (index >= 0)
            list[index] = supplier;
        else
            list.Add(supplier);

        Track(asm!, capability.Name, typeof(T), false);
    }

    public static void RegisterPlayerCapability<T>(PlayerCapability<T> capability, Func<CCSPlayerController, T> supplier)
    {
        if (!PlayerCapability<T>.Providers.TryGetValue(capability.Name, out var list))
            PlayerCapability<T>.Providers[capability.Name] = list = new();

        var asm = supplier.Method.DeclaringType?.Assembly;

        var index = list.FindIndex(f => f.Method.DeclaringType?.Assembly == asm);
        if (index >= 0)
            list[index] = supplier;
        else
            list.Add(supplier);

        Track(asm!, capability.Name, typeof(T), true);
    }

    private static void Track(Assembly asm, string name, Type type, bool isPlayer)
    {
        if (!Registered.TryGetValue(asm, out var list))
            Registered[asm] = list = new();

        list.RemoveAll(e => e.Name == name && e.Type == type && e.IsPlayer == isPlayer);
        list.Add((name, type, isPlayer));
    }

    public static void UnregisterAll(Assembly asm)
    {
        if (!Registered.TryGetValue(asm, out var list))
            return;

        foreach (var entry in list)
        {
            if (entry.IsPlayer)
            {
                if (PlayerCapability<object>.Providers.TryGetValue(entry.Name, out var provList))
                    provList.RemoveAll(f => f.Method.DeclaringType?.Assembly == asm);
            }
            else
            {
                if (PluginCapability<object>.Providers.TryGetValue(entry.Name, out var provList))
                    provList.RemoveAll(f => f.Method.DeclaringType?.Assembly == asm);
            }
        }

        Registered.Remove(asm);
    }
}
