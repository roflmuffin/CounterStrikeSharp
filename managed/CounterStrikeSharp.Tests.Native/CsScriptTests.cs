using System.Linq;
using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.CsScript;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using Xunit;

namespace NativeTestsPlugin;

/// <summary>Requires a running CS2 server.</summary>
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

    private static void AssertComponents(float x, float y, float z, Vector3 value)
    {
        Assert.InRange(value.X, x - 0.01f, x + 0.01f);
        Assert.InRange(value.Y, y - 0.01f, y + 0.01f);
        Assert.InRange(value.Z, z - 0.01f, z + 0.01f);
    }

    [Fact]
    public Task MoveAndTeleport_RoundTripVectorsAndPreserveOmittedFields() => Server.NextWorldUpdateAsync(() =>
    {
        GetSharedRuntime();
        var entity = Utilities.CreateEntityByName<CBaseEntity>("info_target");
        Assert.NotNull(entity);
        try
        {
            entity.DispatchSpawn();
            entity.TeleportScript(new CsScriptTransform
            {
                Position = new Vector3(111.25f, -222.5f, 333.75f),
                Angles = new Vector3(10, 20, 30),
                Velocity = new Vector3(12, -6, 4),
                AngularVelocity = new Vector3(1, 2, 3)
            });
            AssertComponents(111.25f, -222.5f, 333.75f, entity.GetAbsOrigin());
            AssertComponents(111.25f, -222.5f, 333.75f, (Vector3)entity.AbsOrigin!);
            AssertComponents(10, 20, 30, entity.GetAbsAngles());
            AssertComponents(10, 20, 30, (Vector3)entity.AbsRotation!);
            AssertComponents(12, -6, 4, entity.GetAbsVelocity());
            AssertComponents(12, -6, 4, (Vector3)entity.AbsVelocity);

            var previousPosition = entity.GetAbsOrigin();
            entity.Move(new CsScriptTransform { Position = new Vector3(-10, 20, 40) });
            AssertComponents(-10, 20, 40, entity.GetAbsOrigin());
            AssertComponents(-10, 20, 40, entity.GetLocalOrigin());
            AssertComponents(111.25f, -222.5f, 333.75f, previousPosition);
            AssertComponents(10, 20, 30, entity.GetAbsAngles());
            AssertComponents(12, -6, 4, entity.GetAbsVelocity());

            entity.Move(new CsScriptTransform { Velocity = new Vector3(), AngularVelocity = new Vector3() });
            AssertComponents(0, 0, 0, entity.GetAbsVelocity());
            AssertComponents(0, 0, 0, entity.GetLocalAngularVelocity());
            AssertComponents(-10, 20, 40, entity.GetAbsOrigin());
            AssertComponents(10, 20, 30, entity.GetLocalAngles());

#pragma warning disable CS0612
            entity.TeleportScript(null, new Vector3(5, 15, 25), null);
#pragma warning restore CS0612
            AssertComponents(-10, 20, 40, entity.GetAbsOrigin());
            AssertComponents(5, 15, 25, entity.GetAbsAngles());
            AssertComponents(0, 0, 0, entity.GetAbsVelocity());
            Assert.Throws<ArgumentNullException>(() => entity.Move(null!));
        }
        finally
        {
            if (entity.IsValid) entity.Remove();
        }
    });

    [Fact]
    public Task TruncatedTransform_IsRejectedBeforeReadingComponents() => Server.NextWorldUpdateAsync(() =>
    {
        var runtime = GetSharedRuntime();
        using var call = runtime.BeginInvoke(0xb952d4a3u, runtime.Host.EntityHandle.Raw);
        call.Push(1d);
        var error = Assert.Throws<NativeException>(call.Invoke);
        Assert.Contains("Incorrect argument count", error.Message);
        Assert.Equal(CurrentGameRules().WarmupPeriod, runtime.GameRules.IsWarmupPeriod());
    });

    [Fact]
    public Task EntityOwner_RoundTripsEntityArgumentsAndResults() => Server.NextWorldUpdateAsync(() =>
    {
        var runtime = GetSharedRuntime();
        CBaseEntity? entity = null;
        CBaseEntity? owner = null;
        try
        {
            entity = Utilities.CreateEntityByName<CBaseEntity>("info_target");
            owner = Utilities.CreateEntityByName<CBaseEntity>("info_target");
            Assert.NotNull(entity);
            Assert.NotNull(owner);
            entity.DispatchSpawn();
            owner.DispatchSpawn();
            var name = "css_v8_owner_" + Guid.NewGuid().ToString("N");
            owner.SetEntityName(name);

            Assert.Null(entity.GetOwner());
            entity.SetOwner(owner);
            var returned = entity.GetOwner();
            Assert.NotNull(returned);
            Assert.Equal(owner.EntityHandle.Raw, returned.EntityHandle.Raw);
            Assert.Equal(entity.OwnerEntity.Raw, returned.EntityHandle.Raw);
            Assert.Equal(name, returned.GetEntityName());

            Assert.Equal(owner.EntityHandle.Raw, runtime.Instance.FindEntityByName(name)?.EntityHandle.Raw);
            Assert.Null(runtime.Instance.FindEntityByName(name + "_missing"));

            entity.SetOwner(null);
            Assert.False(entity.OwnerEntity.IsValid);
            Assert.Null(entity.GetOwner());
            Console.WriteLine($"[cs_script] Entity owner {owner.EntityHandle.Raw} round-tripped and cleared.");
        }
        finally
        {
            if (entity is { IsValid: true }) entity.Remove();
            if (owner is { IsValid: true }) owner.Remove();
        }
    });

    [Fact]
    public Task UnknownMethodHash_IsRejected_AndSharedRuntimeRemainsUsable() => Server.NextWorldUpdateAsync(() =>
    {
        var runtime = GetSharedRuntime();
        using var call = runtime.BeginInvoke(0u, uint.MaxValue);
        var error = Assert.Throws<NativeException>(call.Invoke);
        Assert.Contains("Unknown cs_script method hash", error.Message);
        Assert.Equal(CurrentGameRules().WarmupPeriod, runtime.GameRules.IsWarmupPeriod());
    });

    [Fact]
    public Task IncompleteCallHeader_IsRejectedBeforeReadingPayload() => Server.NextWorldUpdateAsync(() =>
    {
        var context = new ScriptContext();
        context.Reset();
        var shortRequest = Assert.Throws<NativeException>(() => NativeAPI.CsScriptInvoke(context));
        Assert.Contains("Invalid cs_script argument count", shortRequest.Message);

        context.Reset();
        context.PushPrimitive(0u);
        var missingReceiver = Assert.Throws<NativeException>(() => NativeAPI.CsScriptInvoke(context));
        Assert.Contains("Invalid cs_script argument count", missingReceiver.Message);
    });

    [Fact]
    public Task SharedAccess_ReusesOneHost() => Server.NextWorldUpdateAsync(() =>
    {
        var before = Utilities.FindAllEntitiesByDesignerName<CCSPointScriptEntity>("point_script")
            .Select(entity => entity.EntityHandle.Raw).ToArray();
        var first = GetSharedRuntime();
        Assert.Equal(first.Host.EntityHandle.Raw, NativeAPI.CsScriptGetHost());
        Assert.True(NativeAPI.CsScriptIsReady());
        Assert.Contains(first.Host.EntityHandle.Raw, before);
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
    public async Task DeletedHost_IsRecreated_AndCachedFacadesUseReplacement()
    {
        var runtime = await Server.NextWorldUpdateAsync(GetSharedRuntime);
        var gameRules = runtime.GameRules;
        var instance = runtime.Instance;
        var console = runtime.Console;
        var oldHost = await Server.NextWorldUpdateAsync(() => runtime.Host);
        var handle = await Server.NextWorldUpdateAsync(() => oldHost.EntityHandle.Raw);
        await Server.NextWorldUpdateAsync(() => oldHost.Remove());
        await WaitUntilAsync(() =>
        {
            try
            {
                return runtime.Host.EntityHandle.Raw != handle;
            }
            catch (NativeException)
            {
                return false;
            }
        }, "automatic replacement of the shared script host");
        await Server.NextWorldUpdateAsync(() =>
        {
            Assert.Same(runtime, CsScriptRuntime.Shared);
            Assert.Same(gameRules, CsScriptRuntime.Shared.GameRules);
            Assert.Same(instance, CsScriptRuntime.Shared.Instance);
            Assert.Same(console, CsScriptRuntime.Shared.Console);
            Assert.False(oldHost.IsValid);
            Assert.True(runtime.Host.IsValid);
            Assert.True(runtime.TryInitialize());
            Assert.Equal(CurrentGameRules().WarmupPeriod, gameRules.IsWarmupPeriod());
            Assert.Equal(Server.MapName, instance.GetMapName());
        });
    }

    [Fact]
    public async Task DeletedEntity_RejectsStaleHandle_AndSharedRuntimeRemainsUsable()
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
                var argumentError = Assert.Throws<NativeException>(() => runtime.Host.SetOwner(wrapper));
                Assert.Contains("stale entity serial", argumentError.Message);
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
    public async Task WorkerThread_IsRejected_AndSharedRuntimeRemainsUsable()
    {
        var runtime = await Server.NextWorldUpdateAsync(GetSharedRuntime);
        var host = await Server.NextWorldUpdateAsync(() => runtime.Host);
        await Task.Run(() =>
        {
            Assert.NotEqual(NativeTestsPlugin.gameThreadId, Environment.CurrentManagedThreadId);
            var error = Assert.Throws<InvalidOperationException>(() => runtime.GameRules.IsWarmupPeriod());
            Assert.Contains("game thread", error.Message);

            var sharedError = Assert.Throws<NativeException>(() => { _ = CsScriptRuntime.Shared; });
            Assert.Equal("Invoked on a non-main thread", sharedError.Message);

            var entityError = Assert.Throws<NativeException>(() => host.GetEntityName());
            Assert.Equal("Invoked on a non-main thread", entityError.Message);
        });
        await Server.NextWorldUpdateAsync(() =>
            Assert.Equal(CurrentGameRules().WarmupPeriod, runtime.GameRules.IsWarmupPeriod()));
    }
}
