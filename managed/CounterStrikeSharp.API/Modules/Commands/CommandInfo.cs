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
        
        /// <summary>
        /// Command listener callback.
        /// <returns>If returning <see cref="HookResult.Handled"/> or higher, will prevent the command from executing.</returns>
        /// </summary>
        public delegate HookResult CommandListenerCallback(CCSPlayerController? player, CommandInfo commandInfo);

        public CCSPlayerController? CallingPlayer { get; }
        
        public IntPtr Handle { get; }
        
        internal CommandInfo(IntPtr pointer, CCSPlayerController? player) 
        {
            Handle = pointer;
            CallingPlayer = player;
        }

        public int ArgCount => NativeAPI.CommandGetArgCount(Handle);

        public string ArgString => NativeAPI.CommandGetArgString(Handle);

        public string GetCommandString => NativeAPI.CommandGetCommandString(Handle);

        public string ArgByIndex(int index) => NativeAPI.CommandGetArgByIndex(Handle, index);
        public string GetArg(int index) => NativeAPI.CommandGetArgByIndex(Handle, index);
        
        /// <summary>
        /// Whether or not the command was sent via Console or Chat.
        /// </summary>
        public CommandCallingContext CallingContext => NativeAPI.CommandGetCallingContext(Handle);
        
        [Obsolete("Console parameter is now automatically set based on the context of the command.", true)]
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

        /// <summary>
        /// Replies to the command with a message.
        /// <remarks>
        /// If the command was sent via Chat, <see cref="CCSPlayerController.PrintToChat"/> is used, otherwise <see cref="CCSPlayerController.PrintToConsole"/> is used.
        /// If sent from the server console/RCON, <see cref="Server.PrintToConsole"/> is used.
        /// </remarks>
        /// </summary>
        /// <param name="message">Message to send</param>
        public void ReplyToCommand(string message)
        {
            if (CallingPlayer != null) 
            {
                if (CallingContext == CommandCallingContext.Console) { CallingPlayer.PrintToConsole(message); }
                else CallingPlayer.PrintToChat(message);
            }
            else 
            {
                Server.PrintToConsole(message);    
            }
        }
    }
}