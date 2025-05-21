/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

using CounterStrikeSharp.API.Modules.Memory;

namespace CounterStrikeSharp.API.Core;

public partial class CPlayer_WeaponServices
{
    /// <summary>
    /// Drops player weapon.
    /// </summary>
    /// <exception cref="InvalidOperationException">WeaponServices points to null</exception>
    public void DropWeapon(CBasePlayerWeapon weapon, Vector? vecTarget = null, Vector? velocity = null)
    {
        if (Handle == IntPtr.Zero)
            throw new InvalidOperationException("WeaponServices points to null.");

        Guard.IsValidEntity(weapon);

        nint _position = vecTarget?.Handle ?? 0;
        nint _velocity = velocity?.Handle ?? 0;

        VirtualFunction.CreateVoid<IntPtr, IntPtr, IntPtr, IntPtr>(Handle, GameData.GetOffset("CCSPlayer_WeaponServices_DropWeapon"))(Handle, weapon.Handle, _position, _velocity);
    }

    /// <summary>
    /// Equips weapon in hand.
    /// </summary>
    /// <exception cref="InvalidOperationException">WeaponServices points to null</exception>
    public void SelectWeapon(CBasePlayerWeapon weapon, int unk = 0)
    {
        if (Handle == IntPtr.Zero)
            throw new InvalidOperationException("WeaponServices points to null.");

        Guard.IsValidEntity(weapon);

        VirtualFunction.CreateVoid<IntPtr, IntPtr, int>(Handle, GameData.GetOffset("CCSPlayer_WeaponServices_SelectItem"))(Handle, weapon.Handle, unk);
    }
}
