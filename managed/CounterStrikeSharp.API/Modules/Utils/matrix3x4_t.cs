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
using System.Runtime.InteropServices;
using System.Threading;

namespace CounterStrikeSharp.API.Modules.Utils;

public partial class matrix3x4_t : NativeObject
{
    private float _m00;
    private float _m01;
    private float _m02;
    private float _m03;
    private float _m10;
    private float _m11;
    private float _m12;
    private float _m13;
    private float _m20;
    private float _m21;
    private float _m22;
    private float _m23;
    private IntPtr _ownedHandle;

    public unsafe ref float this[int row, int column] => ref GetElementRef(row * 4 + column);

    private unsafe ref float GetElementRef(int index)
    {
        var handle = RawHandle;
        if (handle != IntPtr.Zero)
        {
            return ref Unsafe.Add(ref *(float*)handle, index);
        }

        switch (index)
        {
            case 0:
                return ref _m00;
            case 1:
                return ref _m01;
            case 2:
                return ref _m02;
            case 3:
                return ref _m03;
            case 4:
                return ref _m10;
            case 5:
                return ref _m11;
            case 6:
                return ref _m12;
            case 7:
                return ref _m13;
            case 8:
                return ref _m20;
            case 9:
                return ref _m21;
            case 10:
                return ref _m22;
            case 11:
                return ref _m23;
            default:
                throw new ArgumentOutOfRangeException(nameof(index));
        }
    }

    public float M00 => this[0, 0];

    public float M01 => this[0, 1];

    public float M02 => this[0, 2];

    public float M03 => this[0, 3];

    public float M10 => this[1, 0];

    public float M11 => this[1, 1];

    public float M12 => this[1, 2];

    public float M13 => this[1, 3];

    public float M20 => this[2, 0];

    public float M21 => this[2, 1];

    public float M22 => this[2, 2];

    public float M23 => this[2, 3];

    public matrix3x4_t(IntPtr pointer) : base(pointer)
    {
    }

    public matrix3x4_t(float? m00 = null, float? m01 = null, float? m02 = null, float? m03 = null,
        float? m10 = null, float? m11 = null, float? m12 = null, float? m13 = null,
        float? m20 = null, float? m21 = null, float? m22 = null, float? m23 = null) : base(IntPtr.Zero)
    {
        this[0, 0] = m00 ?? 0;
        this[0, 1] = m01 ?? 0;
        this[0, 2] = m02 ?? 0;
        this[0, 3] = m03 ?? 0;

        this[1, 0] = m10 ?? 0;
        this[1, 1] = m11 ?? 0;
        this[1, 2] = m12 ?? 0;
        this[1, 3] = m13 ?? 0;

        this[2, 0] = m20 ?? 0;
        this[2, 1] = m21 ?? 0;
        this[2, 2] = m22 ?? 0;
        this[2, 3] = m23 ?? 0;
    }

    protected override void EnsureNativeHandle()
    {
        if (RawHandle != IntPtr.Zero)
        {
            return;
        }

        if (_ownedHandle != IntPtr.Zero)
        {
            SetHandle(_ownedHandle);
            return;
        }

        var allocated = Marshal.AllocHGlobal(sizeof(float) * 12);

        unsafe
        {
            var buffer = (float*)allocated;
            buffer[0] = _m00;
            buffer[1] = _m01;
            buffer[2] = _m02;
            buffer[3] = _m03;
            buffer[4] = _m10;
            buffer[5] = _m11;
            buffer[6] = _m12;
            buffer[7] = _m13;
            buffer[8] = _m20;
            buffer[9] = _m21;
            buffer[10] = _m22;
            buffer[11] = _m23;
        }

        var existing = Interlocked.CompareExchange(ref _ownedHandle, allocated, IntPtr.Zero);
        if (existing != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(allocated);
            SetHandle(existing);
            return;
        }

        NativeHandleTracker.Track(this, allocated);
        SetHandle(allocated);
    }

    public override string ToString()
    {
        return $"{this[0, 0]:n2} {this[0, 1]:n2} {this[0, 2]:n2} {this[0, 3]:n2}\n{this[1, 0]:n2} {this[1, 1]:n2} {this[1, 2]:n2} {this[1, 3]:n2}\n{this[2, 0]:n2} {this[2, 1]:n2} {this[2, 2]:n2} {this[2, 3]:n2}";
    }
}
