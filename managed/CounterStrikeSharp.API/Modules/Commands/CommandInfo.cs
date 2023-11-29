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
        public delegate void CommandCallback(CCSPlayerController? player, CommandInfo commandInfo);
        
        public delegate HookResult CommandListenerCallback(CCSPlayerController? player, CommandInfo commandInfo);

        public CCSPlayerController? CallingPlayer { get; }
        
        public IntPtr Handle { get; }
        
        internal CommandInfo(IntPtr pointer, CCSPlayerController player) 
        {
            Handle = pointer;
            CallingPlayer = player;
        }

        public int ArgCount => NativeAPI.CommandGetArgCount(Handle);

        public string ArgString => NativeAPI.CommandGetArgString(Handle);

        public string GetCommandString => NativeAPI.CommandGetCommandString(Handle);

        public string ArgByIndex(int index) => NativeAPI.CommandGetArgByIndex(Handle, index);
        public string GetArg(int index) => NativeAPI.CommandGetArgByIndex(Handle, index);
        
        public void ReplyToCommand(string message, bool console = false) {
            if (CallingPlayer != null) 
            {
                if (console) { CallingPlayer.PrintToConsole(message); }
                else CallingPlayer.PrintToChat(message);
            }
            else 
            {
                Server.PrintToConsole(message);    
            }
        }
    }
}