using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Entities;

namespace CounterStrikeSharp.API.Tests;

[Collection("Logging collection")]
public class AdminTests
{
    public AdminTests()
    {
        AdminManager.LoadAdminData(TestUtils.GetTestPath("admins.json"));
        AdminManager.LoadAdminGroups(TestUtils.GetTestPath("admin_groups.json"));
        AdminManager.LoadCommandOverrides(TestUtils.GetTestPath("admin_overrides.json"));
        AdminManager.MergeGroupPermsIntoAdmins();
    }
    
    [Fact]
    public void ShouldReturnValidAdminData()
    {
        var adminData = AdminManager.GetPlayerAdminData((SteamID)76561197960265731);
        Assert.Equal("#css/admin", adminData.Groups.Single());
    }

    [Fact]
    public void ShouldUseGroupImmunity()
    {
        var adminData = AdminManager.GetPlayerAdminData((SteamID)76561197960265731);
        Assert.Equal(125u, adminData.Immunity); // Group immunity is 125, Admin immunity is 100
    }
    
    [Fact]
    public void ShouldReturnNullAdminData()
    {
        var adminData = AdminManager.GetPlayerAdminData((SteamID)76561197960265732);
        Assert.Null(adminData);
    }
    
    [Fact]
    public void ShouldReturnValidCommandOverrides()
    {
        var adminData = AdminManager.GetPlayerAdminData((SteamID)76561197960265731);
        Assert.True(adminData.CommandOverrides["fake_command"]);
        Assert.False(adminData.CommandOverrides["css"]);
        Assert.Equal(3, adminData.CommandOverrides.Count);
    }
    
    [Fact]
    public void ShouldHandleWildcardDomainFlags()
    {
        // User has @mycustomplugin/* so should have the admin subflag despite it not being in their group.
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265731, "@mycustomplugin/admin"));
        
        // User has @mycustomplugin/* so should have the admin subflag despite it not existing anywhere else.
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265731, "@mycustomplugin/fake"));
        
        // User has @css/root so should have the slay subflag despite it not being in their group.
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265731, "@css/slay"));
        
        // Flag provided explicitly in the admins.json file
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265731, "@css/custom-flag-2"));
    }

    [Fact]
    public void ShouldAddFlagsAtRuntime()
    {
        Assert.False(AdminManager.PlayerHasPermissions((SteamID)76561197960265731, "@runtime/flag1"));
        AdminManager.AddPlayerPermissions((SteamID)76561197960265731, "@runtime/flag1");
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265731, "@runtime/flag1"));
    }
}