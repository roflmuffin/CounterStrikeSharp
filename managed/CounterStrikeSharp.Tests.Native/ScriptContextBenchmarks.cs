using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using Xunit;

namespace NativeTestsPlugin;

public class BenchmarkResult
{
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public long TotalCalls { get; set; }
    public double TotalMs { get; set; }
    public double NsPerCall { get; set; }
    public double CallsPerSecond { get; set; }
}

public class BenchmarkReport
{
    public string Timestamp { get; set; } = "";
    public string MapName { get; set; } = "";
    public int Iterations { get; set; }
    public int WarmupIterations { get; set; }
    public List<BenchmarkResult> Results { get; set; } = new();
}

public class ScriptContextBenchmarks
{
    private const int Iterations = 1_000_000;
    private const int WarmupIterations = 100_000;

    private static readonly List<BenchmarkResult> _results = new();
    private static readonly object _lock = new();

    private static CWorld? GetWorld() =>
        Utilities.FindAllEntitiesByDesignerName<CWorld>("worldent").FirstOrDefault();

    // ── Primitive returns, no args ──────────────────────────────────────

    [Fact]
    public void Benchmark_GetTickCount()
    {
        Run("GetTickCount (int, no args)", "Primitive Return", () => NativeAPI.GetTickCount());
    }

    [Fact]
    public void Benchmark_GetTickInterval()
    {
        Run("GetTickInterval (float, no args)", "Primitive Return", () => NativeAPI.GetTickInterval());
    }

    [Fact]
    public void Benchmark_GetEngineTime()
    {
        Run("GetEngineTime (double, no args)", "Primitive Return", () => NativeAPI.GetEngineTime());
    }

    [Fact]
    public void Benchmark_GetMaxClients()
    {
        Run("GetMaxClients (int, no args)", "Primitive Return", () => NativeAPI.GetMaxClients());
    }

    [Fact]
    public void Benchmark_IsServerPaused()
    {
        Run("IsServerPaused (bool, no args)", "Primitive Return", () => NativeAPI.IsServerPaused());
    }

    // ── String returns ──────────────────────────────────────────────────

    [Fact]
    public void Benchmark_GetMapName()
    {
        Run("GetMapName (string, no args)", "String Return", () => NativeAPI.GetMapName());
    }

    [Fact]
    public void Benchmark_GetConvarName()
    {
        var idx = NativeAPI.GetConvarAccessIndexByName("sv_cheats");
        Run("GetConvarName (ushort → string)", "String Return", () => NativeAPI.GetConvarName(idx));
    }

    // ── Primitive arg → primitive return ────────────────────────────────

    [Fact]
    public void Benchmark_GetConvarFlags()
    {
        var idx = NativeAPI.GetConvarAccessIndexByName("sv_cheats");
        Run("GetConvarFlags (ushort → ulong)", "Primitive Args", () => NativeAPI.GetConvarFlags(idx));
    }

    [Fact]
    public void Benchmark_GetConvarType()
    {
        var idx = NativeAPI.GetConvarAccessIndexByName("sv_cheats");
        Run("GetConvarType (ushort → short)", "Primitive Args", () => NativeAPI.GetConvarType(idx));
    }

    // ── String push cost ────────────────────────────────────────────────

    [Fact]
    public void Benchmark_FindConvar()
    {
        Run("FindConvar (string → pointer)", "String Push", () => NativeAPI.FindConvar("sv_cheats"));
    }

    [Fact]
    public void Benchmark_PushString_Short()
    {
        Run("PushString 9 bytes", "String Push", () => NativeAPI.GetConvarAccessIndexByName("sv_cheats"));
    }

    [Fact]
    public void Benchmark_PushString_Medium()
    {
        var str = "sv_cheats" + new string('x', 191); // 200 bytes
        Run("PushString 200 bytes", "String Push", () => NativeAPI.GetConvarAccessIndexByName(str));
    }

    [Fact]
    public void Benchmark_PushString_Long()
    {
        var str = "sv_cheats" + new string('x', 1991); // 2000 bytes
        Run("PushString 2000 bytes", "String Push", () => NativeAPI.GetConvarAccessIndexByName(str));
    }

    [Fact]
    public void Benchmark_PushString_Overflow()
    {
        var str = new string('x', 9000); // exceeds 8192 arena
        Run("PushString 9000 bytes (overflow)", "String Push", () => NativeAPI.GetConvarAccessIndexByName(str));
    }

    // ── Mixed workload ──────────────────────────────────────────────────

