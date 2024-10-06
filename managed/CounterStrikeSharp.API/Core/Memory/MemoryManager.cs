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
        /// Currently releasing resources. Normally after this it will enter the idle state if there is nothing else going on.
        /// </summary>
        InProgress,

        /// <summary>
        /// Something caused to enter this state. (<see cref="MemoryManager"/> has to be restarted in order to continue working.)
        /// </summary>
        Stopped,

        /// <summary>
        /// Halted, once it can continue it will enter the <see cref="Idle"/> state.
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
        public int TotalReleased { get; private set; } = 0;

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

        private readonly ILogger<MemoryManager> _logger;

        private readonly ICommandManager _commandManager;

        private readonly Thread _thread;

        public MemoryManager(ILogger<MemoryManager> logger, ICommandManager commandManager)
        {
            _logger = logger;
            _commandManager = commandManager;

            _thread = new Thread(BackgroundThread);
        }

        public void Load()
        {
            if (CoreConfig.EnableMemoryManager)
            {
                Start();
            }
        }

        public void Start()
        {
            _thread.Start();
            _logger.LogInformation("Service has been started");
        }

        public void Stop(bool forceStop = false)
        {
            if (forceStop)
            {
                Task.Run(() =>
                {
                    State = MemoryManagerState.Stopped;

                    _thread.Join();
                    _logger.LogInformation("Service has been stopped");
                });
            } else
            {
                State = MemoryManagerState.Halted;
                _logger.LogInformation("Service has been halted");
            }
        }

        private void BackgroundThread()
        {
            while (State != MemoryManagerState.Stopped)
            {
                State = MemoryManagerState.Idle;

                Thread.Sleep(CoreConfig.MemoryManagerInterval);

                if (State == MemoryManagerState.Halted)
                    continue;

                if (State == MemoryManagerState.Stopped)
                    break;

                int totalCount = CurrentResources;

                if (totalCount == 0)
                    continue;

                State = MemoryManagerState.InProgress;

                _logger.LogInformation("Releasing {0} disposable memory resources...", totalCount);

                // some may go to gen1 or even gen2, but even those are released when this nondeterministic wonder wants so
                GC.Collect(0, GCCollectionMode.Default, true);

                // this might be obsolete with 'blocking: false'?
                GC.WaitForPendingFinalizers();

                // this part is wrong here as there is a chance that new ones are instantiated during this time
                // however, this is not used yet, not even sure it will be as it only serves statistics (TODO: fix)
                int totalCountAfter = CurrentResources;
                int difference = totalCountAfter - totalCount;

                LastReleased = totalCountAfter == 0 ? totalCount : difference;
                TotalReleased += LastReleased;
                LastUpdated = DateTime.UtcNow;

                _logger.LogInformation("Released {0} disposable memory resources. ({1} remains)", LastReleased, CurrentResources);
            }
        }
    }
}
