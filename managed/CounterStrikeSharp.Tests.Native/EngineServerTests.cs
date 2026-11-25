using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.ValveConstants.Protobuf;
using Xunit;

namespace NativeTestsPlugin;

// IVEngineServer2's SetClientListening hook and direct calls go through Slot Bindings, at the slot index the boot
// Vtable Check verified (or healed); each plugin-visible API is its own Engine Feature. Map validity and server
// commands are exercised by EngineTests and nearly every test; the player calls run on a test bot here. PrintToConsole
// is not: the engine's ClientPrintf crashes the server on a bot (as before Slot Bindings; there is no real client here).
public class EngineServerTests
{
    public static IEnumerable<object[]> Features() =>
    [
        [EngineFeature.PlayerSetListening], [EngineFeature.ServerExecuteCommand], [EngineFeature.ServerIsMapValid],
        [EngineFeature.ServerPrecacheModel], [EngineFeature.PlayerUserId], [EngineFeature.PlayerPrintToConsole],
        [EngineFeature.PlayerGetConvarValue], [EngineFeature.PlayerSetFakeClientConvar], [EngineFeature.PlayerExecuteClientCommand],
        [EngineFeature.PlayerDisconnect], [EngineFeature.PlayerAuthorization],
    ];

    [Theory]
    [MemberData(nameof(Features))]
    public void EngineFeature_IsAvailable(EngineFeature feature)
    {
        var status = EngineFeatures.GetStatus(feature);

        Assert.True(status.IsAvailable, status.ToString());
    }

    // Passes either way: only while is-map-valid is disabled (E2E corrupts its fingerprint), IsMapValid throws.
    [Fact]
    public void IsMapValid_ThrowsTypedExceptionOnlyWhileDisabled()
    {
        if (EngineFeatures.IsAvailable(EngineFeature.ServerIsMapValid)) return;

        var exception = Assert.Throws<EngineFeatureUnavailableException>(() => Server.IsMapValid(Server.MapName));
        Assert.Equal(EngineFeatureNames.ServerIsMapValid, exception.EngineFeature);
        Assert.Equal("IVEngineServer2::IsMapValid", exception.Binding);
        Console.WriteLine($"[EngineServerTests] IsMapValid threw {exception.GetType().Name}: {exception.Message}");
    }

    [Fact]
    public async Task UserId_IsSetForABot()
    {
        var (player, _) = await CreateTestPlayerAsync();

        Assert.True(player.UserId is >= 0, $"UserId {player.UserId}");
    }

    [Fact]
    public async Task SetFakeClientConVar_IsReadBackByGetConVarValue()
    {
        var (player, _) = await CreateTestPlayerAsync();

        player.SetFakeClientConVar("cl_cssharp_e2e", "42");

        Assert.Equal("42", player.GetConVarValue("cl_cssharp_e2e"));
    }

    [Fact]
    public async Task ExecuteClientCommand_RunsForABot()
    {
        var (player, _) = await CreateTestPlayerAsync();

        player.ExecuteClientCommand("echo cssharp e2e");
        Assert.Null(player.AuthorizedSteamID); // bots are never authorized
    }

    [Fact]
    public async Task VoiceFlagsAndListenOverride_AreStored()
    {
        var (player, _) = await CreateTestPlayerAsync();

        player.VoiceFlags = VoiceFlags.Muted;
        player.SetListenOverride(player, ListenOverride.Mute);

        Assert.Equal(VoiceFlags.Muted, player.VoiceFlags);
        Assert.Equal(ListenOverride.Mute, player.GetListenOverride(player));
        player.VoiceFlags = VoiceFlags.Normal;
        player.SetListenOverride(player, ListenOverride.Default);
    }

    [Fact]
    public async Task Disconnect_RemovesTheBot()
    {
        var (player, _) = await CreateTestPlayerAsync();

        player.Disconnect(NetworkDisconnectionReason.NETWORK_DISCONNECT_KICKED);

        await WaitUntilAsync(() => !player.IsValid || player.Connected != PlayerConnectedState.Connected, "the bot to disconnect");
    }
}
