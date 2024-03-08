namespace CounterStrikeSharp.API.Core.Capabilities;

public static class Capabilities
{
    public static void RegisterPluginCapability<T>(PluginCapability<T> capability, Func<T> supplier)
    {
        if (!PluginCapability<T>.Providers.ContainsKey(capability.Name))
        {
            PluginCapability<T>.Providers.Add(capability.Name, new());
        }

        PluginCapability<T>.Providers[capability.Name].Add(supplier);
    }

    public static void RegisterPlayerCapability<T>(PlayerCapability<T> capability,
        Func<CCSPlayerController, T> supplier)
    {
        if (!PlayerCapability<T>.Providers.ContainsKey(capability.Name))
        {
            PlayerCapability<T>.Providers.Add(capability.Name, new());
        }

        PlayerCapability<T>.Providers[capability.Name].Add(supplier);
    }
}