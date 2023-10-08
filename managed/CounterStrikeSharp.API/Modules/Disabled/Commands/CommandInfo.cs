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

namespace CounterStrikeSharp.API.Modules.Commands
{
    public class CommandInfo
    {
        public delegate void CommandCallback(int client, CommandInfo commandInfo);

        public IntPtr Handle { get; private set; }
        internal CommandInfo(IntPtr pointer)
        {
            Handle = pointer;
        }

        public int ArgCount() => NativeAPI.CommandGetArgCount(Handle);

        public string ArgString() => NativeAPI.CommandGetArgString(Handle);

        public string GetCommandString() => NativeAPI.CommandGetCommandString(Handle);

        public string ArgByIndex(int index) => NativeAPI.CommandGetArgByIndex(Handle, index);
    }
}