    [Fact]
    public void Benchmark_Mixed()
    {
        var convarIdx = NativeAPI.GetConvarAccessIndexByName("sv_cheats");

        Run("Mixed (4 natives/iter)", "Mixed", () =>
        {
            NativeAPI.GetTickCount();
            NativeAPI.GetMapName();
            NativeAPI.FindConvar("sv_cheats");
            NativeAPI.GetConvarFlags(convarIdx);
        }, callsPerIteration: 4);
    }

    // ── Schema ──────────────────────────────────────────────────────────

    [Fact]
    public void Benchmark_SchemaOffset_Cached()
    {
        Schema.GetSchemaOffset("CBaseEntity", "m_iHealth"); // prime cache
        Run("SchemaOffset cached", "Schema", () => Schema.GetSchemaOffset("CBaseEntity", "m_iHealth"));
    }

    [Fact]
    public void Benchmark_SchemaOffset_MultipleKeys()
    {
        Schema.GetSchemaOffset("CBaseEntity", "m_iHealth");
        Schema.GetSchemaOffset("CBaseEntity", "m_iTeamNum");
        Schema.GetSchemaOffset("CBaseEntity", "m_fFlags");
        Schema.GetSchemaOffset("CBasePlayerPawn", "m_vecAbsVelocity");

        Run("SchemaOffset 4 keys", "Schema", () =>
        {
            Schema.GetSchemaOffset("CBaseEntity", "m_iHealth");
            Schema.GetSchemaOffset("CBaseEntity", "m_iTeamNum");
            Schema.GetSchemaOffset("CBaseEntity", "m_fFlags");
            Schema.GetSchemaOffset("CBasePlayerPawn", "m_vecAbsVelocity");
        }, callsPerIteration: 4);
    }

    [Fact]
    public void Benchmark_GetDeclaredClass()
    {
        var world = GetWorld();
        if (world == null) { Skip("no world entity"); return; }

        Run("GetDeclaredClass (CBodyComponent)", "Schema", () => _ = world.CBodyComponent);
    }

    [Fact]
    public void Benchmark_GetSchemaValue_Int()
    {
        var world = GetWorld();
        if (world == null) { Skip("no world entity"); return; }

        Run("GetSchemaValue<int> (Health)", "Schema", () => _ = world.Health);
    }

    // ── Virtual function invocation ────────────────────────────────────

    [Fact]
    public void Benchmark_VFunc_PreCreatedDelegate()
    {
        var world = GetWorld();
        if (world == null) { Skip("no world entity"); return; }

        // Pre-create the delegate so we only measure the invoke path, not
        // GameData.GetOffset or VirtualFunction.Create overhead.
        var offset = GameData.GetOffset("CBaseEntity_IsPlayerPawn");
        var isPlayerPawn = VirtualFunction.Create<IntPtr, bool>(world.Handle, offset);
        var handle = world.Handle;

        Run("VFunc IsPlayerPawn (pre-created delegate)", "Virtual Function", () => isPlayerPawn(handle));
    }

    [Fact]
    public void Benchmark_VFunc_HighLevelApi()
    {
        var world = GetWorld();
        if (world == null) { Skip("no world entity"); return; }

        // Full path: Guard.IsValidEntity + GameData.GetOffset +
        // VirtualFunction.Create + invoke on every call.
        Run("VFunc IsPlayerPawn (high-level API)", "Virtual Function", () => world.IsPlayerPawn());
    }

    // ── Entity lifecycle ────────────────────────────────────────────────

    [Fact]
    public async Task Benchmark_EntityCreateAndDelete()
    {
        // Create info_target entities in batches of 4096, timing only the
        // creation. After each batch, remove them and wait a frame so the
        // engine frees the slots before the next round.
        const int batchSize = 4096;
        int batches = Iterations / batchSize;

        // Warmup batch
        var buf = new CBaseEntity[batchSize];
        for (int i = 0; i < batchSize; i++)
        {
            buf[i] = Utilities.CreateEntityByName<CBaseEntity>("info_target")!;
            buf[i].DispatchSpawn();
        }
        for (int i = 0; i < batchSize; i++)
            buf[i].Remove();
        await TestUtils.WaitOneFrame();

        var sw = new Stopwatch();

        for (int b = 0; b < batches; b++)
        {
            sw.Start();
            for (int i = 0; i < batchSize; i++)
            {
                buf[i] = Utilities.CreateEntityByName<CBaseEntity>("info_target")!;
                buf[i].DispatchSpawn();
            }
            sw.Stop();

            for (int i = 0; i < batchSize; i++)
                buf[i].Remove();
            await TestUtils.WaitOneFrame();
        }

        Record("Entity create+spawn info_target", "Entity Lifecycle", sw, (long)batches * batchSize);
    }

