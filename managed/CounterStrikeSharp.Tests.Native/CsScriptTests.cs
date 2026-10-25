using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.CsScript;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using Xunit;

namespace NativeTestsPlugin;

/// <summary>Live engine tests; no compiled script or map addon is required.</summary>
[Trait("Category", "CsScript")]
public sealed class CsScriptTests
{
    private static CsScriptRuntime GetSharedRuntime()
    {
        Assert.Equal(NativeTestsPlugin.gameThreadId, Environment.CurrentManagedThreadId);
        Console.WriteLine("[cs_script] Getting the shared point_script context...");
        var runtime = CsScriptRuntime.Shared;
        Console.WriteLine($"[cs_script] Context ready on host {runtime.Host.EntityHandle.Raw}.");
        return runtime;
    }

    private static CCSGameRules CurrentGameRules()
    {
        var rules = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules")
            .FirstOrDefault()?.GameRules;
        Assert.NotNull(rules);
        return rules;
    }

    [Fact]
    public Task EmptyHost_ReadsCurrentGameRules() => Server.NextWorldUpdateAsync(() =>
    {
        var runtime = GetSharedRuntime();
        Assert.True(runtime.Host.IsValid);
        Assert.Equal("point_script", runtime.Host.DesignerName);
        Assert.True(runtime.TryInitialize());
        var rules = CurrentGameRules();
        for (var i = 0; i < 10; i++)
            Assert.Equal(rules.WarmupPeriod, runtime.GameRules.IsWarmupPeriod());
        Assert.Equal(rules.WarmupPeriod, rules.Script(runtime).IsWarmupPeriod());
        Assert.Same(runtime.GameRules, rules.Script());
        Assert.Equal(Server.MapName, runtime.Instance.GetMapName());
        Assert.InRange(runtime.Instance.GetGameTime(), Server.CurrentTime - 0.1d, Server.CurrentTime + 0.1d);
    });

    [Fact]
    public Task AddTeamMoney_IncreasesEligiblePlayerBalances() => Server.NextWorldUpdateAsync(() =>
    {
        const int amount = 100;
        var runtime = GetSharedRuntime();
        var rules = CurrentGameRules();
        Assert.False(rules.WarmupPeriod, "End warmup before running the team-money test (mp_warmup_end).");
        var maxMoneyConVar = ConVar.Find("mp_maxmoney");
        Assert.NotNull(maxMoneyConVar);
        var maxMoney = maxMoneyConVar.GetPrimitiveValue<int>();

        // Include retained disconnected controllers: Valve can award them money too.
        var before = Utilities.FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller")
            .Where(player => player.IsValid && player.InGameMoneyServices != null)
            .Select(player => new
            {
                Player = player,
                Team = player.Team,
                Connected = player.Connected == PlayerConnectedState.Connected,
                Money = player.InGameMoneyServices!,
                Account = player.InGameMoneyServices!.Account,
                Earned = player.InGameMoneyServices!.MoneyEarnedForNextRound,
                Eligible = player.InGameMoneyServices!.ReceivesMoneyNextRound
            }).ToArray();
        var recipient = before.FirstOrDefault(player => player.Connected && player.Eligible &&
            (player.Team == CsTeam.CounterTerrorist || player.Team == CsTeam.Terrorist) &&
            player.Account >= 0 && (long)player.Account + amount <= maxMoney);
        Assert.True(recipient != null,
            "Need a connected T or CT eligible for round money with at least $100 below mp_maxmoney. " +
            "Join a team or add a bot, then start a normal round.");
        var team = recipient!.Team;

        try
        {
            // Award and read back in the same game-thread callback so purchases
            // and normal round processing cannot occur between the snapshots.
            rules.Script(runtime).AddTeamMoney((double)team, CSTeamMoneyReason.NONE, amount);
            Assert.Equal(recipient.Account + amount, recipient.Money.Account);
            foreach (var player in before)
            {
                if (player.Team != team)
                    Assert.Equal(player.Account, player.Money.Account);
                else if (player.Connected && player.Eligible)
                    Assert.Equal((int)Math.Min((long)player.Account + amount, maxMoney), player.Money.Account);
            }
            Console.WriteLine($"[cs_script] AddTeamMoney({team}, {amount}): " +
                $"player {recipient.Player.EntityHandle.Raw} balance {recipient.Account} -> {recipient.Money.Account}.");
            Assert.Equal(rules.WarmupPeriod, runtime.GameRules.IsWarmupPeriod());
        }
        finally
        {
            foreach (var player in before)
            {
                if (!player.Player.IsValid) continue;
                player.Money.Account = player.Account;
                player.Money.MoneyEarnedForNextRound = player.Earned;
                Utilities.SetStateChanged(player.Player, "CCSPlayerController", "m_pInGameMoneyServices");
            }
        }
    });

