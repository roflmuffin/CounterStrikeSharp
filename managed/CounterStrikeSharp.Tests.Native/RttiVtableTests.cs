using System;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;
using Xunit;

namespace NativeTestsPlugin;

// Finding a class's vtable by RTTI on the live server: exact class name only, one primary vtable.
public class RttiVtableTests
{
    [Theory]
    [InlineData("CGameEventManager")]
    [InlineData("HUDPanelDialogVariableString_t")]
    [InlineData("CNavPhysicsInterface")]
    [InlineData("CCSGameRules")]
    [InlineData("CCSPlayerController")]
    [InlineData("CCSPlayerPawn")]
    public void ClassesLookedUpByRtti_Resolve(string className)
    {
        Assert.NotEqual(IntPtr.Zero, NativeAPI.FindVirtualTable(Addresses.ServerPath, className));
    }

    [Fact]
    public void GameRulesVtable_IsTheOneTheLiveGameRulesUse()
    {
        // "12CCSGameRules" also occurs inside the pointer type's name "P12CCSGameRules"; the lookup must not depend on
        // which comes first. The live object's vtable pointer is the independent truth.
        var gameRules = Utilities.GetGameRules();
        Assert.NotNull(gameRules);

        Assert.Equal(Marshal.ReadIntPtr(gameRules!.Handle), NativeAPI.FindVirtualTable(Addresses.ServerPath, "CCSGameRules"));
    }

    [Fact]
    public void UnknownClass_IsNotFound()
    {
        Assert.Equal(IntPtr.Zero, NativeAPI.FindVirtualTable(Addresses.ServerPath, "CNoSuchClassAnywhere"));
    }
}
