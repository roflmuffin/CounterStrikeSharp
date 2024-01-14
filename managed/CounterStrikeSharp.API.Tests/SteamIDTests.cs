using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace CounterStrikeSharp.API.Tests;

public class SteamIdTests
{
    [Theory]
    [InlineData(76561197960524373ul, 76561197960524373ul, "STEAM_0:1:129322", "[U:1:258645]", 258645, SteamAccountType.Individual, SteamAccountInstance.Desktop, SteamAccountUniverse.Public, true)]
    [InlineData(258645, 76561197960524373ul, "STEAM_0:1:129322", "[U:1:258645]", 258645, SteamAccountType.Individual, SteamAccountInstance.Desktop, SteamAccountUniverse.Public, true)]
    [InlineData(76561197960265728ul, 76561197960265728ul, "STEAM_0:0:0", "[U:1:0]", 0, SteamAccountType.Individual, SteamAccountInstance.Desktop, SteamAccountUniverse.Public, false)]
    [InlineData(103582791429521412ul, 103582791429521412ul, "STEAM_0:0:13510796734627842", "[g:1:27021593469255684]", 4, SteamAccountType.Clan, SteamAccountInstance.All, SteamAccountUniverse.Public, true)]
    public void ValidateSteamId(ulong parseValue, ulong steamId64, string steamId2, string steamId3, int steamId32, SteamAccountType accountType, SteamAccountInstance accountInstance, SteamAccountUniverse accountUniverse, bool valid)
    {
        var steamId = new SteamID(parseValue);
        
        Assert.Equal(steamId64, steamId.SteamId64);
        Assert.Equal(steamId2, steamId.SteamId2);
        Assert.Equal(steamId3, steamId.SteamId3);
        Assert.Equal(steamId32, steamId.SteamId32);
        Assert.Equal(accountType, steamId.AccountType);
        Assert.Equal(accountInstance, steamId.AccountInstance);
        Assert.Equal(accountUniverse, steamId.AccountUniverse);
        Assert.Equal(valid, steamId.IsValid());
    }
    
    [Theory]
    [InlineData(76561197960524373ul, 76561197960524373ul)]
    [InlineData("STEAM_0:1:129322", 76561197960524373ul)]
    [InlineData("[U:1:258645]", 76561197960524373ul)]
    public void CanCastLongToString(dynamic parseable, ulong longValue)
    {
        Assert.Equal(longValue, ((SteamID)parseable).SteamId64);
    }
    
    [Fact]
    public void CanUseValueEquality()
    {
        var steamId1 = new SteamID(76561197960524373ul);
        var steamId2 = new SteamID(76561197960524373ul);
        var steamId3 = new SteamID(76561197960265728ul);
        
        Assert.True(steamId1 == steamId2);
        Assert.True(steamId1 != steamId3);
    }
    
    [Fact]
    public void ThrowsOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new SteamID(0));
    }
}