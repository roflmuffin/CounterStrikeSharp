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
    // credits: qstage
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct CFixedBitVecBase
    {
        private const int LOG2_BITS_PER_INT = 5;
        private const int BITS_PER_INT = 32;

        private readonly uint* m_Ints;

        public void Add(CEntityInstance entityInstance) => Write(entityInstance.Index);

        public void Add(int bitNum) => Write(bitNum);

        public void Add(uint bitNum) => Write(bitNum);

        public void Remove(CEntityInstance entityInstance) => Clear(entityInstance.Index);

        public void Remove(int bitNum) => Clear(bitNum);

        public void Remove(uint bitNum) => Clear(bitNum);

        public bool Contains(CEntityInstance entityInstance) => Contains(entityInstance.Index);

        public bool Contains(uint bitNum) => Contains((int)bitNum);

        private void Clear(uint bitNum) => Clear((int)bitNum);

        private void Write(uint bitNum) => Write((int)bitNum);

        private void Clear(int bitNum)
        {
            if (!(bitNum >= 0 && bitNum < Utilities.MaxEdicts))
                return;

            uint* pInt = m_Ints + BitVec_Int(bitNum);
            *pInt &= ~(uint)BitVec_Bit(bitNum);
        }

        private void Write(int bitNum)
        {
            if (!(bitNum >= 0 && bitNum < Utilities.MaxEdicts))
                return;

            uint* pInt = m_Ints + BitVec_Int(bitNum);
            *pInt |= (uint)BitVec_Bit(bitNum);
        }

        public bool Contains(int bitNum)
        {
            if (!(bitNum >= 0 && bitNum < Utilities.MaxEdicts))
                return false;

            uint* pInt = m_Ints + BitVec_Int(bitNum);
            return (*pInt & (uint)BitVec_Bit(bitNum)) != 0;
        }

        private int BitVec_Int(int bitNum) => bitNum >> LOG2_BITS_PER_INT;

        private int BitVec_Bit(int bitNum) => 1 << (bitNum & (BITS_PER_INT - 1));
    }
}
