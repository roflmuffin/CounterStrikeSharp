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

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CounterStrikeSharp.API.Modules.Utils
{
    [StructLayout(LayoutKind.Sequential, Pack = 4, Size = 12)]
    public struct QAngle
    {
        public float Pitch;
        public float Yaw;
        public float Roll;

        public QAngle(float pitch, float yaw, float roll)
        {
            Pitch = pitch;
            Yaw = yaw;
            Roll = roll;
        }

        public QAngle(QAngle other)
        {
            Pitch = other.Pitch;
            Yaw = other.Yaw;
            Roll = other.Roll;
        }

        public QAngle(float value) : this(value, value, value)
        {
        }

        public static QAngle Zero => new(0, 0, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QAngle operator +(QAngle left, QAngle right) =>
            new(left.Pitch + right.Pitch, left.Yaw + right.Yaw, left.Roll + right.Roll);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QAngle operator -(QAngle left, QAngle right) =>
            new(left.Pitch - right.Pitch, left.Yaw - right.Yaw, left.Roll - right.Roll);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QAngle operator *(QAngle left, QAngle right) =>
            new(left.Pitch * right.Pitch, left.Yaw * right.Yaw, left.Roll * right.Roll);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QAngle operator /(QAngle left, QAngle right) =>
            new(left.Pitch / right.Pitch, left.Yaw / right.Yaw, left.Roll / right.Roll);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QAngle operator *(QAngle left, float b) => new(left.Pitch * b, left.Yaw * b, left.Roll * b);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QAngle operator /(QAngle value1, float value2)
        {
            return value1 / new QAngle(value2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static QAngle operator -(QAngle value) => Zero - value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(QAngle left, QAngle right) =>
            left.Pitch == right.Pitch && left.Yaw == right.Yaw && left.Roll == right.Roll;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(QAngle left, QAngle right) => !(left == right);

        public override bool Equals(object? obj) => obj is QAngle angle && this == angle;
        public override int GetHashCode() => HashCode.Combine(Pitch, Yaw, Roll);
        public override string ToString() => $"QAngle({Pitch}, {Yaw}, {Roll})";
    }
}