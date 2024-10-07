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

namespace CounterStrikeSharp.API.Modules.Memory
{
    public static class MemAlloc
    {
        /// <summary>
        /// Indirect allocation using 'MemAlloc'
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static nint Allocate(int size)
        {
            return NativeAPI.MemAllocAllocate(size);
        }

        /// <summary>
        /// Release pointer using 'MemAlloc'
        /// </summary>
        /// <param name="ptr"></param>
        public static void Free(nint ptr)
        {
            NativeAPI.MemAllocFreePointer(ptr);
        }
    }
}
