using FastGenericNew;
﻿/*
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

using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CCSPlayer_ItemServices
{
    /// <summary>
    /// Drops the active player weapon on the ground.
    /// </summary>
    /// <exception cref="InvalidOperationException">ItemServices points to null</exception>
    /// <exception cref="EngineFeatureUnavailableException">Engine Feature <c>cssharp.items/drop-active-weapon</c> is disabled</exception>
    public void DropActivePlayerWeapon(CBasePlayerWeapon activeWeapon)
    {
        var offset = EngineFeatures.Current.GetOffset(ManagedEngineFeatures.ItemsDropActiveWeapon,
            "CCSPlayer_ItemServices_DropActivePlayerWeapon");
        if (Handle == IntPtr.Zero)
            throw new InvalidOperationException("ItemServices points to null.");

        Guard.IsValidEntity(activeWeapon);

        VirtualFunction.CreateVoid<nint, nint, nint>(Handle, offset)(Handle, activeWeapon.Handle, Vector.Zero.Handle);
    }

    /// <summary>
    /// Removes every weapon from the player.
    /// </summary>
    /// <exception cref="InvalidOperationException">ItemServices points to null</exception>
    /// <exception cref="EngineFeatureUnavailableException">Engine Feature <c>cssharp.items/remove-weapons</c> is disabled</exception>
    public void RemoveWeapons()
    {
        var offset = EngineFeatures.Current.GetOffset(ManagedEngineFeatures.ItemsRemoveWeapons, "CCSPlayer_ItemServices_RemoveWeapons");
        if (Handle == IntPtr.Zero)
            throw new InvalidOperationException("ItemServices points to null.");

        VirtualFunction.CreateVoid<nint>(Handle, offset)(Handle);
    }

    /// <exception cref="EngineFeatureUnavailableException">Engine Feature <c>cssharp.items/give-named-item</c> is disabled</exception>
    public T? GiveNamedItem<T>(string item) where T : CEntityInstance
    {
        var offset = EngineFeatures.Current.GetOffset(ManagedEngineFeatures.ItemsGiveNamedItem, "CCSPlayer_ItemServices_GiveNamedItem");
        var pointer = VirtualFunction.Create<nint, string, nint>(Handle, offset)(Handle, item);
        if (pointer == IntPtr.Zero)
            return null;

        return FastNew.CreateInstance<T, IntPtr>(pointer);
    }

    /// <exception cref="EngineFeatureUnavailableException">Engine Feature <c>cssharp.items/can-acquire-hooks</c> is disabled</exception>
    public AcquireResult CanAcquire(CEconItemView itemView, AcquireMethod method, IntPtr unknown = 0)
    {
        return VirtualFunctions.CCSPlayer_ItemServices_CanAcquireFunc.Invoke(this, itemView, method, unknown);
    }
}
