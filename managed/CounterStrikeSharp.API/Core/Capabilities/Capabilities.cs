namespace CounterStrikeSharp.API.Core.Capabilities;

public static class Capabilities
{
    public static void RegisterPlayerCapability<T>(PlayerCapability<T> capability,
        Func<CCSPlayerController, T> supplier)
    {
        if (!PlayerCapability<T>.Providers.ContainsKey(capability.Name))
        {
            PlayerCapability<T>.Providers.Add(capability.Name, new());
        }

        PlayerCapability<T>.Providers[capability.Name].Add(entity =>
        {
            if (entity is CCSPlayerController player)
            {
                return supplier(player);
            }

            throw new InvalidOperationException($"Cannot get {capability} from non-player entity");
        });
    }
}