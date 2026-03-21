using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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

    // ── Primitive returns, no args ──────────────────────────────────────

    [Fact]
    public void Benchmark_GetTickCount_PrimitiveIntReturn()
    {
        for (int i = 0; i < WarmupIterations; i++)
            NativeAPI.GetTickCount();

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            NativeAPI.GetTickCount();
        sw.Stop();

        Record("GetTickCount (int return, no args)", "Primitive Return", sw);
    }

    [Fact]
    public void Benchmark_GetTickInterval_PrimitiveFloatReturn()
    {
        for (int i = 0; i < WarmupIterations; i++)
            NativeAPI.GetTickInterval();

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            NativeAPI.GetTickInterval();
        sw.Stop();

        Record("GetTickInterval (float return, no args)", "Primitive Return", sw);
    }

    [Fact]
    public void Benchmark_GetEngineTime_PrimitiveDoubleReturn()
    {
        for (int i = 0; i < WarmupIterations; i++)
            NativeAPI.GetEngineTime();

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            NativeAPI.GetEngineTime();
        sw.Stop();

        Record("GetEngineTime (double return, no args)", "Primitive Return", sw);
    }

    [Fact]
    public void Benchmark_GetMaxClients_PrimitiveIntReturn()
    {
        for (int i = 0; i < WarmupIterations; i++)
            NativeAPI.GetMaxClients();

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            NativeAPI.GetMaxClients();
        sw.Stop();

        Record("GetMaxClients (int return, no args)", "Primitive Return", sw);
    }

    [Fact]
    public void Benchmark_IsServerPaused_PrimitiveBoolReturn()
    {
        for (int i = 0; i < WarmupIterations; i++)
            NativeAPI.IsServerPaused();

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            NativeAPI.IsServerPaused();
        sw.Stop();

        Record("IsServerPaused (bool return, no args)", "Primitive Return", sw);
    }

    // ── String returns, no args ─────────────────────────────────────────

    [Fact]
    public void Benchmark_GetMapName_StringReturn()
    {
        for (int i = 0; i < WarmupIterations; i++)
            NativeAPI.GetMapName();

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            NativeAPI.GetMapName();
        sw.Stop();

        Record("GetMapName (string return, no args)", "String Return", sw);
    }

    // ── Primitive arg → primitive return ────────────────────────────────

    [Fact]
    public void Benchmark_GetConvarFlags_UshortArgUlongReturn()
    {
        var index = NativeAPI.GetConvarAccessIndexByName("sv_cheats");

        for (int i = 0; i < WarmupIterations; i++)
            NativeAPI.GetConvarFlags(index);

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            NativeAPI.GetConvarFlags(index);
        sw.Stop();

        Record("GetConvarFlags (ushort arg, ulong return)", "Primitive Args", sw);
    }

    [Fact]
    public void Benchmark_GetConvarType_UshortArgShortReturn()
    {
        var index = NativeAPI.GetConvarAccessIndexByName("sv_cheats");

        for (int i = 0; i < WarmupIterations; i++)
            NativeAPI.GetConvarType(index);

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            NativeAPI.GetConvarType(index);
        sw.Stop();

        Record("GetConvarType (ushort arg, short return)", "Primitive Args", sw);
    }

    // ── Primitive arg → string return ───────────────────────────────────

    [Fact]
    public void Benchmark_GetConvarName_UshortArgStringReturn()
    {
        var index = NativeAPI.GetConvarAccessIndexByName("sv_cheats");

        for (int i = 0; i < WarmupIterations; i++)
            NativeAPI.GetConvarName(index);

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            NativeAPI.GetConvarName(index);
        sw.Stop();

        Record("GetConvarName (ushort arg, string return)", "String Return", sw);
    }

    // ── String arg → pointer return (measures string push cost) ─────────

    [Fact]
    public void Benchmark_FindConvar_StringArgPointerReturn()
    {
        for (int i = 0; i < WarmupIterations; i++)
            NativeAPI.FindConvar("sv_cheats");

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            NativeAPI.FindConvar("sv_cheats");
        sw.Stop();

        Record("FindConvar (string arg, pointer return)", "String Push", sw);
    }

    // ── String push scaling (short / medium / long) ─────────────────────

    [Fact]
    public void Benchmark_PushString_ShortString()
    {
        for (int i = 0; i < WarmupIterations; i++)
            NativeAPI.GetConvarAccessIndexByName("sv_cheats");

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            NativeAPI.GetConvarAccessIndexByName("sv_cheats");
        sw.Stop();

        Record("PushString short 'sv_cheats' (9 bytes)", "String Push", sw);
    }

    [Fact]
    public void Benchmark_PushString_MediumString()
    {
        var mediumStr = "sv_cheats" + new string('x', 191); // 200 bytes total

        for (int i = 0; i < WarmupIterations; i++)
            NativeAPI.GetConvarAccessIndexByName(mediumStr);

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            NativeAPI.GetConvarAccessIndexByName(mediumStr);
        sw.Stop();

        Record("PushString medium (200 bytes)", "String Push", sw);
    }

    [Fact]
    public void Benchmark_PushString_LongString()
    {
        var longStr = "sv_cheats" + new string('x', 1991); // 2000 bytes total

        for (int i = 0; i < WarmupIterations; i++)
            NativeAPI.GetConvarAccessIndexByName(longStr);

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            NativeAPI.GetConvarAccessIndexByName(longStr);
        sw.Stop();

        Record("PushString long (2000 bytes)", "String Push", sw);
    }

    [Fact]
    public void Benchmark_PushString_OverflowString()
    {
        var hugeStr = new string('x', 9000); // exceeds 8192 arena

        for (int i = 0; i < WarmupIterations; i++)
            NativeAPI.GetConvarAccessIndexByName(hugeStr);

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            NativeAPI.GetConvarAccessIndexByName(hugeStr);
        sw.Stop();

        Record("PushString overflow (9000 bytes)", "String Push", sw);
    }

    // ── Mixed workload ──────────────────────────────────────────────────

    [Fact]
    public void Benchmark_MixedSummary()
    {
        var convarIndex = NativeAPI.GetConvarAccessIndexByName("sv_cheats");

        for (int i = 0; i < WarmupIterations; i++)
        {
            NativeAPI.GetTickCount();
            NativeAPI.GetMapName();
            NativeAPI.FindConvar("sv_cheats");
            NativeAPI.GetConvarFlags(convarIndex);
        }

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            NativeAPI.GetTickCount();
            NativeAPI.GetMapName();
            NativeAPI.FindConvar("sv_cheats");
            NativeAPI.GetConvarFlags(convarIndex);
        }

        sw.Stop();

        Record("Mixed workload (4 natives per iteration)", "Mixed", sw, Iterations * 4);
    }

    // ── Schema property access ─────────────────────────────────────────

    [Fact]
    public void Benchmark_SchemaOffset_CachedLookup()
    {
        // Prime the cache
        Schema.GetSchemaOffset("CBaseEntity", "m_iHealth");

        for (int i = 0; i < WarmupIterations; i++)
            Schema.GetSchemaOffset("CBaseEntity", "m_iHealth");

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            Schema.GetSchemaOffset("CBaseEntity", "m_iHealth");
        sw.Stop();

        Record("Schema.GetSchemaOffset cached (record struct key)", "Schema", sw);
    }

    [Fact]
    public void Benchmark_SchemaOffset_MultipleDifferentKeys()
    {
        // Prime caches for several different fields
        Schema.GetSchemaOffset("CBaseEntity", "m_iHealth");
        Schema.GetSchemaOffset("CBaseEntity", "m_iTeamNum");
        Schema.GetSchemaOffset("CBaseEntity", "m_fFlags");
        Schema.GetSchemaOffset("CBasePlayerPawn", "m_vecAbsVelocity");

        for (int i = 0; i < WarmupIterations; i++)
        {
            Schema.GetSchemaOffset("CBaseEntity", "m_iHealth");
            Schema.GetSchemaOffset("CBaseEntity", "m_iTeamNum");
            Schema.GetSchemaOffset("CBaseEntity", "m_fFlags");
            Schema.GetSchemaOffset("CBasePlayerPawn", "m_vecAbsVelocity");
        }

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
        {
            Schema.GetSchemaOffset("CBaseEntity", "m_iHealth");
            Schema.GetSchemaOffset("CBaseEntity", "m_iTeamNum");
            Schema.GetSchemaOffset("CBaseEntity", "m_fFlags");
            Schema.GetSchemaOffset("CBasePlayerPawn", "m_vecAbsVelocity");
        }

        sw.Stop();

        Record("Schema.GetSchemaOffset 4 different keys", "Schema", sw, Iterations * 4);
    }

    [Fact]
    public void Benchmark_GetDeclaredClass()
    {
        var world = Utilities.FindAllEntitiesByDesignerName<CWorld>("worldent").FirstOrDefault();
        if (world == null)
        {
            Console.WriteLine("[BENCH] SKIP: GetDeclaredClass - no world entity");
            return;
        }

        for (int i = 0; i < WarmupIterations; i++)
            _ = world.CBodyComponent;

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            _ = world.CBodyComponent;
        sw.Stop();

        Record("GetDeclaredClass via CBodyComponent (FastNew)", "Schema", sw);
    }

    [Fact]
    public void Benchmark_GetSchemaValue_Int()
    {
        var world = Utilities.FindAllEntitiesByDesignerName<CWorld>("worldent").FirstOrDefault();
        if (world == null)
        {
            Console.WriteLine("[BENCH] SKIP: GetSchemaValue<int> - no world entity");
            return;
        }

        for (int i = 0; i < WarmupIterations; i++)
            _ = world.Health;

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < Iterations; i++)
            _ = world.Health;
        sw.Stop();

        Record("GetSchemaValue<int> via Health property", "Schema", sw);
    }

    // ── Export ───────────────────────────────────────────────────────────

    public static void ExportResults()
    {
        List<BenchmarkResult> snapshot;
        lock (_lock)
        {
            snapshot = new List<BenchmarkResult>(_results.OrderBy(r => r.Category).ThenBy(r => r.Name));
            _results.Clear();
        }

        if (snapshot.Count == 0)
        {
            Console.WriteLine("[BENCH] No benchmark results to export.");
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

        var outputDir = TryGetPluginDirectory() ?? Path.GetTempPath();
        Directory.CreateDirectory(outputDir);

        var jsonPath = Path.Combine(outputDir, "benchmark-results.json");
        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(jsonPath, JsonSerializer.Serialize(report, jsonOptions));

        var mdPath = Path.Combine(outputDir, "benchmark-results.md");
        File.WriteAllText(mdPath, GenerateMarkdown(report));

        Console.WriteLine("=== BENCHMARK EXPORT ===");
        Console.WriteLine($"  JSON: {jsonPath}");
        Console.WriteLine($"  MD:   {mdPath}");
        Console.WriteLine($"  {snapshot.Count} benchmark(s) exported.");
        Console.WriteLine("========================");
    }

    // ── Internal helpers ────────────────────────────────────────────────

    private static void Record(string name, string category, Stopwatch sw, long? totalCalls = null)
    {
        var calls = totalCalls ?? Iterations;
        var elapsed = sw.Elapsed;
        var nsPerCall = elapsed.TotalNanoseconds / calls;
        var callsPerSecond = calls / elapsed.TotalSeconds;

        var result = new BenchmarkResult
        {
            Name = name,
            Category = category,
            TotalCalls = calls,
            TotalMs = Math.Round(elapsed.TotalMilliseconds, 2),
            NsPerCall = Math.Round(nsPerCall, 0),
            CallsPerSecond = Math.Round(callsPerSecond, 0)
        };

        lock (_lock)
        {
            _results.Add(result);
        }

        Console.WriteLine($"[BENCH] {name}");
        Console.WriteLine(
            $"        {calls:N0} calls in {elapsed.TotalMilliseconds:F2} ms | {nsPerCall:F0} ns/call | {callsPerSecond:N0} calls/sec");
    }

    private static string TryGetMapName()
    {
        try
        {
            return NativeAPI.GetMapName();
        }
        catch
        {
            return "unknown";
        }
    }

    private static string? TryGetPluginDirectory()
    {
        try
        {
            return NativeTestsPlugin.Instance?.ModulePath != null ? Path.GetDirectoryName(NativeTestsPlugin.Instance.ModulePath) : null;
        }
        catch
        {
            return null;
        }
    }

    private static string GenerateMarkdown(BenchmarkReport report)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Benchmark Results");
        sb.AppendLine();
        sb.AppendLine($"- **Date:** {report.Timestamp}");
        sb.AppendLine($"- **Map:** {report.MapName}");
        sb.AppendLine($"- **Iterations:** {report.Iterations:N0}");
        sb.AppendLine($"- **Warmup:** {report.WarmupIterations:N0}");
        sb.AppendLine();

        var categories = report.Results
            .GroupBy(r => r.Category)
            .OrderBy(g => g.Key);

        foreach (var group in categories)
        {
            sb.AppendLine($"## {group.Key}");
            sb.AppendLine();
            sb.AppendLine("| Benchmark | ns/call | calls/sec | total ms |");
            sb.AppendLine("|:----------|--------:|----------:|---------:|");

            foreach (var r in group)
            {
                sb.AppendLine($"| {r.Name} | {r.NsPerCall:N0} | {r.CallsPerSecond:N0} | {r.TotalMs:F2} |");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
}
