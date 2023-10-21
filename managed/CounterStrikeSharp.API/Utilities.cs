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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        public static List<CEntityInstance> FindAllEntitiesByDesignerName(string designerName)
        {
            var entList = new List<CEntityInstance>();
            for (int i = 0; i < MaxEdicts; i++)
            {
                var entPtr = NativeAPI.GetEntityFromIndex(i);
                if (entPtr == IntPtr.Zero) continue;
                var ent = new CEntityInstance(entPtr);
                if (!ent.DesignerName.Contains(designerName)) continue;

                entList.Add(ent);
            }
            return entList; 
        }
    }
}
