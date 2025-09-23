using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Modules.Extensions;

public static class CBasePlayerWeaponExtensions
{
    /// <summary>
    /// Gets the weapon's designer name (e.g., "weapon_ak47").
    /// </summary>
    /// <param name="weapon">The <see cref="CBasePlayerWeapon"/> instance.</param>
    /// <returns>The designer name of the weapon as a string, or <c>null</c> if it cannot be retrieved.</returns>
    public static string? GetWeaponName(this CBasePlayerWeapon weapon)
    {
        return weapon.GetVData<CCSWeaponBaseVData>()?.Name;
    }

    /// <summary>
    /// Gets the econ owner of a weapon entity
    /// </summary>
    /// <param name="weapon">The weapon entity.</param>
    /// <returns>The <see cref="CCSPlayerController"/> instance for the player, or <c>null</c> if it doesn't exist.</returns>
    public static CCSPlayerController? GetEconOwner(this CBasePlayerWeapon weapon)
    {
        ulong originalXuid = weapon.OriginalOwnerXuidLow;
        SteamID? steamId = originalXuid > 0 ? new(originalXuid) : null;

        CCSPlayerController? player = null;

        if (steamId?.IsValid() == true)
            player = Utilities.GetPlayers().FirstOrDefault(p =>
                p.SteamID == steamId.SteamId64 || p.SteamID == originalXuid);

        if (player == null)
            player = weapon.OwnerEntity.Get()?.As<CCSPlayerController>();

        return player?.Connected == PlayerConnectedState.PlayerConnected ? player : null;
    }
}
