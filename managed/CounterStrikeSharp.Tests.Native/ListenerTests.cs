using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using Xunit;

public class ListenerTests
{
    [Fact]
    public async Task CanRegisterAndDeregisterListeners()
    {
        int callCount = 0;
        var callback = FunctionReference.Create((int playerSlot, string name, string ipAddress) =>
        {
            Assert.NotNull(ipAddress);
            Assert.NotEmpty(name);
            Assert.Equal("127.0.0.1", ipAddress);
            callCount++;
        });

        NativeAPI.IssueServerCommand("bot_quota 0; bot_quota_mode normal");
        await WaitOneFrame();

        NativeAPI.AddListener("OnClientConnect", callback);

        // Test hooking
        NativeAPI.IssueServerCommand("bot_kick");
        NativeAPI.IssueServerCommand("bot_add");
        await WaitOneFrame();

        Assert.Equal(1, callCount);
        NativeAPI.RemoveListener("OnClientConnect", callback);

        // Test unhooking
        NativeAPI.IssueServerCommand("bot_kick");
        NativeAPI.IssueServerCommand("bot_add");
        await WaitOneFrame();
        Assert.Equal(1, callCount);

        NativeAPI.IssueServerCommand("bot_quota 1");
    }

    [Fact]
    public async Task TakeDamageListenersAreFired()
    {
        int preCallCount = 0;
        int postCallCount = 0;

        var preCallback = FunctionReference.Create((IntPtr entityPtr, IntPtr damageInfoPtr) => { preCallCount++; });

        var postCallback = FunctionReference.Create((IntPtr entityPtr, IntPtr damageInfoPtr) => { postCallCount++; });

        try
        {
            NativeAPI.AddListener("OnEntityTakeDamagePre", preCallback);
            NativeAPI.AddListener("OnEntityTakeDamagePost", postCallback);

            // Spawn a bot and deal damage to it
            NativeAPI.IssueServerCommand("bot_kick");
            NativeAPI.IssueServerCommand("bot_add");
            await WaitOneFrame();

            var player = Utilities.GetPlayers().FirstOrDefault(p => p.IsBot);
            var playerHealth = player.PlayerPawn.Value.Health;
            DealDamageFunc(player, player, 10);

            await WaitOneFrame();
            Assert.Equal(playerHealth - 10, player.PlayerPawn.Value.Health);

            Assert.Equal(1, preCallCount);
            Assert.Equal(1, postCallCount);
        }
        finally
        {
            NativeAPI.RemoveListener("OnEntityTakeDamagePre", preCallback);
            NativeAPI.RemoveListener("OnEntityTakeDamagePost", postCallback);
        }
    }

    [Fact]
    public async Task TakeDamageListenerCanBeCancelled()
    {
        int preCallCount = 0;
        int postCallCount = 0;

        Listeners.OnEntityTakeDamagePre preCallback = (entityPtr, damageInfoPtr) =>
        {
            preCallCount++;
            return HookResult.Stop;
        };

        Listeners.OnEntityTakeDamagePre secondCallback = (entityPtr, damageInfoPtr) =>
        {
            preCallCount++;
            return HookResult.Continue;
        };

        Listeners.OnEntityTakeDamagePost postCallback = (entity, damageInfo, damageResult) => { postCallCount++; };

        try
        {
            NativeAPI.AddListener("OnEntityTakeDamagePre", preCallback);
            NativeAPI.AddListener("OnEntityTakeDamagePre", secondCallback);
            NativeAPI.AddListener("OnEntityTakeDamagePost", postCallback);

            // Spawn a bot and deal damage to it
            NativeAPI.IssueServerCommand("bot_kick");
            NativeAPI.IssueServerCommand("bot_add");
            await WaitOneFrame();

            var player = Utilities.GetPlayers().FirstOrDefault(p => p.IsBot);
            var playerHealth = player.PlayerPawn.Value.Health;
            DealDamageFunc(player, player, 10);

            await WaitOneFrame();
            Assert.Equal(player.PlayerPawn.Value.Health, playerHealth);

            Assert.Equal(1, preCallCount);
            Assert.Equal(0, postCallCount);
        }
        finally
        {
            NativeAPI.RemoveListener("OnEntityTakeDamagePre", preCallback);
            NativeAPI.RemoveListener("OnEntityTakeDamagePre", secondCallback);
            NativeAPI.RemoveListener("OnEntityTakeDamagePost", postCallback);
        }
    }

    private static void DealDamageFunc(CCSPlayerController attacker, CCSPlayerController victim, int damage,
        object data = null, DamageTypes_t type = DamageTypes_t.DMG_ENERGYBEAM)
    {
        var size = Schema.GetClassSize("CTakeDamageInfo");
        var ptr = Marshal.AllocHGlobal(size);

        for (var i = 0; i < size; i++)
            Marshal.WriteByte(ptr, i, 0);

        var damageInfo = new CTakeDamageInfo(ptr);
        var attackerInfo = new AttackerInfo_t()
        {
            AttackerPawn = attacker.Pawn.Raw, AttackerPlayerSlot = attacker.Slot,
            IsPawn = true, NeedInit = true,
        };

        Marshal.StructureToPtr(attackerInfo, new IntPtr(ptr.ToInt64() + 0x88), false);

        if (attacker.Team == victim.Team)
            attacker = victim;

        Schema.SetSchemaValue(damageInfo.Handle, "CTakeDamageInfo", "m_hInflictor",
            attacker.PawnIsAlive ? attacker.Pawn.Raw : attacker.PlayerPawn.Raw);
        Schema.SetSchemaValue(damageInfo.Handle, "CTakeDamageInfo", "m_hAttacker", attacker.Pawn.Raw);

        damageInfo.Damage = damage;
        damageInfo.BitsDamageType = type;
        if (type == DamageTypes_t.DMG_ENERGYBEAM)
            damageInfo.DamageFlags = TakeDamageFlags_t.DFLAG_IGNORE_ARMOR;

        size = Schema.GetClassSize("CTakeDamageResult");
        var ptr2 = Marshal.AllocHGlobal(size);

        for (var i = 0; i < size; i++)
            Marshal.WriteByte(ptr2, i, 0);

        var damageResult = new CTakeDamageResult(ptr2);
        Schema.SetSchemaValue(damageResult.Handle, "CTakeDamageResult", "m_pOriginatingInfo", damageInfo.Handle);

        damageResult.HealthBefore = victim.PlayerPawn.Value.Health;
        damageResult.HealthLost = damage;
        damageResult.DamageDealt = damage;
        damageResult.PreModifiedDamage = damage;
        damageResult.TotalledHealthLost = damage;
        damageResult.TotalledDamageDealt = damage;
        damageResult.WasDamageSuppressed = false;

        VirtualFunctions.CBaseEntity_TakeDamageOldFunc.Invoke(victim.Pawn.Value, damageInfo, damageResult);
        Marshal.FreeHGlobal(ptr);
        Marshal.FreeHGlobal(ptr2);
    }
}

[StructLayout(LayoutKind.Sequential)]
public struct AttackerInfo_t
{
    public bool NeedInit;
    public bool IsPawn;
    public bool IsWorld;
    public uint AttackerPawn;
    public int AttackerPlayerSlot;
    public int TeamChecked;
    public int Team;
};