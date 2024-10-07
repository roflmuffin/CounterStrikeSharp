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
        /// Indirect allocation using 'MemAlloc'
        /// </summary>
        /// <param name="size"></param>
        /// <param name="align"></param>
        /// <returns></returns>
        public static nint AllocateAligned(int size, int align)
        {
            return NativeAPI.MemAllocAllocateAligned(size, align);
        }

        public static nint ReallocateAligned(nint pointer, int size, int align)
        {
            return NativeAPI.MemAllocReallocateAligned(pointer, size, align);
        }

        public static int GetSizeAligned(nint pointer)
        {
            return NativeAPI.MemAllocGetsizeAligned(pointer);
        }

        /// <summary>
        /// Release pointer using 'MemAlloc'
        /// </summary>
        /// <param name="ptr"></param>
        public static void Free(nint ptr)
        {
            NativeAPI.MemAllocFreePointer(ptr);
        }

        /// <summary>
        /// Release pointer using 'MemAlloc'
        /// </summary>
        /// <param name="ptr"></param>
        public static void FreeAligned(nint ptr)
        {
            NativeAPI.MemAllocFreePointerAligned(ptr);
        }
    }
}
