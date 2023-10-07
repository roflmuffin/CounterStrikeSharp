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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Listeners;

namespace CounterStrikeSharp.API
{
    public static class Utilities
    {

        public static IEnumerable<T> FlagsToList<T>(this T flags) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>()
                .Where(x => flags.HasFlag(x)).AsEnumerable();
        }
    }
}
