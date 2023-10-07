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

using System;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Players
{
    public class PlayerInfo : NativeObject
    {
        internal PlayerInfo(IntPtr cHandle) : base(cHandle)
        {
        }

        public string Name => NativeAPI.PlayerinfoGetName(Handle);

        public int UserId => NativeAPI.PlayerinfoGetUserid(Handle);
        public string SteamId => NativeAPI.PlayerinfoGetSteamid(Handle);

        public Team Team
        {
            get => (Team)NativeAPI.PlayerinfoGetTeam(Handle);
            set => NativeAPI.PlayerinfoSetTeam(Handle, (int) value);
        }

        public int Kills => NativeAPI.PlayerinfoGetKills(Handle);
        public int Deaths => NativeAPI.PlayerinfoGetKills(Handle);

        public bool IsConnected => NativeAPI.PlayerinfoIsConnected(Handle);

        public int Armor => NativeAPI.PlayerinfoGetArmor(Handle);
        public bool IsHLTV => NativeAPI.PlayerinfoIsHltv(Handle);
        public bool IsPlayer => NativeAPI.PlayerinfoIsPlayer(Handle);
        public bool IsFakeClient => NativeAPI.PlayerinfoIsFakeclient(Handle);
        public bool IsDead => NativeAPI.PlayerinfoIsDead(Handle);
        public bool IsInAVehicle => NativeAPI.PlayerinfoIsInVehicle(Handle);
        public bool IsObserver => NativeAPI.PlayerinfoIsObserver(Handle);
        /*public Angle Origin => NativePINVOKE.IPlayerInfo_GetAbsOrigin(Handle).ToObject<Angle>();
        public Angle Angles => NativePINVOKE.IPlayerInfo_GetAbsAngles(Handle).ToObject<Angle>();
        public Angle MinSize => NativePINVOKE.IPlayerInfo_GetPlayerMins(Handle).ToObject<Angle>();
        public Angle MaxSize => NativePINVOKE.IPlayerInfo_GetPlayerMaxs(Handle).ToObject<Angle>();*/
        public string WeaponName => NativeAPI.PlayerinfoGetWeaponName(Handle);
        public string ModelName => NativeAPI.PlayerinfoGetModelName(Handle);
        public int Health => NativeAPI.PlayerinfoGetHealth(Handle);
        public int MaxHealth => NativeAPI.PlayerinfoGetMaxHealth(Handle);
        //public CBotCmd LastUserCommand => NativePINVOKE.IPlayerInfo_GetLastUserCommand(Handle);
    }
}