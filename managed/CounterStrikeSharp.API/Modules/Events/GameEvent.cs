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
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Events
{
    public class EventAttribute : Attribute
    {
        public EventAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class GameEvent
    {
        public IntPtr Handle { get; internal set; }

        protected GameEvent()
        {
        }

        public GameEvent(string name, bool force) : this(NativeAPI.CreateEvent(name, force))
        {
        }
        
        internal GameEvent(IntPtr pointer)
        {
            Handle = pointer;
        }

        public string EventName => NativeAPI.GetEventName(Handle);

        public T Get<T>(string name)
        {
            var type = typeof(T);
            object result = type switch
            {
                _ when type == typeof(float) => GetFloat(name),
                _ when type == typeof(int) => GetInt(name),
                _ when type == typeof(string) => GetString(name),
                _ when type == typeof(bool) => GetBool(name),
                _ when type == typeof(ulong) => GetUint64(name),
                _ when type == typeof(IntPtr) => throw new NotImplementedException("IntPtr event arguments are not supported yet."),
                _ => throw new NotSupportedException(),
            };

            return (T)result;
        }
        
        public void Set<T>(string name, T value)
        {
            var type = typeof(T);
            switch (type)
            {
                case var _ when value is float f:
                    SetFloat(name, f);
                    break;
                case var _ when value is int i:
                    SetInt(name, i);
                    break;
                case var _ when value is IntPtr:
                    throw new NotImplementedException("IntPtr event arguments are not supported yet.");
                case var _ when value is string s:
                    SetString(name, s);
                    break;
                case var _ when value is bool b:
                    SetBool(name, b);
                    break;
                case var _ when value is ulong ul:
                    SetUint64(name, ul);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        protected bool GetBool(string name) => NativeAPI.GetEventBool(Handle, name);
        protected float GetFloat(string name) => NativeAPI.GetEventFloat(Handle, name);
        protected string GetString(string name) => NativeAPI.GetEventString(Handle, name);
        protected int GetInt(string name) => NativeAPI.GetEventInt(Handle, name);

        protected ulong GetUint64(string name) => 0;

        protected void SetUint64(string name, ulong value)
        {
        }

        // public Player GetPlayer(string name) => Player.FromUserId(GetInt(name));

        protected void SetBool(string name, bool value) => NativeAPI.SetEventBool(Handle, name, value);
        protected void SetFloat(string name, float value) => NativeAPI.SetEventFloat(Handle, name, value);
        protected void SetString(string name, string value) => NativeAPI.SetEventString(Handle, name, value);
        protected void SetInt(string name, int value) => NativeAPI.SetEventInt(Handle, name, value);
        protected void SetInt(string name, long value) => SetInt(name, (int)value);

        public void FireEvent(bool dontBroadcast) => NativeAPI.FireEvent(Handle, dontBroadcast);
        // public void FireEventToClient(int clientId, bool dontBroadcast) => NativeAPI.FireEventToClient(Handle, clientId);
    }
}