    [Fact]
    public Task EntityName_RoundTripsThroughV8AndNativeSchema() => Server.NextWorldUpdateAsync(() =>
    {
        var runtime = GetSharedRuntime();
        var entity = Utilities.CreateEntityByName<CBaseEntity>("info_target");
        Assert.NotNull(entity);
        try
        {
            entity.DispatchSpawn();
            Assert.True(entity.IsValidScript());
            Assert.Throws<ArgumentException>(() => entity.SetEntityName("invalid\0name"));
            var name = "css_v8_probe_\u03bb_" + Guid.NewGuid().ToString("N");
            entity.SetEntityName(name);
            Assert.Equal(name, entity.Entity!.Name);
            Assert.Equal(name, entity.GetEntityName());
            Assert.Equal(CurrentGameRules().WarmupPeriod, runtime.GameRules.IsWarmupPeriod());
        }
        finally
        {
            if (entity.IsValid) entity.Remove();
        }
    });

    [Fact]
    public Task UnknownMethodHash_IsRejected_AndSessionRemainsUsable() => Server.NextWorldUpdateAsync(() =>
    {
        var runtime = GetSharedRuntime();
        using var call = runtime.BeginInvoke(0u, uint.MaxValue);
        var error = Assert.Throws<NativeException>(call.Invoke);
        Assert.Contains("Unknown cs_script method hash", error.Message);
        Assert.Equal(CurrentGameRules().WarmupPeriod, runtime.GameRules.IsWarmupPeriod());
    });

    [Fact]
    public Task InvalidTransport_IsRejectedBeforeReadingPayload() => Server.NextWorldUpdateAsync(() =>
    {
        var context = new ScriptContext();
        context.Reset();
        context.SetIdentifier(CsScriptSchema.Native);
        context.Invoke();
        var shortRequest = Assert.Throws<NativeException>(context.CheckErrors);
        Assert.Contains("argument count / transport version", shortRequest.Message);

        context.Reset();
        context.PushPrimitive(CsScriptSchema.Word0 ^ 1UL);
        context.PushPrimitive(CsScriptSchema.Word1);
        context.PushPrimitive(CsScriptSchema.Word2);
        context.PushPrimitive(CsScriptSchema.Word3);
        context.PushPrimitive((uint)CsScriptOperation.Current);
        context.PushPrimitive(0UL);
        context.PushPrimitive(0u);
        context.PushPrimitive(uint.MaxValue);
        context.Invoke();
        var mismatch = Assert.Throws<NativeException>(context.CheckErrors);
        Assert.Contains("schema mismatch", mismatch.Message);
    });

    [Fact]
    public Task SharedAccess_ReusesOneHost() => Server.NextWorldUpdateAsync(() =>
    {
        var before = Utilities.FindAllEntitiesByDesignerName<CCSPointScriptEntity>("point_script")
            .Select(entity => entity.EntityHandle.Raw).ToArray();
        var first = GetSharedRuntime();
        Assert.Contains(first.Host.EntityHandle.Raw, before); // Already created by the core.
        for (var i = 0; i < 10; i++) Assert.Same(first, CsScriptRuntime.Shared);
        Assert.Equal(before, Utilities.FindAllEntitiesByDesignerName<CCSPointScriptEntity>("point_script")
            .Select(entity => entity.EntityHandle.Raw).ToArray());
    });

    private sealed class ScriptConsumer : BasePlugin
    {
        public override string ModuleName => "Shared script test consumer";
        public override string ModuleVersion => "1";
        public CsScriptRuntime Runtime => CsScriptRuntime.Shared;
    }

