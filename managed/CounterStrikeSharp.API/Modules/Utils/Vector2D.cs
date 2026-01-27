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

namespace CounterStrikeSharp.API.Modules.Utils
{
    public class Vector2D : NativeObject
    {
        private float _x;
        private float _y;
        private IntPtr _ownedHandle;

        public unsafe ref float X => ref GetElementRef(0);

        public unsafe ref float Y => ref GetElementRef(1);

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
                    return ref _x;
                case 1:
                    return ref _y;
                default:
                    throw new ArgumentOutOfRangeException(nameof(index));
            }
        }

        public Vector2D(IntPtr pointer) : base(pointer)
        {
        }

        public Vector2D(float? x = null, float? y = null) : base(IntPtr.Zero)
        {
            _x = x ?? 0;
            _y = y ?? 0;
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

            var allocated = Marshal.AllocHGlobal(sizeof(float) * 2);

            unsafe
            {
                var buffer = (float*)allocated;
                buffer[0] = _x;
                buffer[1] = _y;
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
            return $"{X:n2} {Y:n2}";
        }
    }
}
