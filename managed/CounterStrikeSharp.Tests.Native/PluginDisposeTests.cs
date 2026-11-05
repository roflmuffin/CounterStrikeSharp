using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using Xunit;

namespace NativeTestsPlugin;

public class PluginDisposeTests
{
    private class ThrowawayPlugin : BasePlugin
    {
        public override string ModuleName => "Throwaway";
        public override string ModuleVersion => "0";
    }

    [Fact]
    public async Task Dispose_UnhooksACommandListenerRegisteredForTwoCommands()
    {
        var calls = 0;
        HookResult Listener(CCSPlayerController? player, CommandInfo info)
        {
            calls++;
            return HookResult.Continue;
        }

        var plugin = new ThrowawayPlugin();
        plugin.AddCommandListener("echo", Listener);
        plugin.AddCommandListener("say", Listener);

        NativeAPI.IssueServerCommand("echo before dispose");
        NativeAPI.IssueServerCommand("say before dispose");
        await WaitUntilAsync(() => calls == 2, "both command listeners to fire");

        plugin.Dispose();
        NativeAPI.IssueServerCommand("echo after dispose");
        NativeAPI.IssueServerCommand("say after dispose");
        await WaitForSeconds(0.25f);

        Assert.Equal(2, calls);
    }

    [Fact]
    public async Task Dispose_UnhooksAnEventHandlerRegisteredInBothModes()
    {
        var calls = 0;
        HookResult Handler(EventShowSurvivalRespawnStatus @event, GameEventInfo info)
        {
            calls++;
            return HookResult.Continue;
        }

        var plugin = new ThrowawayPlugin();
        plugin.RegisterEventHandler<EventShowSurvivalRespawnStatus>(Handler, HookMode.Pre);
        plugin.RegisterEventHandler<EventShowSurvivalRespawnStatus>(Handler, HookMode.Post);

        new EventShowSurvivalRespawnStatus(true).FireEvent(true);
        await WaitOneFrame();
        Assert.Equal(2, calls);

        plugin.Dispose();
        new EventShowSurvivalRespawnStatus(true).FireEvent(true);
        await WaitOneFrame();

        Assert.Equal(2, calls);
    }

    [Fact]
    public async Task RemoveCommandListener_OfOneHandlerRegisteredForTwoCommands_RemovesTheOneAskedFor()
    {
        var calls = 0;
        HookResult Listener(CCSPlayerController? player, CommandInfo info)
        {
            calls++;
            return HookResult.Continue;
        }

        var plugin = new ThrowawayPlugin();
        plugin.AddCommandListener("echo", Listener);
        plugin.AddCommandListener("say", Listener);

        try
        {
            plugin.RemoveCommandListener("echo", Listener, HookMode.Pre);
            NativeAPI.IssueServerCommand("echo removed");
            await WaitForSeconds(0.25f);
            Assert.Equal(0, calls);

            NativeAPI.IssueServerCommand("say kept");
            await WaitUntilAsync(() => calls == 1, "the listener kept on say to fire");
        }
        finally
        {
            plugin.Dispose();
        }
    }
}
