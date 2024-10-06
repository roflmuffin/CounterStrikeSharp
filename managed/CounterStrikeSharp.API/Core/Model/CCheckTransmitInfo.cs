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

using System.Runtime.InteropServices;

namespace CounterStrikeSharp.API.Core
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CCheckTransmitInfo
    {
        /// <summary>
        /// Entity n is already marked for transmission
        /// </summary>
        public CFixedBitVecBase TransmitEntity;
    };

    public sealed class CCheckTransmitInfoList : NativeObject
    {
        private int CheckTransmitPlayerSlotOffset = GameData.GetOffset("CheckTransmitPlayerSlot");

        private unsafe nint* Inner => (nint*)base.Handle;

        public unsafe CCheckTransmitInfoList(IntPtr pointer) : base(pointer)
            { }

        /// <summary>
        /// Get transmit info for the given index.
        /// </summary>
        /// <param name="index">Index of the info you want to retrieve from the list, should be between 0 and 'infoCount' - 1</param>
        /// <returns></returns>
        public unsafe (CCheckTransmitInfo, int) Get(int index)
        {
            return (Marshal.PtrToStructure<CCheckTransmitInfo>(this.Inner[index]), (int)(*(byte*)(this.Inner[index] + CheckTransmitPlayerSlotOffset)));
        }
    }
}
