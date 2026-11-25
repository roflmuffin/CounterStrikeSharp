using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Xunit;

namespace NativeTestsPlugin;

// The IServerGameClients hooks are Slot Bindings, hooked at the slot index the boot Vtable Check verified, each group
// behind its own Engine Feature. The listener tests pass whether their Engine Feature is available or not (E2E breaks
// one): available, the hook fires; disabled, the listener still registers and never fires.
public class ServerGameClientsTests
{
    private static bool ConnectionEventsAvailable => EngineFeatures.IsAvailable(EngineFeature.PlayerConnectionEvents);
    private static bool ClientCommandsAvailable => EngineFeatures.IsAvailable(EngineFeature.PlayerClientCommands);

    [Fact]
    public void ConnectionEvents_IsAvailable() => AssertAvailable(EngineFeature.PlayerConnectionEvents);

    [Fact]
    public void ClientCommands_IsAvailable() => AssertAvailable(EngineFeature.PlayerClientCommands);

    [Fact]
    public void VoiceEvents_IsAvailable() => AssertAvailable(EngineFeature.PlayerVoiceEvents);

    [Fact]
    public void CustomHudLayout_IsAvailable() => AssertAvailable(EngineFeature.UiCustomHudLayout);

    [Fact]
    public async Task ConnectionListeners_FireForABot_OnlyWhileConnectionEventsIsAvailable()
    {
        int connects = 0, putInServers = 0, disconnects = 0;
        var onConnect = FunctionReference.Create((int slot, string name, string ipAddress) => { connects++; });
        var onPutInServer = FunctionReference.Create((int slot) => { putInServers++; });
        var onDisconnect = FunctionReference.Create((int slot, int reason) => { disconnects++; });

        await KickAllBotsAsync();
        try
        {
            NativeAPI.AddListener("OnClientConnect", onConnect);
            NativeAPI.AddListener("OnClientPutInServer", onPutInServer);
            NativeAPI.AddListener("OnClientDisconnect", onDisconnect);

            Server.ExecuteCommand("bot_quota_mode normal; bot_add");
            await WaitUntilAsync(() => Utilities.GetPlayers().Any(p => p.IsBot), "a bot to join");
            await KickAllBotsAsync();
            await WaitOneFrame();
        }
        finally
        {
            NativeAPI.RemoveListener("OnClientConnect", onConnect);
            NativeAPI.RemoveListener("OnClientPutInServer", onPutInServer);
            NativeAPI.RemoveListener("OnClientDisconnect", onDisconnect);
        }

        Console.WriteLine($"[ServerGameClientsTests] connection-events available: {ConnectionEventsAvailable}; " +
                          $"connects {connects}, put in server {putInServers}, disconnects {disconnects}");
        if (ConnectionEventsAvailable)
        {
            Assert.True(connects >= 1, "OnClientConnect did not fire");
            Assert.True(putInServers >= 1, "OnClientPutInServer did not fire");
            Assert.True(disconnects >= 1, "OnClientDisconnect did not fire");
        }
        else
        {
            Assert.Equal((0, 0, 0), (connects, putInServers, disconnects));
        }
    }

    [Fact]
    public async Task ClientTypedCommand_ReachesCommandListeners_OnlyWhileClientCommandsIsAvailable()
    {
        // Not a registered console command, so only the ClientCommand hook can see it.
        const string command = "css_itest_client_typed";
        var calls = 0;
        var listener = FunctionReference.Create((int slot, IntPtr commandInfo) =>
        {
            calls++;
            return HookResult.Continue;
        });

        var (player, _) = await CreateTestPlayerAsync();
        NativeAPI.AddCommandListener(command, listener, false);
        try
        {
            player.ExecuteClientCommand(command);
            await WaitOneFrame();
            await WaitOneFrame();
        }
        finally
        {
            NativeAPI.RemoveCommandListener(command, listener, false);
        }

        Console.WriteLine($"[ServerGameClientsTests] client-commands available: {ClientCommandsAvailable}; listener calls {calls}");
        Assert.Equal(ClientCommandsAvailable ? 1 : 0, calls);
    }

    private static void AssertAvailable(EngineFeature feature)
    {
        var status = EngineFeatures.GetStatus(feature);
        Assert.True(status.IsAvailable, status.ToString());
    }

    private static async Task KickAllBotsAsync()
    {
        Server.ExecuteCommand("bot_quota 0; bot_kick");
        await WaitUntilAsync(() => !Utilities.GetPlayers().Any(p => p.IsBot), "all bots to leave");
    }
}