    [Fact]
    public Task PluginDisposal_LeavesSharedContextUsable() => Server.NextWorldUpdateAsync(() =>
    {
        using var first = new ScriptConsumer();
        using var second = new ScriptConsumer();
        var runtime = first.Runtime;
        Assert.Same(runtime, second.Runtime);
        first.Dispose();
        Assert.Same(runtime, second.Runtime);
        second.Dispose();
        Assert.Same(runtime, CsScriptRuntime.Shared);
        Assert.True(runtime.Host.IsValid);
        Assert.Equal(CurrentGameRules().WarmupPeriod, runtime.GameRules.IsWarmupPeriod());
    });

    [Fact]
    public async Task DeletedHost_IsRecreated_AndOldSessionIsRejected()
    {
        var old = await Server.NextWorldUpdateAsync(GetSharedRuntime);
        var handle = await Server.NextWorldUpdateAsync(() => old.Host.EntityHandle.Raw);
        await Server.NextWorldUpdateAsync(() => old.Host.Remove());
        await WaitUntilAsync(() =>
        {
            try { return CsScriptRuntime.Shared.Host.EntityHandle.Raw != handle; }
            catch (NativeException) { return false; } // Waiting for the core's deferred frame initialization.
        }, "automatic replacement of the shared script host");
        await Server.NextWorldUpdateAsync(() =>
        {
            var replacement = CsScriptRuntime.Shared;
            Assert.NotSame(old, replacement);
            Assert.False(old.Host.IsValid);
            var error = Assert.Throws<NativeException>(() => old.GameRules.IsWarmupPeriod());
            Assert.Contains("reacquire CsScriptRuntime.Shared", error.Message);
            Assert.Equal(CurrentGameRules().WarmupPeriod, replacement.GameRules.IsWarmupPeriod());
        });
    }

    [Fact]
    public async Task DeletedEntity_RejectsStaleHandle_AndSessionRemainsUsable()
    {
        var runtime = await Server.NextWorldUpdateAsync(GetSharedRuntime);
        CBaseEntity? entity = null;
        try
        {
            var wrapper = await Server.NextWorldUpdateAsync(() =>
            {
                entity = Utilities.CreateEntityByName<CBaseEntity>("info_target");
                Assert.NotNull(entity);
                entity.DispatchSpawn();
                entity.Remove();
                return entity;
            });
            await WaitUntilAsync(() => !entity!.IsValid, "test entity removal");
            await Server.NextWorldUpdateAsync(() =>
            {
                var error = Assert.Throws<NativeException>(() => wrapper.GetEntityName());
                Assert.Contains("stale entity serial", error.Message);
                Assert.Equal(CurrentGameRules().WarmupPeriod, runtime.GameRules.IsWarmupPeriod());
            });
        }
        finally
        {
            await Server.NextWorldUpdateAsync(() =>
            {
                if (entity is { IsValid: true }) entity.Remove();
            });
        }
    }

    [Fact]
    public async Task WorkerThread_IsRejected_AndSessionRemainsUsable()
    {
        var runtime = await Server.NextWorldUpdateAsync(GetSharedRuntime);
        var host = await Server.NextWorldUpdateAsync(() => runtime.Host);
        await Task.Run(() =>
        {
            Assert.NotEqual(NativeTestsPlugin.gameThreadId, Environment.CurrentManagedThreadId);
            var error = Assert.Throws<InvalidOperationException>(() => runtime.GameRules.IsWarmupPeriod());
            Assert.Contains("game thread", error.Message);
            // Shared reaches the native dispatcher, whose thread guard runs
            // before the cs_script handler and uses its own error message.
            var sharedError = Assert.Throws<NativeException>(() => { _ = CsScriptRuntime.Shared; });
            Assert.Equal("Invoked on a non-main thread", sharedError.Message);
            // Entity methods must reject the worker before resolving its handle.
            var entityError = Assert.Throws<NativeException>(() => host.GetEntityName());
            Assert.Equal("Invoked on a non-main thread", entityError.Message);
        });
        await Server.NextWorldUpdateAsync(() =>
            Assert.Equal(CurrentGameRules().WarmupPeriod, runtime.GameRules.IsWarmupPeriod()));
    }
}
