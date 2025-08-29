using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using System;
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
        // Max entity bits & constants
        public const int MaxEdictBits = 15;
        public const int MaxEdicts = 1 << MaxEdictBits;
        public const int NumEHandleSerialNumberBits = 17;
        public const uint InvalidEHandleIndex = 0xFFFFFFFF;

        // Convert enum flags to list
        public static IEnumerable<T> FlagsToList<T>(this T flags) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Where(x => flags.HasFlag(x));
        }

        // Entity retrieval
        public static T? GetEntityFromIndex<T>(int index) where T : CEntityInstance
        {
            var entityPtr = EntitySystem.GetEntityByIndex((uint)index);
            if (entityPtr == IntPtr.Zero) return null;
            return (T)Activator.CreateInstance(typeof(T), entityPtr)!;
        }

        public static T? CreateEntityByName<T>(string name) where T : CBaseEntity
        {
            return (T?)Activator.CreateInstance(typeof(T), VirtualFunctions.UTIL_CreateEntityByName(name, -1));
        }

        // Player retrieval
        public static CCSPlayerController? GetPlayerFromIndex(int index)
        {
            var player = GetEntityFromIndex<CCSPlayerController>(index);
            return player?.DesignerName == "cs_player_controller" ? player : null;
        }

        public static CCSPlayerController? GetPlayerFromSlot(int slot) => GetPlayerFromIndex(slot + 1);
        public static CCSPlayerController? GetPlayerFromUserid(int userid) => GetPlayerFromIndex((userid & 0xFF) + 1);
        public static CCSPlayerController? GetPlayerFromSteamId(ulong steamId) =>
            GetPlayers().FirstOrDefault(player => player.AuthorizedSteamID == (SteamID)steamId);

        // Targeting
        public static TargetResult ProcessTargetString(string pattern, CCSPlayerController player) =>
            new Target(pattern).GetTarget(player);

        // Item removal
        public static bool RemoveItemByDesignerName(this CCSPlayerController player, string designerName) =>
            RemoveItemByDesignerName(player, designerName, false);

        public static bool RemoveItemByDesignerName(this CCSPlayerController player, string designerName, bool shouldRemoveEntity)
        {
            if (player.PlayerPawn.Value?.WeaponServices == null) return false;

            CHandle<CBasePlayerWeapon>? item = null;

            foreach (var weapon in player.PlayerPawn.Value.WeaponServices.MyWeapons)
            {
                if (weapon?.IsValid != true || !weapon.Value.IsValid) continue;
                if (weapon.Value.DesignerName != designerName) continue;
                item = weapon;
            }

            if (item?.Value != null)
            {
                player.PlayerPawn.Value.RemovePlayerItem(item.Value);
                if (shouldRemoveEntity) item.Value.Remove();
                return true;
            }

            return false;
        }

        // Entity searches
        public static IEnumerable<T> FindAllEntitiesByDesignerName<T>(string designerName) where T : CEntityInstance
        {
            for (var pEntity = new CEntityIdentity(EntitySystem.FirstActiveEntity);
                 pEntity != null && pEntity.Handle != IntPtr.Zero;
                 pEntity = pEntity.Next)
            {
                if (pEntity.DesignerName?.Contains(designerName) != true) continue;
                yield return new PointerTo<T>(pEntity.Handle).Value;
            }
        }

        public static IEnumerable<CEntityInstance> GetAllEntities()
        {
            for (var pEntity = new CEntityIdentity(EntitySystem.FirstActiveEntity);
                 pEntity != null && pEntity.Handle != IntPtr.Zero;
                 pEntity = pEntity.Next)
            {
                yield return new PointerTo<CEntityInstance>(pEntity.Handle).Value;
            }
        }

        // Player list
        public static List<CCSPlayerController> GetPlayers()
        {
            var players = new List<CCSPlayerController>();

            for (int i = 0; i < Server.MaxPlayers; i++)
            {
                var controller = GetPlayerFromSlot(i);
                if (controller == null || !controller.IsValid || controller.Connected != PlayerConnectedState.PlayerConnected) continue;
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

        // Pointer & string helpers
        public static unsafe string ReadStringUtf8(IntPtr ptr)
        {
            unsafe
            {
                var nativeUtf8 = Unsafe.Read<IntPtr>((void*)ptr);
                if (nativeUtf8 == IntPtr.Zero) return null;

                int len = 0;
                while (Marshal.ReadByte(nativeUtf8, len) != 0) len++;

                var buffer = new byte[len];
                Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
                return Encoding.UTF8.GetString(buffer);
            }
        }

        public static T? GetPointer<T>(IntPtr pointer) where T : NativeObject
        {
            var ptr = Marshal.ReadIntPtr(pointer);
            if (ptr == IntPtr.Zero) return null;
            return (T)Activator.CreateInstance(typeof(T), ptr)!;
        }

        private static int FindSchemaChain(string className) => Schema.GetSchemaOffset(className, "__m_pChainEntity");

        // Networked state update
        public static void SetStateChanged(CBaseEntity entity, string className, string fieldName, int extraOffset = 0)
        {
            Guard.IsValidEntity(entity);

            if (!Schema.IsSchemaFieldNetworked(className, fieldName))
            {
                Application.Instance.Logger.LogWarning(
                    "Field {ClassName}:{FieldName} is not networked, but SetStateChanged was called on it.", className, fieldName);
                return;
            }

            int offset = Schema.GetSchemaOffset(className, fieldName);
            int chainOffset = FindSchemaChain(className);

            if (chainOffset != 0)
                NativeAPI.SchemaNetworkStateChanged(entity.Handle + chainOffset, (uint)(offset + extraOffset), 0xFFFFFFFF, 0xFFFFFFFF);
            else
                NativeAPI.SchemaSetStateChanged(entity.Handle, (uint)(offset + extraOffset), 0xFFFFFFFF, 0xFFFFFFFF);

            entity.LastNetworkChange = Server.CurrentTime;
            entity.IsSteadyState.Clear();
        }

        // Metamod API pointer
        public static IntPtr? MetaFactory(string interfaceName)
        {
            var ptr = NativeAPI.MetaFactory(interfaceName);
            return ptr != IntPtr.Zero ? ptr : null;
        }
    }
}
