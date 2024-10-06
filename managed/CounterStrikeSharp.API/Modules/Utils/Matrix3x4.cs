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

namespace CounterStrikeSharp.API.Modules.Utils
{
    /// <summary>
    /// Matrix3x4 for <see cref="CEntityKeyValues"/>
    /// </summary>
    public class Matrix3x4
    {
        public float M11;
        public float M12;
        public float M13;
        public float M14;
        public float M21;
        public float M22;
        public float M23;
        public float M24;
        public float M31;
        public float M32;
        public float M33;
        public float M34;

        public Matrix3x4() { }

        public Matrix3x4(float M11, float M12, float M13, float M14, float M21, float M22, float M23, float M24, float M31, float M32, float M33, float M34)
        {
            this.M11 = M11; this.M12 = M12; this.M13 = M13; this.M14 = M14; this.M21 = M21; this.M22 = M22; this.M23 = M23; this.M24 = M24; this.M31 = M31; this.M32 = M32; this.M33 = M33; this.M34 = M34;
        }
    }
}