    // ── Export ───────────────────────────────────────────────────────────

    public static void ExportResults()
    {
        List<BenchmarkResult> snapshot;
        lock (_lock)
        {
            snapshot = _results.OrderBy(r => r.Category).ThenBy(r => r.Name).ToList();
            _results.Clear();
        }

        if (snapshot.Count == 0)
        {
            Console.WriteLine("[BENCH] No results to export.");
            return;
        }

        var report = new BenchmarkReport
        {
            Timestamp = DateTime.UtcNow.ToString("o"),
            MapName = TryGetMapName(),
            Iterations = Iterations,
            WarmupIterations = WarmupIterations,
            Results = snapshot
        };

        var dir = TryGetPluginDirectory() ?? Path.GetTempPath();
        Directory.CreateDirectory(dir);

        var jsonPath = Path.Combine(dir, "benchmark-results.json");
        File.WriteAllText(jsonPath, JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true }));

        var mdPath = Path.Combine(dir, "benchmark-results.md");
        File.WriteAllText(mdPath, FormatMarkdown(report));

        Console.WriteLine("=== BENCHMARK EXPORT ===");
        Console.WriteLine($"  JSON: {jsonPath}");
        Console.WriteLine($"  MD:   {mdPath}");
        Console.WriteLine($"  {snapshot.Count} benchmark(s) exported.");
        Console.WriteLine("========================");
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    /// <summary>
    /// Warmup + measure a synchronous action. If the action body contains
    /// multiple logical calls, pass <paramref name="callsPerIteration"/> so
    /// the per-call stats are correct.
    /// </summary>
    private static void Run(string name, string category, Action body, int callsPerIteration = 1)
    {
        for (int i = 0; i < WarmupIterations; i++)
            body();

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            body();
        sw.Stop();

        Record(name, category, sw, (long)Iterations * callsPerIteration);
    }

    private static void Record(string name, string category, Stopwatch sw, long totalCalls)
    {
        var elapsed = sw.Elapsed;
        var nsPerCall = elapsed.TotalNanoseconds / totalCalls;
        var callsPerSec = totalCalls / elapsed.TotalSeconds;

        lock (_lock)
        {
            _results.Add(new BenchmarkResult
            {
                Name = name,
                Category = category,
                TotalCalls = totalCalls,
                TotalMs = Math.Round(elapsed.TotalMilliseconds, 2),
                NsPerCall = Math.Round(nsPerCall, 0),
                CallsPerSecond = Math.Round(callsPerSec, 0)
            });
        }

        Console.WriteLine($"[BENCH] {name}");
        Console.WriteLine($"        {totalCalls:N0} calls in {elapsed.TotalMilliseconds:F2} ms" +
                          $" | {nsPerCall:F0} ns/call | {callsPerSec:N0} calls/sec");
    }

    private static void Skip(string reason) =>
        Console.WriteLine($"[BENCH] SKIP: {reason}");

    private static string TryGetMapName()
    {
        try { return NativeAPI.GetMapName(); }
        catch { return "unknown"; }
    }

    private static string? TryGetPluginDirectory()
    {
        try
        {
            var path = NativeTestsPlugin.Instance?.ModulePath;
            return path != null ? Path.GetDirectoryName(path) : null;
        }
        catch { return null; }
    }

    private static string FormatMarkdown(BenchmarkReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Benchmark Results");
        sb.AppendLine();
        sb.AppendLine($"- **Date:** {report.Timestamp}");
        sb.AppendLine($"- **Map:** {report.MapName}");
        sb.AppendLine($"- **Iterations:** {report.Iterations:N0}");
        sb.AppendLine($"- **Warmup:** {report.WarmupIterations:N0}");
        sb.AppendLine();

        foreach (var group in report.Results.GroupBy(r => r.Category).OrderBy(g => g.Key))
        {
            sb.AppendLine($"## {group.Key}");
            sb.AppendLine();
            sb.AppendLine("| Benchmark | ns/call | calls/sec | total ms |");
            sb.AppendLine("|:----------|--------:|----------:|---------:|");

            foreach (var r in group)
                sb.AppendLine($"| {r.Name} | {r.NsPerCall:N0} | {r.CallsPerSecond:N0} | {r.TotalMs:F2} |");

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
