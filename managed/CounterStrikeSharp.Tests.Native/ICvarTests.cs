using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Cvars;
using Xunit;

namespace NativeTestsPlugin;

// Every ICvar call (the console dispatch hook, our direct calls and the SDK's ConVar/ConCommand code) goes through a
// Slot Binding at the slot index the boot Vtable Check verified (or healed). CommandTests and ConVarTests exercise the
// APIs themselves.
public class ICvarTests
{
    public static IEnumerable<object[]> Features() =>
    [
        [EngineFeature.CommandsConsoleDispatch], [EngineFeature.CommandsAddCommand], [EngineFeature.PlayerExecuteClientCommandFromServer],
        [EngineFeature.ConvarsFind], [EngineFeature.ConvarsRead], [EngineFeature.ConvarsWrite], [EngineFeature.ConvarsCreate],
        [EngineFeature.ConvarsChangeEvents],
    ];

    [Theory]
    [MemberData(nameof(Features))]
    public void EngineFeature_IsAvailable(EngineFeature feature)
    {
        var status = EngineFeatures.GetStatus(feature);

        Assert.True(status.IsAvailable, status.ToString());
    }

    // A server console command reaches us only through the DispatchConCommand hook. The command buffer may run a few
    // frames later, so wait until it did (or give up).
    [Fact]
    public async Task ServerConsoleCommand_ReachesItsCallbackAndListenersThroughTheDispatchHook()
    {
        const string name = "css_icvar_dispatch_test";
        var calls = 0;
        var preListenerCalls = 0;
        var postListenerCalls = 0;
        var callback = FunctionReference.Create(() => { calls++; });
        var preListener = FunctionReference.Create(() => { preListenerCalls++; });
        var postListener = FunctionReference.Create(() => { postListenerCalls++; });

        NativeAPI.AddCommand(name, "ICvar dispatch test", true, (int)ConCommandFlags.FCVAR_LINKED_CONCOMMAND, callback);
        NativeAPI.AddCommandListener(name, preListener, false);
        NativeAPI.AddCommandListener(name, postListener, true);
        try
        {
            NativeAPI.IssueServerCommand(name);
            for (var frame = 0; frame < 32 && (calls == 0 || postListenerCalls == 0); frame++) await WaitOneFrame();

            Assert.Equal(1, calls);
            Assert.Equal(1, preListenerCalls);
            Assert.Equal(1, postListenerCalls);
        }
        finally
        {
            NativeAPI.RemoveCommandListener(name, preListener, false);
            NativeAPI.RemoveCommandListener(name, postListener, true);
            NativeAPI.RemoveCommand(name, callback);
        }
    }

    [Fact]
    public void ConVar_FindsAndReadsAnEngineConVar()
    {
        var conVar = ConVar.Find("mp_roundtime");

        Assert.NotNull(conVar);
        Assert.Equal("mp_roundtime", conVar!.Name);
        Assert.True(conVar.GetPrimitiveValue<float>() > 0);
    }
}
