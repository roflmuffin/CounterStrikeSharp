using System;
using System.Text;
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

    public bool OnMessage(IMessageSinkMessage message)
    {
        lock (_lock)
        {
            switch (message)
            {
                // A test has passed
                case ITestPassed passed:
                    Interlocked.Increment(ref _passed);
                    AnsiConsole.MarkupLineInterpolated($"[underline green][[PASS]][/] [green]{passed.Test.DisplayName}[/]");
                    break;

                // A test has failed
                case ITestFailed failed:
                    Interlocked.Increment(ref _failed);
                    AnsiConsole.MarkupLineInterpolated($"[underline red][[FAIL]][/] [red]{failed.Test.DisplayName}[/]");
                    AnsiConsole.WriteLine($"\tReason: {failed.ExceptionTypes[0]} - {failed.Messages[0]}");
                    AnsiConsole.WriteLine(IndentStackTrace(failed.StackTraces[0] ?? "No stack trace available."));
                    break;

                // A test was skipped (e.g., using [Fact(Skip = "...")])
                case ITestSkipped skipped:
                    Interlocked.Increment(ref _skipped);
                    AnsiConsole.MarkupLineInterpolated($"[underline yellow][[SKIP]][/] [yellow]{skipped.Test.DisplayName}[/]");
                    AnsiConsole.MarkupLineInterpolated($"[yellow]\tReason: {skipped.Reason}[/]");
                    break;

                // This message indicates the entire test run for the assembly is complete.
                case ITestAssemblyFinished:
                    // We signal the main thread that it can stop waiting now.
                    Finished.SetResult(true);
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
