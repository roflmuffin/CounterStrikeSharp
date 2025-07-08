using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Core;

public partial class CBasePlayerWeapon
{
    public CBasePlayerWeaponVData? VData => GetVData<CBasePlayerWeaponVData>();

    /// <summary>
    /// Changes the subclass (loadout definition) of the weapon to a specified item definition index.
    /// <example>
    /// <code>
    /// weapon.ChangeSubclass(5027);
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="itemDefIndex">The item definition index to apply as subclass</param>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void ChangeSubclass(ushort itemDefIndex)
    {
        Guard.IsValidEntity(this);
        VirtualFunctions.ChangeSubclass(Handle, itemDefIndex.ToString(), 0);
    }
}
