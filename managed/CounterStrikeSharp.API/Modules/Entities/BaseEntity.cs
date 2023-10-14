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

namespace CounterStrikeSharp.API.Modules.Entities
{
    public class BaseEntity : NativeObject
    {
        public int Index { get; init; }

        public BaseEntity(int index, IntPtr handle) : base(handle)
        {
            Index = index;
        }

        public override string ToString()
        {
            return string.Format($"[BaseEntity index={Index}, handle={Handle}");
        }
    }
}