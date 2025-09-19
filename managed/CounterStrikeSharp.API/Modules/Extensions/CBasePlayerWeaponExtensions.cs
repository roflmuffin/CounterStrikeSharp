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
    /// Gets the owner of a weapon entity
    /// </summary>
    /// <param name="weapon">The weapon entity.</param>
    /// <returns>The <see cref="CCSPlayerController"/> instance for the player, or <c>null</c> if it doesn't exist.</returns>
    public static CCSPlayerController? GetOwner(this CBasePlayerWeapon weapon)
    {
        SteamID? steamId = null;

        if (weapon.OriginalOwnerXuidLow > 0)
            steamId = new(weapon.OriginalOwnerXuidLow);

        CCSPlayerController? player;

        if (steamId != null && steamId.IsValid())
        {
            player = Utilities.GetPlayerFromSteamId(steamId.SteamId64);

            if (player == null)
                player = Utilities.GetPlayerFromSteamId(weapon.OriginalOwnerXuidLow);
        }
        else
        {
            player = Utilities.GetPlayerFromIndex((int)weapon.OwnerEntity.Index);

            if (player == null)
                player = Utilities.GetPlayerFromIndex((int)weapon.As<CCSWeaponBaseGun>().OwnerEntity.Value!.Index);
        }

        return !string.IsNullOrEmpty(player?.PlayerName) ? player : null;
    }
}
