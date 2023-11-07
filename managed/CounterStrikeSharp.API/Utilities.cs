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

using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace CounterStrikeSharp.API
{
    public static class Utilities
    {
        // https://github.com/dotabuff/manta/blob/master/entity.go#L186-L190
        public const int MaxEdictBits = 14;
        public const int MaxEdicts = 1 << MaxEdictBits;
        public const int NumEHandleSerialNumberBits = 17;

        public static IEnumerable<T> FlagsToList<T>(this T flags) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>()
                .Where(x => flags.HasFlag(x)).AsEnumerable();
        }

        public static T GetEntityFromIndex<T>(int index) where T : CEntityInstance
        {
            return (T)Activator.CreateInstance(typeof(T), NativeAPI.GetEntityFromIndex(index))!;
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

        public static IEnumerable<T> FindAllEntitiesByDesignerName<T>(string designerName) where T : CEntityInstance
        {
            var pEntity = new CEntityIdentity(NativeAPI.GetFirstActiveEntity());
            for (; pEntity.Handle != IntPtr.Zero; pEntity = pEntity.Next.Value)
            {
                if (!pEntity.DesignerName.Contains(designerName)) continue;
                yield return new PointerTo<T>(pEntity.Handle).Value;
            }
        }
        
        /// <summary>
        /// Returns a list of <see cref="CCSPlayerController"/> that are valid and have a valid <see cref="CCSPlayerController.UserId"/> >= 0
        /// </summary>
        public static List<CCSPlayerController> GetPlayers()
        {
            List<CCSPlayerController> players = new();

            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                var controller = GetPlayerFromIndex(i);

                if (!controller.IsValid || controller.UserId < 0)
                    continue;

                players.Add(controller);
            }

            return players;
        }

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
    }
}