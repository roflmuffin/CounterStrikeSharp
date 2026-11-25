using System.Collections.Generic;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
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

    [Fact]
    public void ConVar_FindsAndReadsAnEngineConVar()
    {
        var conVar = ConVar.Find("mp_roundtime");

        Assert.NotNull(conVar);
        Assert.Equal("mp_roundtime", conVar!.Name);
        Assert.True(conVar.GetPrimitiveValue<float>() > 0);
    }
}
