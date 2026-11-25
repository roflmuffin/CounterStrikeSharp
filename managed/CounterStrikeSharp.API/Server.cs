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

using System.Collections.Concurrent;
using System.Threading.Tasks;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Bindings;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API
{
    public class Server
    {
        internal static void Initialize()
        {
            NativeAPI.AddListener("OnTick", (Delegate)(() => OnTick()));
            NativeAPI.AddListener("OnServerPreWorldUpdate", (Delegate)((bool simulating) => OnWorldUpdate()));
        }

        private static readonly ConcurrentQueue<Action> _onTickTaskQueue = new();
        private static readonly ConcurrentQueue<Action> _onWorldUpdateTaskQueue = new();

        internal static void OnTick()
        {
            ExecuteTickTasks(_onTickTaskQueue);
        }

        internal static void OnWorldUpdate()
        {
            ExecuteTickTasks(_onWorldUpdateTaskQueue);
        }

        // A task is queued even while the hook that runs it is disabled; it then never runs, and the first one warns.
        private static void CheckFrameTask(string api)
        {
            if (Application.Instance is { } application)
                FrameTaskEngineFeatures.CheckRegistration(EngineFeatures.Current, api, application.Logger);
        }

        private static void ExecuteTickTasks(ConcurrentQueue<Action> taskQueue)
        {
            int count = Math.Min(taskQueue.Count, CoreConfig.MaximumFrameTasksExecutedPerTick);
            for (int i = 0; i < count; i++)
            {
                if (!taskQueue.TryDequeue(out var task))
                    break;

                try
                {
                    task();
                }
                catch (Exception e)
                {
                    Application.Instance.Logger.LogError(e, "Error invoking callback");
                }
            }
        }


        /// <summary>
        /// Duration of a single game tick in seconds, based on a 64 tick server (hard coded in CS2).
        /// </summary>
        public static float TickInterval => 0.015625f;

        /// <summary>
        /// Executes a command on the server, as if it was entered from the console.
        /// </summary>
        /// <param name="command"></param>
        /// <exception cref="Core.EngineFeatureUnavailableException">Engine Feature <c>cssharp.server/execute-command</c> is
        /// disabled</exception>
        public static void ExecuteCommand(string command)
        {
            EngineFeatures.Current.ThrowIfUnavailable(EngineFeatureNames.ServerExecuteCommand);
            NativeAPI.IssueServerCommand(command);
        }

        /// <exception cref="Core.EngineFeatureUnavailableException">Engine Feature <c>cssharp.server/global-vars</c> is disabled</exception>
        public static string MapName => GlobalVars(NativeAPI.GetMapName);

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
        /// <exception cref="Core.EngineFeatureUnavailableException">Engine Feature <c>cssharp.server/global-vars</c> is disabled</exception>
        public static float CurrentTime => GlobalVars(NativeAPI.GetCurrentTime);

        /// <summary>
        /// Returns the current map tick count.
        /// CS2 is a 64 tick server, so the value will increment by 64 every second.
        /// </summary>
        /// <exception cref="Core.EngineFeatureUnavailableException">Engine Feature <c>cssharp.server/global-vars</c> is disabled</exception>
        public static int TickCount => GlobalVars(NativeAPI.GetTickCount);

        /// <summary>
        /// Returns the total time the server has been running in seconds.
        /// </summary>
        /// <remarks>Increments even when server is hibernating</remarks>
        public static double EngineTime => NativeAPI.GetEngineTime();

        /// <summary>
        /// Returns the time spent on last server or client frame
        /// </summary>
        /// <exception cref="Core.EngineFeatureUnavailableException">Engine Feature <c>cssharp.server/global-vars</c> is disabled</exception>
        public static float FrameTime => GlobalVars(NativeAPI.GetGameFrameTime);

        // Map name, times, tick count and max players are read from the engine's global vars.
        private static T GlobalVars<T>(Func<T> read)
        {
            Core.EngineFeatures.Current.ThrowIfUnavailable(Core.EngineFeatureNames.ServerGlobalVars);
            return read();
        }

        /// <exception cref="Core.EngineFeatureUnavailableException">Engine Feature <c>cssharp.server/precache-model</c> is
        /// disabled</exception>
        public static void PrecacheModel(string name)
        {
            EngineFeatures.Current.ThrowIfUnavailable(EngineFeatureNames.ServerPrecacheModel);
            NativeAPI.PrecacheModel(name);
        }

        /// <summary>
        /// <inheritdoc cref="RunOnTick"/>
        /// Returns Task that completes once the synchronous task has been completed.
        /// </summary>
        public static Task RunOnTickAsync(int tick, Action task)
        {
            CheckFrameTask(nameof(RunOnTick));
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
            CheckFrameTask(nameof(NextFrame));
            var tcs = new TaskCompletionSource();

            _onTickTaskQueue.Enqueue(() =>
            {
                try
                {
                    task();
                    tcs.SetResult();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// <inheritdoc cref="NextFrame"/>
        /// <returns>A <see cref="Task{TResult}"/> that will be completed when the function has finished executing.</returns>
        /// </summary>
        public static Task<TResult> NextFrameAsync<TResult>(Func<TResult> task)
        {
            CheckFrameTask(nameof(NextFrame));
            var tcs = new TaskCompletionSource<TResult>();

            _onTickTaskQueue.Enqueue(() =>
            {
                try
                {
                    TResult result = task();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
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
            CheckFrameTask(nameof(NextWorldUpdate));
            var tcs = new TaskCompletionSource();

            _onWorldUpdateTaskQueue.Enqueue(() =>
            {
                try
                {
                    task();
                    tcs.SetResult();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
        }

        /// <summary>
        /// <inheritdoc cref="NextWorldUpdate"/>
        /// <returns>A <see cref="Task{TResult}"/> that will be completed when the function has finished executing.</returns>
        /// </summary>
        public static Task<TResult> NextWorldUpdateAsync<TResult>(Func<TResult> task)
        {
            CheckFrameTask(nameof(NextWorldUpdate));
            var tcs = new TaskCompletionSource<TResult>();

            _onWorldUpdateTaskQueue.Enqueue(() =>
            {
                try
                {
                    TResult result = task();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });

            return tcs.Task;
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

        /// <exception cref="Core.EngineFeatureUnavailableException">Engine Feature <c>cssharp.chat/client-print</c> is disabled on this
        /// build.</exception>
        public static void PrintToChatAll(string message)
        {
            VirtualFunctions.ClientPrintAll(HudDestination.Chat, message, 0, 0, 0, 0, 0);
        }

        public static string GameDirectory => NativeAPI.GetGameDirectory();

        private static int? _maxPlayers;

        public static int MaxPlayers
        {
            get
            {
                _maxPlayers ??= GlobalVars(NativeAPI.GetMaxClients);

                return _maxPlayers.Value;
            }
        }

        /// <exception cref="Core.EngineFeatureUnavailableException">Engine Feature <c>cssharp.server/is-map-valid</c> is
        /// disabled</exception>
        public static bool IsMapValid(string mapName)
        {
            EngineFeatures.Current.ThrowIfUnavailable(EngineFeatureNames.ServerIsMapValid);
            return NativeAPI.IsMapValid(mapName);
        }

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
