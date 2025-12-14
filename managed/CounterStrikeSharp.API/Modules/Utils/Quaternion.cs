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

using System.Runtime.CompilerServices;

namespace CounterStrikeSharp.API.Modules.Utils
{
    public class Quaternion : NativeObject
    {
        // Not sure who made this one? maybe mark it as 'obsolete' to don't break existing plugins but warn them?
        public unsafe ref float Value => ref Unsafe.Add(ref *(float*)Handle.ToPointer(), 0);

        public unsafe ref float X => ref Unsafe.Add(ref *(float*)Handle.ToPointer(), 0);

        public unsafe ref float Y => ref Unsafe.Add(ref *(float*)Handle.ToPointer(), 1);

        public unsafe ref float Z => ref Unsafe.Add(ref *(float*)Handle.ToPointer(), 2);

        public unsafe ref float W => ref Unsafe.Add(ref *(float*)Handle.ToPointer(), 3);

        public Quaternion(IntPtr pointer) : base(pointer)
        {
        }

        public Quaternion(float? x = null, float? y = null, float? z = null, float? w = null) : this(NativeAPI.QuaternionNew())
        {
            this.X = x ?? 0;
            this.Y = y ?? 0;
            this.Z = z ?? 0;
            this.W = w ?? 0;
        }

        public override string ToString()
        {
            return $"{X:n2} {Y:n2} {Z:n2} {W:n2}";
        }
    }
}