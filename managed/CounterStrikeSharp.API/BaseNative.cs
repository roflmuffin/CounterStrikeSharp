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
using System.Text;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;

namespace CounterStrikeSharp.API
{
    public abstract class NativeObject
    {
        private IntPtr _handle;

        public IntPtr Handle
        {
            get
            {
                if (_handle == IntPtr.Zero)
                {
                    EnsureNativeHandle();
                }

                return _handle;
            }
            internal set => _handle = value;
        }

        internal IntPtr RawHandle => _handle;

        protected NativeObject(IntPtr pointer)
        {
            _handle = pointer;
        }

        protected void SetHandle(IntPtr pointer)
        {
            _handle = pointer;
        }

        protected virtual void EnsureNativeHandle()
        {
        }

        /// <summary>
        /// Returns a new instance of the specified type using the pointer from the passed in object.
        /// </summary>
        /// <remarks>
        /// Useful for creating a new instance of a class that inherits from NativeObject.
        /// e.g. <code>var weaponServices = playerWeaponServices.As&lt;CCSPlayer_WeaponServices&gt;();</code>
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>() where T : NativeObject
        {
            return (T)Activator.CreateInstance(typeof(T), this.Handle);
        }
    }
}
