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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Listeners;

namespace CounterStrikeSharp.API.Modules.Cvars
{
    public class ConVar
    {
        public delegate void ConVarChangedCallback(ConVar convar, string oldValue, string newValue);

        public IntPtr Handle { get; private set; }

        internal ConVar(IntPtr handle)
        {
            Handle = handle;
        }

        public ConVar(string name, string value, string description, ConVarFlags flags, bool hasMinValue, float minValue, bool hasMaxValue, float maxValue) : 
            this(NativeAPI.CreateConvar(name, value, description, (int)flags, hasMinValue, minValue, hasMaxValue, maxValue))
        {
        }

        public ConVar(string name, string value, string description = null, ConVarFlags flags = ConVarFlags.None, float? minValue = null, float? maxValue = null) : 
            this(name, value, description, flags, minValue.HasValue, minValue?? 0, maxValue.HasValue, maxValue?? 0)
        {

        }

        public string Name => NativeAPI.ConvarGetName(Handle);

        public string StringValue
        {
            get { return NativeAPI.ConvarGetStringValue(Handle); }
            set { NativeAPI.ConvarSetStringValue(Handle, value); }
        }

        public float FloatValue
        {
            get { return Convert.ToSingle(NativeAPI.ConvarGetStringValue(Handle)); }
            set { NativeAPI.ConvarSetStringValue(Handle, value.ToString("n2")); }
        }

        public int IntValue
        {
            get { return Convert.ToInt32(NativeAPI.ConvarGetStringValue(Handle)); }
            set { NativeAPI.ConvarSetStringValue(Handle, value.ToString()); }
        }

        public bool BoolValue
        {
            get { return NativeAPI.ConvarGetStringValue(Handle) == "1"; }
            set { NativeAPI.ConvarSetStringValue(Handle, value ? "1" : "0"); }
        }

        public ConVarFlags Flags
        {
            get { return (ConVarFlags) NativeAPI.ConvarGetFlags(Handle); }
            set { NativeAPI.ConvarSetFlags(Handle, (int)value); }
        }

        public bool Public
        {
            get { return Flags.HasFlag(ConVarFlags.Notify); }
            set
            {
                if (value)
                {
                    Flags |= ConVarFlags.Notify;
                }
                else
                {
                    Flags &= ~ConVarFlags.Notify;
                }
            }
        }

        public static ConVar Find(string name)
        {
            var ptr = NativeAPI.FindConvar(name);
            if (ptr == IntPtr.Zero) return null;

            return new ConVar(ptr);
        }

        public void Unregister()
        {
            NativeAPI.ConvarUnregister(Handle);
        }
    }
}
