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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using Xunit;
using Xunit.Abstractions;


namespace NativeTestsPlugin
{
    public class NativeTestsPlugin : BasePlugin
    {
        public override string ModuleName => "Native Tests";
        public override string ModuleVersion => "v1.0.0";

        public override string ModuleAuthor => "Roflmuffin";

        public override string ModuleDescription => "A an automated test plugin.";

        public static int gameThreadId;
        private bool _running;

        public static NativeTestsPlugin Instance { get; private set; } = null!;

        public override void Load(bool hotReload)
        {
            gameThreadId = Thread.CurrentThread.ManagedThreadId;
            Instance = this;
            // Loading blocks the game thread, so we use NextFrame to run our tests asynchronously.
            // Uncomment to run the tests on load
            // Server.NextWorldUpdate(() => RunTests());
            AddCommand("css_run_tests", "Runs the xUnit tests for the native plugin.", (player, info) => { RunTests(); });
        }

        [ConsoleCommand("css_itest")]
        public void OnCommandTest(CCSPlayerController? player, CommandInfo command)
        {
            var filter = command.GetArg(1);
            if (string.IsNullOrWhiteSpace(filter))
            {
                Server.PrintToConsole("Usage: css_itest <filter>");
                Server.PrintToConsole("Example: css_itest MyTestClass");
                Server.PrintToConsole("         css_itest MyTestMethod");
                return;
            }

            RunTests(filter);
        }

        [ConsoleCommand("css_smoke_test", "Run all non-benchmark tests and export a JSON report.")]
        public void OnCommandSmokeTest(CCSPlayerController? player, CommandInfo command)
        {
            // Only the server console/RCON may start an automated run.
            var runId = command.GetArg(1);
            if (player != null || !Regex.IsMatch(runId, @"\A[a-zA-Z0-9-]{1,80}\z")) return;
            _ = RunTests(smokeRunId: runId);
        }

        public async Task RunTests(string? filter = null, string? smokeRunId = null)
        {
            if (_running)
            {
                Console.WriteLine($"[{ModuleName}] A test run is already in progress.");
                return;
            }
            _running = true;
            using var reporter = new ConsoleTestReporterSink();
            Exception? runError = null;
            Console.WriteLine("*****************************************************************");
            if (!string.IsNullOrWhiteSpace(filter))
            {
                Console.WriteLine($"[{ModuleName}] Starting xUnit test run with filter: {filter}");
            }
            else
            {
                Console.WriteLine($"[{ModuleName}] Starting xUnit test run...");
            }

            Console.WriteLine("*****************************************************************");

            try
            {
                using var controller = new XunitFrontController(AppDomainSupport.IfAvailable, this.ModulePath);

                var executionOptions = TestFrameworkOptions.ForExecution();
                executionOptions.SetDisableParallelization(true);
                executionOptions.SetMaxParallelThreads(1);
                executionOptions.SetSynchronousMessageReporting(true);
                SynchronizationContext.SetSynchronizationContext(new SourceSynchronizationContext(gameThreadId));

                var discoveryOptions = TestFrameworkOptions.ForDiscovery();

                if (smokeRunId != null || !string.IsNullOrWhiteSpace(filter))
                {
                    // Discover all tests first
                    var discoverySink = new TestDiscoverySink();
                    controller.Find(false, discoverySink, discoveryOptions);
                    discoverySink.Finished.WaitOne();

                    // Filter test cases by class name or method name
                    var filteredTests = new List<ITestCase>();
                    foreach (var testCase in discoverySink.TestCases)
                    {
                        var testClassName = testCase.TestMethod?.TestClass?.Class?.Name ?? "";
                        var testMethodName = testCase.TestMethod?.Method?.Name ?? "";

                        var isBenchmark = testCase.Traits.Any(trait =>
                            trait.Key.Equals("Category", StringComparison.OrdinalIgnoreCase) &&
                            trait.Value.Any(value => value.Equals("Benchmark", StringComparison.OrdinalIgnoreCase))) ||
                            testClassName.Contains("Benchmark", StringComparison.OrdinalIgnoreCase) ||
                            testMethodName.Contains("Benchmark", StringComparison.OrdinalIgnoreCase);

                        if (smokeRunId != null ? !isBenchmark :
                            testClassName.Contains(filter!, StringComparison.OrdinalIgnoreCase) ||
                            testMethodName.Contains(filter!, StringComparison.OrdinalIgnoreCase))
                        {
                            filteredTests.Add(testCase);
                        }
                    }

                    if (filteredTests.Count == 0)
                    {
                        Console.WriteLine($"[{ModuleName}] No tests selected (filter: {filter ?? "non-benchmark suite"}).");
                        return;
                    }

                    Console.WriteLine($"[{ModuleName}] Selected {filteredTests.Count} test(s).");

                    // Run only the filtered tests
                    controller.RunTests(filteredTests, reporter, executionOptions);
                }
                else
                {
                    controller.RunAll(reporter, discoveryOptions, executionOptions);
                }

                await reporter.Finished.Task;
                Console.WriteLine("*****************************************************************");
                Console.WriteLine($"[{ModuleName}] Test run finished.");
                Console.WriteLine(reporter.GetSummary());
                Console.WriteLine("*****************************************************************");

                // Export benchmark results if any were collected
                if (smokeRunId == null) ScriptContextBenchmarks.ExportResults();
            }
            catch (Exception ex)
            {
                runError = ex;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{ModuleName}] A critical error occurred during the test run setup: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }
            finally
            {
                try
                {
                    if (smokeRunId != null)
                        reporter.WriteReport(Path.Combine(ModuleDirectory, "smoke-results.json"), smokeRunId, runError);
                }
                finally
                {
                    _running = false;
                }
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

        public override void Post(SendOrPostCallback d, object? state)
        {
            Server.NextWorldUpdate(() => d(state));
        }

        public override SynchronizationContext CreateCopy()
        {
            return this;
        }
    }

    public class TestDiscoverySink : LongLivedMarshalByRefObject, IMessageSink
    {
        public List<ITestCase> TestCases { get; } = new List<ITestCase>();
        public ManualResetEvent Finished { get; } = new ManualResetEvent(false);

        public bool OnMessage(IMessageSinkMessage message)
        {
            if (message is ITestCaseDiscoveryMessage discoveryMessage)
            {
                TestCases.Add(discoveryMessage.TestCase);
            }
            else if (message is IDiscoveryCompleteMessage)
            {
                Finished.Set();
            }

            return true;
        }
    }
}