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

using System.Threading;
using System.Threading.Tasks;

using CounterStrikeSharp.API.Core.Commands;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory;

using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Core.Memory
{
    /// <summary>
    /// Represents the <see cref="MemoryManager"/> states.
    /// </summary>
    public enum MemoryManagerState : int
    {
        /// <summary>
        /// Waiting in idle state.
        /// </summary>
        Idle,

        /// <summary>
        /// Currently releasing resources. Normally after this it will enter <see cref="Idle"/> if there is nothing else going on.
        /// </summary>
        InProgress,

        /// <summary>
        /// Something caused to enter this state. (<see cref="MemoryManager"/> has to be restarted in order to continue working.)
        /// </summary>
        Stopped,

        /// <summary>
        /// Halted, once it can continue it will enter the <see cref="Idle"/> or <see cref="InProgress"/> state.
        /// </summary>
        Halted,

        /// <summary>
        /// We have lost this dude.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Handles forced garbage collection and <see cref="IDisposableMemory"/>
    /// </summary>
    public class MemoryManager : IMemoryManager, IStartupService
    {
        /// <summary>
        /// Returns the total amount of released resources since startup.
        /// </summary>
        public ulong TotalReleased { get; private set; } = 0;

        /// <summary>
        /// Returns how much resources were released last time.
        /// </summary>
        public int LastReleased { get; private set; } = 0;

        /// <summary>
        /// Returns how much resources lives currently.
        /// </summary>
        public int CurrentResources => DisposableMemory.Instances;

        /// <summary>
        /// Last time the <see cref="MemoryManager"/> run.
        /// </summary>
        public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// <see cref="MemoryManager"/> state.
        /// </summary>
        public MemoryManagerState State { get; private set; } = MemoryManagerState.Unknown;

        public long TotalMemory => GC.GetTotalMemory(false);

        public long TotalAllocated => GC.GetTotalAllocatedBytes();

        public double TotalMemoryMB => (TotalMemory / (1024.0 * 1024.0));

        public double TotalAllocatedMB => (TotalAllocated / (1024.0 * 1024.0));

        private readonly ILogger<MemoryManager> _logger;

        private readonly ICommandManager _commandManager;

        private Thread _thread;

        public MemoryManager(ILogger<MemoryManager> logger, ICommandManager commandManager)
        {
            _logger = logger;
            _commandManager = commandManager;

            _thread = new Thread(BackgroundThread);
        }

        [RequiresPermissions("@css/generic")]
        private void OnCommand(CCSPlayerController? caller, CommandInfo info)
        {
            switch (info.GetArg(1))
            {
                case "stats":
                {
                    PrintStatistics(info);
                } break;

                case "start":
                {
                    Start();
                } break;

                case "stop":
                {
                    Stop(true);
                } break;

                case "pause":
                {
                    Stop(false);
                }  break;

                case "resume":
                {
                    Resume();
                } break;

                default:
                {
                    info.ReplyToCommand("Valid usage: css_memory [option]\n" +
                                               "  stats - Print garbage collector statistics.\n" +
                                               "  start - Starts the memory manager that handles leaking resources.\n" +
                                               "  stop - Stops the memory manager.\n" +
                                               "  pause - Stops the memory manager.\n" +
                                               "  resume - Resumes the memory manager.\n");
                } break;
            }
        }

        public void Load()
        {
            _commandManager.RegisterCommand(new("css_memory", "Counter-Strike Sharp Memory Manager options.",
                OnCommand)
            {
                ExecutableBy = CommandUsage.CLIENT_AND_SERVER,
                MinArgs = 1,
                UsageHint = "[option]\n" +
                            "  stats - Print garbage collector statistics.\n" +
                            "  start - Starts the memory manager that handles leaking resources.\n" +
                            "  stop - Stops the memory manager.\n" +
                            "  pause - Stops the memory manager.\n" +
                            "  resume - Resumes the memory manager.\n",
            });

            Start();
        }

        private void PrintStatistics(CommandInfo info)
        {
            info.ReplyToCommand("Memory Manager Statistics:\n" +
                                      $"State: {State}\n" +
                                      $"Total Released: {TotalReleased}\n" +
                                      $"Last Released: {LastReleased}\n" +
                                      $"Current Resources: {CurrentResources}\n" +
                                      $"Last Updated: {LastUpdated.ToString("yyyy.MM.dd hh:mm:ss tt")} (UTC)\n" +
                                      $"Heap Memory Usage: ~{TotalMemoryMB:F5} MB ({TotalMemory} bytes)\n" +
                                      $"Total Allocated Bytes: ~{TotalAllocatedMB:F5} MB ({TotalAllocated} bytes)");
        }

        public void Start()
        {
            if (CoreConfig.EnableMemoryManager)
            {
                if (State == MemoryManagerState.Idle || State == MemoryManagerState.InProgress)
                {
                    _logger.LogInformation("Service is already running");
                    return;
                }

                if (State == MemoryManagerState.Halted)
                {
                    _logger.LogInformation("Service should be resumed, not started");
                    return;
                }

                _logger.LogInformation("Starting service...");

                try
                {
                    _thread.Start();
                } catch (ThreadStateException)
                {
                    _thread = new Thread(BackgroundThread);
                    _thread.Start();
                } catch (Exception e)
                {
                    _logger.LogCritical("Exception occured: '{0}' ({1})", e.Message, e);
                }
            } else
            {
                _logger.LogError("Unable to start 'MemoryManager' with CoreConfig option '{0}' disabled.", "EnableMemoryManager");
            }
        }

        public void Stop(bool forceStop = false)
        {
            if (forceStop)
            {
                if (State == MemoryManagerState.Stopped)
                {
                    _logger.LogInformation("Service is already stopped");
                    return;
                }

                _logger.LogInformation("Stopping service... (might take a while)");

                Task.Run(() =>
                {
                    State = MemoryManagerState.Stopped;

                    _thread.Join();
                    _logger.LogInformation("Service has been stopped");
                });
            } else
            {
                if (State == MemoryManagerState.Halted)
                {
                    _logger.LogInformation("Service is already paused");
                    return;
                }

                State = MemoryManagerState.Halted;
                _logger.LogInformation("Service has been halted");
            }
        }

        public void Resume()
        {
            if (State != MemoryManagerState.Halted)
            {
                _logger.LogWarning("Unable to resume service (state: {0})", State);
                return;
            }

            State = MemoryManagerState.Idle;
            _logger.LogInformation("Service has been resumed");
        }

        private void BackgroundThread()
        {
            _logger.LogInformation("Service has been started");
            State = MemoryManagerState.Idle;

            while (State != MemoryManagerState.Stopped)
            {
                Thread.Sleep(CoreConfig.MemoryManagerInterval);

                if (State == MemoryManagerState.Stopped)
                    break;

                if (State == MemoryManagerState.Halted)
                    continue;

                int totalCount = CurrentResources;

                if (totalCount == 0)
                    continue;

                State = MemoryManagerState.InProgress;

                _logger.LogInformation("Running garbage collector ({0} disposable memory in total)", totalCount);
                DateTime startTime = DateTime.UtcNow;

                // some may go to gen1 or even gen2, but even those are released when this nondeterministic wonder wants so
                GC.Collect(0, GCCollectionMode.Default, true);

                // this might be obsolete with 'blocking: false'?
                GC.WaitForPendingFinalizers();

                // this part is wrong here as there is a chance that new ones are instantiated during this time
                // however, this is not used yet, not even sure it will be as it only serves statistics (TODO: fix)
                int totalCountAfter = CurrentResources;
                int difference = Math.Abs(totalCountAfter - totalCount);

                LastReleased = totalCountAfter == 0 ? totalCount : difference;
                TotalReleased += (ulong)LastReleased;
                LastUpdated = DateTime.UtcNow;

                if (LastReleased > 0)
                {
                    _logger.LogInformation("Released {0} leaking memory resources in {1}ms ({2} remains)", LastReleased, (LastUpdated - startTime).TotalMilliseconds, CurrentResources);
                } else
                {
                    Thread.Sleep(CoreConfig.MemoryManagerInterval);
                }

                if (State == MemoryManagerState.InProgress)
                {
                    State = MemoryManagerState.Idle;
                }
            }
        }
    }
}
