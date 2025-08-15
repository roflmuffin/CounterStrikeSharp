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
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API
{
    public class Server
    {
        /// <summary>
        /// Duration of a single game tick in seconds, based on a 64 tick server (hard coded in CS2).
        /// </summary>
        public static float TickInterval => 0.015625f;

        /// <summary>
        /// Executes a command on the server, as if it was entered from the console.
        /// </summary>
        /// <param name="command"></param>
        public static void ExecuteCommand(string command) => NativeAPI.IssueServerCommand(command);

        public static string MapName => NativeAPI.GetMapName();

        /// <summary>
        /// Returns the total time the server has been running in seconds.
        /// </summary>
        /// <remarks>Does not increment when server is hibernating</remarks>
        public static double TickedTime => NativeAPI.GetTickedTime();

        /// <summary>
        /// Returns the current map time in seconds, as an interval of the server's tick interval.
        /// e.g. 70.046875 would represent 70 seconds of map time and the 4483rd tick of the server (70.046875 / 0.015625).
        /// </summary>
        /// <remarks>Increments even when server is hibernating</remarks>
        public static float CurrentTime => NativeAPI.GetCurrentTime();

        /// <summary>
        /// Returns the current map tick count.
        /// CS2 is a 64 tick server, so the value will increment by 64 every second.
        /// </summary>
        public static int TickCount => NativeAPI.GetTickCount();

        /// <summary>
        /// Returns the total time the server has been running in seconds.
        /// </summary>
        /// <remarks>Increments even when server is hibernating</remarks>
        public static double EngineTime => NativeAPI.GetEngineTime();

        /// <summary>
        /// Returns the time spent on last server or client frame
        /// </summary>
        public static float FrameTime => NativeAPI.GetGameFrameTime();

        public static void PrecacheModel(string name) => NativeAPI.PrecacheModel(name);

        /// <summary>
        /// <inheritdoc cref="RunOnTick"/>
        /// Returns Task that completes once the synchronous task has been completed.
        /// </summary>
        public static Task RunOnTickAsync(int tick, Action task)
        {
            var functionReference = FunctionReference.Create(task, FunctionLifetime.SingleUse);
            NativeAPI.QueueTaskForFrame(tick, functionReference);
            return functionReference.CompletionTask;
        }

        /// <summary>
        /// Queue a task to be executed on the specified tick.
        /// See <see cref="TickCount"/> to retrieve the current tick.
        /// <remarks>Does not execute if the server is hibernating.</remarks>
        /// </summary>
        public static void RunOnTick(int tick, Action task)
        {
            RunOnTickAsync(tick, task);
        }

        /// <summary>
        /// <inheritdoc cref="NextFrame"/>
        /// Returns Task that completes once the synchronous task has been completed.
        /// </summary>
        public static Task NextFrameAsync(Action task)
        {
            var functionReference = FunctionReference.Create(task, FunctionLifetime.SingleUse);
            NativeAPI.QueueTaskForNextFrame(functionReference);
            return functionReference.CompletionTask;
        }

        /// <summary>
        /// Queue a task to be executed on the next game frame.
        /// <remarks>Does not execute if the server is hibernating.</remarks>
        /// </summary>
        public static void NextFrame(Action task)
        {
            NextFrameAsync(task);
        }

        /// <summary>
        /// <inheritdoc cref="NextWorldUpdate"/>
        /// Returns Task that completes once the synchronous task has been completed.
        /// </summary>
        public static Task NextWorldUpdateAsync(Action task)
        {
            var functionReference = FunctionReference.Create(task, FunctionLifetime.SingleUse);
            NativeAPI.QueueTaskForNextWorldUpdate(functionReference);
            return functionReference.CompletionTask;
        }

        /// <summary>
        /// Queue a task to be executed on the next pre world update.
        /// <remarks>Executes if the server is hibernating.</remarks>
        /// </summary>
        /// <param name="task"></param>
        public static void NextWorldUpdate(Action task)
        {
            NextWorldUpdateAsync(task);
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
