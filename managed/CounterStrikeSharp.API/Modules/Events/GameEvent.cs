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

        public bool GetBool(string name) => NativeAPI.GetEventBool(Handle, name);
        public float GetFloat(string name) => NativeAPI.GetEventFloat(Handle, name);
        public string GetString(string name) => NativeAPI.GetEventString(Handle, name);
        public int GetInt(string name) => NativeAPI.GetEventInt(Handle, name);

        public ulong GetUint64(string name) => 0;

        public void SetUint64(string name, ulong value)
        {
        }

        // public Player GetPlayer(string name) => Player.FromUserId(GetInt(name));

        public void SetBool(string name, bool value) => NativeAPI.SetEventBool(Handle, name, value);
        public void SetFloat(string name, float value) => NativeAPI.SetEventFloat(Handle, name, value);
        public void SetString(string name, string value) => NativeAPI.SetEventString(Handle, name, value);
        public void SetInt(string name, int value) => NativeAPI.SetEventInt(Handle, name, value);
        public void SetInt(string name, long value) => SetInt(name, (int)value);

        public void FireEvent(bool dontBroadcast) => NativeAPI.FireEvent(Handle, dontBroadcast);
        // public void FireEventToClient(int clientId, bool dontBroadcast) => NativeAPI.FireEventToClient(Handle, clientId);
    }
}