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
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Xunit;


namespace NativeTestsPlugin
{
    public class NativeTestsPlugin : BasePlugin
    {
        public override string ModuleName => "Native Tests";
        public override string ModuleVersion => "v1.0.0";

        public override string ModuleAuthor => "Roflmuffin";

        public override string ModuleDescription => "A an automated test plugin.";

        private int gameThreadId;

        public override void Load(bool hotReload)
        {
            gameThreadId = Thread.CurrentThread.ManagedThreadId;
            // Loading blocks the game thread, so we use NextFrame to run our tests asynchronously.
            Server.NextFrame(() => RunTests());
        }

        async Task RunTests()
        {
            Console.WriteLine("*****************************************************************");
            Console.WriteLine($"[{ModuleName}] Starting xUnit test run...");
            Console.WriteLine("*****************************************************************");

            try
            {
                using var reporter = new ConsoleTestReporterSink();

                var project = new XunitProject();
                using var controller = new XunitFrontController(AppDomainSupport.IfAvailable, this.ModulePath);

                var executionOptions = TestFrameworkOptions.ForExecution();
                executionOptions.SetDisableParallelization(true);
                executionOptions.SetMaxParallelThreads(1);
                executionOptions.SetSynchronousMessageReporting(true);
                SynchronizationContext.SetSynchronizationContext(new SourceSynchronizationContext(gameThreadId));

                controller.RunAll(reporter, TestFrameworkOptions.ForDiscovery(), executionOptions);

                await reporter.Finished.Task;
                Console.WriteLine("*****************************************************************");
                Console.WriteLine($"[{ModuleName}] Test run finished.");
                Console.WriteLine(reporter.GetSummary());
                Console.WriteLine("*****************************************************************");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{ModuleName}] A critical error occurred during the test run setup: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }
        }
    }

    public class SourceSynchronizationContext : SynchronizationContext
    {
        private readonly int _mainThreadId;
        public SourceSynchronizationContext(int mainThreadId)
        {
            _mainThreadId = mainThreadId;
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            Server.NextWorldUpdate(() => d(state));
        }

        public override SynchronizationContext CreateCopy()
        {
            return this;
        }
    }
}