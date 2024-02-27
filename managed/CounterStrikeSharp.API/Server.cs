﻿/*
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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API
{
    public class Server
    {
        public static float TickInterval => NativeAPI.GetTickInterval();

        public static void ExecuteCommand(string command) => NativeAPI.IssueServerCommand(command);

        public static string MapName => NativeAPI.GetMapName();
        // public static void PrintToConsole(string message) => NativeAPI.PrintToConsole(message);

        public static double TickedTime => NativeAPI.GetTickedTime();
        public static float CurrentTime => NativeAPI.GetCurrentTime();
        public static int TickCount => NativeAPI.GetTickCount();
        public static float GameFrameTime => NativeAPI.GetGameFrameTime();
        public static double EngineTime => NativeAPI.GetEngineTime();
        public static void PrecacheModel(string name) => NativeAPI.PrecacheModel(name);

        /// <summary>
        /// Queue a task to be executed on the next game frame.
        /// <remarks>Does not execute if the server is hibernating.</remarks>
        /// </summary>
        public static void NextFrame(Action task)
        {
            var functionReference = FunctionReference.Create(task);
            functionReference.Lifetime = FunctionLifetime.SingleUse;
            NativeAPI.QueueTaskForNextFrame(functionReference);
        }
        
        /// <summary>
        /// Queue a task to be executed on the next pre world update.
        /// <remarks>Executes if the server is hibernating.</remarks>
        /// </summary>
        /// <param name="task"></param>
        public static void NextWorldUpdate(Action task)
        {
            var functionReference = FunctionReference.Create(task);
            functionReference.Lifetime = FunctionLifetime.SingleUse;
            NativeAPI.QueueTaskForNextWorldUpdate(functionReference);
        }

        public static void PrintToChatAll(string message)
        {
            VirtualFunctions.ClientPrintAll(HudDestination.Chat, message, 0, 0, 0, 0);
        }

        public static string GameDirectory => NativeAPI.GetGameDirectory();

        public static int MaxPlayers => NativeAPI.GetMaxClients();

        public static bool IsMapValid(string mapName) => NativeAPI.IsMapValid(mapName);

        public static string[] GetMapList()
        {
            var mapListPath = Path.Join(GameDirectory, "maplist.txt");
            if (!File.Exists(mapListPath)) throw new Exception("Maplist.txt could not be found!");

            var mapNames = File.ReadAllLines(mapListPath);

            return mapNames.Where(map =>
            {
                var filePath = Path.Join(GameDirectory, "maps", $"{map}.bsp");
                return File.Exists(filePath) && IsMapValid(map);
            }).ToArray();
        }

        public static void PrintToConsole(string s) => NativeAPI.PrintToServerConsole($"{s}\n\0");
    }
}