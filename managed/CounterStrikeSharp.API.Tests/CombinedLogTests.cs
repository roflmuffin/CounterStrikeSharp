using CounterStrikeSharp.API.Core.Logging;
using Serilog;
using Serilog.Core;
using Xunit;

namespace CounterStrikeSharp.API.Tests;

public sealed class CombinedLogTests : IDisposable
{
    private readonly string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
    private readonly Logger combined;

    public CombinedLogTests()
    {
        combined = CombinedLog.CreateLogger(directory);
    }

    [Fact]
    public void CoreAndPluginEventsKeepTheirFormats()
    {
        using var core = new LoggerConfiguration()
            .Enrich.WithProperty("SourceContext", "Core")
            .WriteTo.Logger(combined)
            .CreateLogger();
        using var plugin = PluginLogger("Example");

        core.Information("Core message");
        plugin.Error(new InvalidOperationException("Test exception"), "Plugin message");

        var text = ReadLog(Assert.Single(LogFiles()));
        Assert.Contains("[INFO] (cssharp:Core) Core message", text);
        Assert.Contains("[EROR] plugin:Example Plugin message", text);
        Assert.Contains("System.InvalidOperationException: Test exception", text);
    }

    [Fact]
    public void DisposingPluginLoggersLeavesTheCombinedLogOpen()
    {
        for (var i = 0; i < 20; i++)
        {
            using var plugin = PluginLogger("Example");
            plugin.Information("Reload {Index}", i);
        }

        combined.Information("Still open");

        var lines = ReadLines(Assert.Single(LogFiles()));
        Assert.Equal(21, lines.Length);
        Assert.Contains("Still open", lines[^1]);
    }

    [Fact]
    public void ConcurrentPluginsWriteEveryEventOnce()
    {
        Parallel.For(0, 8, producer =>
        {
            using var plugin = PluginLogger($"Plugin{producer}");
            for (var i = 0; i < 100; i++)
                plugin.Information("Event {Producer}:{Index}", producer, i);
        });

        var lines = LogFiles().SelectMany(ReadLines).ToArray();
        Assert.Equal(800, lines.Length);
        Assert.Equal(800, lines.Select(line => line[line.IndexOf("Event ", StringComparison.Ordinal)..]).Distinct().Count());
    }

    [Fact]
    public void CombinedLogDoesNotFilterEventsAcceptedByThePlugin()
    {
        using var plugin = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.WithProperty("PluginName", "Example")
            .WriteTo.Logger(combined)
            .CreateLogger();

        plugin.Debug("Debug message");

        Assert.Contains("[DBUG] plugin:Example Debug message", ReadLog(Assert.Single(LogFiles())));
    }

    [Fact]
    public void DisposingTheOwnerReleasesTheFile()
    {
        using var plugin = PluginLogger("Example");
        plugin.Information("Final message");

        combined.Dispose();

        using var file = File.Open(Assert.Single(LogFiles()), FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        Assert.True(file.Length > 0);
    }

    private Logger PluginLogger(string name) => new LoggerConfiguration()
        .Enrich.WithProperty("PluginName", name)
        .WriteTo.Logger(combined)
        .CreateLogger();

    private string[] LogFiles() => Directory.GetFiles(Path.Combine(directory, "logs"), "log-all*.txt");

    private static string ReadLog(string path)
    {
        using var file = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        using var reader = new StreamReader(file);
        return reader.ReadToEnd();
    }

    private static string[] ReadLines(string path) => ReadLog(path).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

    public void Dispose()
    {
        combined.Dispose();
        if (Directory.Exists(directory))
            Directory.Delete(directory, true);
    }
}
