using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;
using Xunit;
using Xunit.Abstractions;

namespace NativeTestsPlugin;

public class ConsoleTestReporterSink : LongLivedMarshalByRefObject, IMessageSink, IDisposable
{
    public TaskCompletionSource<bool> Finished { get; } = new();

    private int _passed = 0;
    private int _failed = 0;
    private int _skipped = 0;
    private readonly object _lock = new();
    private readonly List<object> _tests = new();
    private readonly List<string> _errors = new();
    private int _expectedTotal;

    public void WriteReport(string path, string runId, Exception? error = null)
    {
        lock (_lock)
        {
            if (error != null) _errors.Add(error.ToString());
            if (_expectedTotal != _passed + _failed + _skipped)
                _errors.Add("The assembly test count did not match the reported results.");
            var report = new
            {
                runId,
                total = _passed + _failed + _skipped,
                passed = _passed,
                failed = _failed,
                skipped = _skipped,
                errors = _errors,
                tests = _tests
            };
            // Publish only complete reports; the remote runner polls for this file.
            File.WriteAllText(path + ".tmp", JsonSerializer.Serialize(report));
            File.Move(path + ".tmp", path, overwrite: true);
        }
    }

    public bool OnMessage(IMessageSinkMessage message)
    {
        lock (_lock)
        {
            switch (message)
            {
                // A test has passed
                case ITestPassed passed:
                    Interlocked.Increment(ref _passed);
                    _tests.Add(new { name = passed.Test.DisplayName, outcome = "passed", durationSeconds = passed.ExecutionTime });
                    AnsiConsole.MarkupLineInterpolated($"[underline green][[PASS]][/] [green]{passed.Test.DisplayName}[/]");
                    break;

                // A test has failed
                case ITestFailed failed:
                    Interlocked.Increment(ref _failed);
                    _tests.Add(new { name = failed.Test.DisplayName, outcome = "failed", durationSeconds = failed.ExecutionTime,
                        messages = failed.Messages, stackTraces = failed.StackTraces });
                    AnsiConsole.MarkupLineInterpolated($"[underline red][[FAIL]][/] [red]{failed.Test.DisplayName}[/]");
                    AnsiConsole.WriteLine($"\tReason: {failed.ExceptionTypes[0]} - {failed.Messages[0]}");
                    AnsiConsole.WriteLine(IndentStackTrace(failed.StackTraces[0] ?? "No stack trace available."));
                    break;

                // A test was skipped (e.g., using [Fact(Skip = "...")])
                case ITestSkipped skipped:
                    Interlocked.Increment(ref _skipped);
                    _tests.Add(new { name = skipped.Test.DisplayName, outcome = "skipped", reason = skipped.Reason });
                    AnsiConsole.MarkupLineInterpolated($"[underline yellow][[SKIP]][/] [yellow]{skipped.Test.DisplayName}[/]");
                    AnsiConsole.MarkupLineInterpolated($"[yellow]\tReason: {skipped.Reason}[/]");
                    break;

                // This message indicates the entire test run for the assembly is complete.
                case ITestAssemblyFinished finished:
                    _expectedTotal = finished.TestsRun;
                    // We signal the main thread that it can stop waiting now.
                    Finished.TrySetResult(true);
                    break;

                // Includes fatal runner errors and collection/class/fixture cleanup failures.
                case IFailureInformation failure:
                    _errors.Add(string.Join(Environment.NewLine, failure.Messages));
                    break;
            }
        }
        return true;
    }

    public string GetSummary()
    {
        return $"Summary: {_passed} Passed, {_failed} Failed, {_skipped} Skipped.";
    }

    private static string IndentStackTrace(string stackTrace)
    {
        var builder = new StringBuilder();
        var lines = stackTrace.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
        foreach (var line in lines)
        {
            builder.AppendLine($"       {line}");
        }
        return builder.ToString();
    }

    public void Dispose()
    {
        Finished.TrySetResult(true);
        GC.SuppressFinalize(this);
    }
}
