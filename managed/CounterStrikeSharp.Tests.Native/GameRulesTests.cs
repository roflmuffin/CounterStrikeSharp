using System;
using System.Linq;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using Xunit;

namespace NativeTestsPlugin;

public class GameRulesTests
{
    private static bool _askedOnLoad;
    private static bool _nullOnLoad;

    internal static void AskOnLoad()
    {
        _nullOnLoad = Utilities.GetGameRules() == null;
        Console.WriteLine(_nullOnLoad
            ? "[GameRulesTests] GetGameRules during Load returned null, as it does on a cold start"
            : "[GameRulesTests] GetGameRules during Load returned the game rules: the map was already up, the cold start case was not exercised");
        _askedOnLoad = true;
    }

    [Fact]
    public void GetGameRules_MatchesTheEntityScan()
    {
        var scanned = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First().GameRules;
        var cached = Utilities.GetGameRules();

        Assert.NotNull(scanned);
        Assert.NotNull(cached);
        Assert.Equal(scanned!.Handle, cached!.Handle);
    }

    [Fact]
    public void GetGameRules_AskedDuringLoad_ResolvesOnceTheMapIsUp()
    {
        Assert.True(_askedOnLoad);
        Assert.NotNull(Utilities.GetGameRules());
    }

    [Fact]
    public void GetGameRules_ReturnsTheSameEntityTwice()
    {
        Assert.Equal(Utilities.GetGameRules()!.Handle, Utilities.GetGameRules()!.Handle);
    }
}

[Collection("MapChange")]
public class GameRulesMapChangeTests
{
    [Fact]
    public async Task GetGameRules_AfterAMapChange_ReturnsTheNewMapsRules()
    {
        var oldProxy = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First();
        var oldHandle = oldProxy.EntityHandle.Raw;
        Assert.Equal(oldProxy.GameRules!.Handle, Utilities.GetGameRules()!.Handle);

        Server.ExecuteCommand($"changelevel {Server.MapName}");
        await WaitUntilAsync(
            () => EntitySystem.GetEntityByHandle(oldHandle) == null
                  && Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").FirstOrDefault()?.GameRules != null,
            "the next map to create its game rules", timeoutSeconds: 90);

        var proxy = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules").First();
        Assert.NotEqual(oldHandle, proxy.EntityHandle.Raw);
        Assert.Equal(proxy.GameRules!.Handle, Utilities.GetGameRules()!.Handle);
    }
}
