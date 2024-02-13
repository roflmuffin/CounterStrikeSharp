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
using CounterStrikeSharp.API.Modules.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using CounterStrikeSharp.API.Core.Logging;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Entities;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API
{
    public static class Utilities
    {
        // https://github.com/dotabuff/manta/blob/master/entity.go#L186-L190
        public const int MaxEdictBits = 15;
        public const int MaxEdicts = 1 << MaxEdictBits;
        public const int NumEHandleSerialNumberBits = 17;
        public const uint InvalidEHandleIndex = 0xFFFFFFFF;

        public static IEnumerable<T> FlagsToList<T>(this T flags) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>()
                .Where(x => flags.HasFlag(x)).AsEnumerable();
        }

        public static T GetEntityFromIndex<T>(int index) where T : CEntityInstance
        {
            return (T)Activator.CreateInstance(typeof(T), NativeAPI.GetEntityFromIndex(index))!;
        }

        public static T? CreateEntityByName<T>(string name) where T : CBaseEntity
        {
            return (T?)Activator.CreateInstance(typeof(T), VirtualFunctions.UTIL_CreateEntityByName(name, -1));
        }

        public static CCSPlayerController GetPlayerFromIndex(int index)
        {
            return Utilities.GetEntityFromIndex<CCSPlayerController>(index);
        }

        public static CCSPlayerController GetPlayerFromSlot(int slot)
        {
            return Utilities.GetEntityFromIndex<CCSPlayerController>(slot + 1);
        }

        public static CCSPlayerController GetPlayerFromUserid(int userid)
        {
            return Utilities.GetEntityFromIndex<CCSPlayerController>((userid & 0xFF) + 1);
        }

        public static CCSPlayerController? GetPlayerFromSteamId(ulong steamId)
        {
            return Utilities.GetPlayers().FirstOrDefault(player => player.AuthorizedSteamID == (SteamID)steamId);
        }

        public static TargetResult ProcessTargetString(string pattern, CCSPlayerController player)
        {
            return new Target(pattern).GetTarget(player);
        }

        public static bool RemoveItemByDesignerName(this CCSPlayerController player, string designerName)
        {
            return RemoveItemByDesignerName(player, designerName, false);
        }

        public static bool RemoveItemByDesignerName(this CCSPlayerController player, string designerName, bool shouldRemoveEntity)
        {
            CHandle<CBasePlayerWeapon>? item = null;
            if (player.PlayerPawn.Value == null || player.PlayerPawn.Value.WeaponServices == null) return false;

            foreach(var weapon in player.PlayerPawn.Value.WeaponServices.MyWeapons)
            {
                if (weapon is not { IsValid: true, Value.IsValid: true }) 
                    continue;
                if (weapon.Value.DesignerName != designerName) 
                    continue;

                item = weapon;
            }
            
            if (item != null && item.Value != null)
            {
                player.PlayerPawn.Value.RemovePlayerItem(item.Value);

                if (shouldRemoveEntity)
                {
                    item.Value.Remove();
                }

                return true;
            }
            
            return false;
        }

        public static IEnumerable<T> FindAllEntitiesByDesignerName<T>(string designerName) where T : CEntityInstance
        {
            var pEntity = new CEntityIdentity(EntitySystem.FirstActiveEntity);
            for (; pEntity != null && pEntity.Handle != IntPtr.Zero; pEntity = pEntity.Next)
            {
                if (pEntity.DesignerName == null || !pEntity.DesignerName.Contains(designerName)) continue;
                yield return new PointerTo<T>(pEntity.Handle).Value;
            }
        }
        
        public static IEnumerable<CEntityInstance> GetAllEntities()
        {
            var pEntity = new CEntityIdentity(EntitySystem.FirstActiveEntity);
            for (; pEntity != null && pEntity.Handle != IntPtr.Zero; pEntity = pEntity.Next)
            {
                yield return new PointerTo<CEntityInstance>(pEntity.Handle).Value;
            }
        }
        
        /// <summary>
        /// Returns a list of <see cref="CCSPlayerController"/> that are valid and have a valid <see cref="CCSPlayerController.UserId"/> >= 0
        /// </summary>
        public static List<CCSPlayerController> GetPlayers()
        {
            List<CCSPlayerController> players = new();

            for (int i = 0; i < Server.MaxPlayers; i++)
            {
                var controller = GetPlayerFromSlot(i);

                if (!controller.IsValid || controller.UserId == -1)
                    continue;

                players.Add(controller);
            }

            return players;
        }

        [Obsolete]
        public static void ReplyToCommand(CCSPlayerController? player, string msg, bool console = false)
        {
            if (player != null)
            {
                if (console) player.PrintToConsole(msg);
                else player.PrintToChat(msg);
            }
            else
            {
                Server.PrintToConsole(msg);
            }
        }

        public static unsafe string ReadStringUtf8(IntPtr ptr)
        {
            unsafe
            {
                var nativeUtf8 = Unsafe.Read<IntPtr>((void*)ptr);

                if (nativeUtf8 == IntPtr.Zero)
                {
                    return null;
                }

                var len = 0;
                while (Marshal.ReadByte(nativeUtf8, len) != 0)
                {
                    ++len;
                }

                var buffer = new byte[len];
                Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer);
            }
        }

        public static T? GetPointer<T>(IntPtr pointer) where T : NativeObject
        {
            var pointerTo = Marshal.ReadIntPtr(pointer);
            if (pointerTo == IntPtr.Zero)
            {
                return null;
            }

            return (T)Activator.CreateInstance(typeof(T), pointerTo)!;
        }
        
        private static int FindSchemaChain(string className) => Schema.GetSchemaOffset(className, "__m_pChainEntity");

        /// <summary>
        /// Marks a field as changed for network transmission.
        /// Not all schema fields are network enabled, so please check the schema before using this.
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <param name="className" example="CBaseEntity">Schema field class name</param>
        /// <param name="fieldName" example="m_iHealth">Schema field name</param>
        /// <param name="extraOffset">Any additional offset to the schema field</param>
        public static void SetStateChanged(CBaseEntity entity, string className, string fieldName, int extraOffset = 0)
        {
            if (!Schema.IsSchemaFieldNetworked(className, fieldName))
            {
                Application.Instance.Logger.LogWarning("Field {ClassName}:{FieldName} is not networked, but SetStateChanged was called on it.", className, fieldName);
                return;
            }
            
            int offset = Schema.GetSchemaOffset(className, fieldName);
            int chainOffset = FindSchemaChain(className);

            if (chainOffset != 0)
            {
                VirtualFunctions.NetworkStateChanged(entity.Handle + chainOffset, offset + extraOffset, 0xFFFFFFFF);
                return;
            }

            VirtualFunctions.StateChanged(entity.NetworkTransmitComponent.Handle, entity.Handle, offset + extraOffset, -1, -1);

            entity.LastNetworkChange = Server.CurrentTime;
            entity.IsSteadyState.Clear();
        }
    }
}