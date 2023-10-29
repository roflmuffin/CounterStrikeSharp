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
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Utils
{
    public class QAngle : NativeObject
    {
        public QAngle(IntPtr pointer) : base(pointer)
        {
        }
        
        public QAngle(float? x = null, float? y = null, float? z = null) : this(NativeAPI.AngleNew())
        {
            this.X = x ?? 0;
            this.Y = y ?? 0;
            this.Z = z ?? 0;
        }

        public unsafe ref float X => ref Unsafe.Add(ref *(float*)Handle.ToPointer(), 0);
        public unsafe ref float Y => ref Unsafe.Add(ref *(float*)Handle, 1);
        public unsafe ref float Z => ref Unsafe.Add(ref *(float*)Handle, 2);
        
        public override string ToString()
        {
            return $"{X:n2} {Y:n2} {Z:n2}";
        }
    }
}