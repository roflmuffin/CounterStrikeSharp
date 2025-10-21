namespace CounterStrikeSharp.API.Core.Capabilities;

public static class Capabilities
{
    public record RegisteredCapability(string Name, Type Type, bool IsPlayer);

    public static void RegisterPluginCapability<T>(BasePlugin plugin, PluginCapability<T> capability, Func<T> supplier)
    {
        if (!PluginCapability<T>.Providers.TryGetValue(capability.Name, out var list))
            PluginCapability<T>.Providers[capability.Name] = list = new();

        list.RemoveAll(f => f.Method.DeclaringType == supplier.Method.DeclaringType);
        list.Add(supplier);

        plugin.TrackCapability(capability.Name, typeof(T), false);
    }

    public static void RegisterPlayerCapability<T>(BasePlugin plugin, PlayerCapability<T> capability, Func<CCSPlayerController, T> supplier)
    {
        if (!PlayerCapability<T>.Providers.TryGetValue(capability.Name, out var list))
            PlayerCapability<T>.Providers[capability.Name] = list = new();

        list.RemoveAll(f => f.Method.DeclaringType == supplier.Method.DeclaringType);
        list.Add(supplier);

        plugin.TrackCapability(capability.Name, typeof(T), true);
    }

    public static void Unregister(RegisteredCapability cap)
    {
        if (cap.IsPlayer)
        {
            if (PlayerCapability<object>.Providers.TryGetValue(cap.Name, out var list))
                list.RemoveAll(f => f.Method.DeclaringType == cap.Type.DeclaringType);
        }
        else
        {
            if (PluginCapability<object>.Providers.TryGetValue(cap.Name, out var list))
                list.RemoveAll(f => f.Method.DeclaringType == cap.Type.DeclaringType);
        }
    }
}
