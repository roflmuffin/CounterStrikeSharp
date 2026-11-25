using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Memory;
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
            // Leave a bot in the server, as other tests expect a player.
            Server.ExecuteCommand("bot_quota_mode normal; bot_quota 1");
        }
        await WaitUntilAsync(() => Utilities.GetPlayers().Any(p => p.IsBot), "a bot to rejoin");

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
    public async Task ClientCommand_ReachesCommandListeners_OnlyWhileClientCommandsIsAvailable()
    {
        // Bots never send commands to the server, so the test calls IServerGameClients::ClientCommand itself, through
        // the vtable (where the hook sits), for an empty player slot: the game ignores a command from a slot with no
        // player, so only our hook can pass it on to the command's listeners.
        const string command = "css_itest_client_command";
        var probeSlot = Enumerable.Range(0, Server.MaxPlayers).Reverse().FirstOrDefault(
            slot => Utilities.GetPlayerFromSlot(slot) == null, -1);
        Assert.True(probeSlot >= 0, $"no empty player slot among {Server.MaxPlayers}");
        var gameClients = ValveInterface.ServerGameClients.Pointer;
        var clientCommand = VirtualFunction.CreateVoid<IntPtr, int, IntPtr>(gameClients,
            GameData.GetOffset("IServerGameClients::ClientCommand"));

        var sent = false;
        var calls = 0;
        var callback = FunctionReference.Create((int slot, IntPtr commandInfo) =>
        {
            if (!sent)
            {
                // From the server console: send the same command as if the probe slot's player had typed it.
                sent = true;
                clientCommand(gameClients, probeSlot, commandInfo);
            }
            else if (slot == probeSlot)
            {
                calls++;
            }

            return HookResult.Continue;
        });

        NativeAPI.AddCommand(command, "client command probe", true, (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND, callback);
        try
        {
            NativeAPI.IssueServerCommand(command);
            await WaitOneFrame();
            await WaitOneFrame();
        }
        finally
        {
            NativeAPI.RemoveCommand(command, callback);
        }

        Console.WriteLine($"[ServerGameClientsTests] client-commands available: {ClientCommandsAvailable}; sent {sent}, " +
                          $"listener calls {calls} (probe slot {probeSlot})");
        Assert.True(sent, "the server command did not run");
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
