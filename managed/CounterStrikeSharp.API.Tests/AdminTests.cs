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
        Assert.NotNull(adminData);
        Assert.Equal("#css/admin", adminData.Groups.Single());
    }

    [Fact]
    public void ShouldUseGroupImmunity()
    {
        var adminData = AdminManager.GetPlayerAdminData((SteamID)76561197960265731);
        Assert.NotNull(adminData);
        Assert.Equal(125u, AdminManager.GetPlayerImmunity((SteamID)76561197960265731)); // Group immunity is 125, Admin immunity is 100
        AdminManager.SetPlayerImmunity((SteamID)76561197960265731, 150u);
        Assert.Equal(150u, AdminManager.GetPlayerImmunity((SteamID)76561197960265731)); // Group immunity is 125, Admin immunity is 100
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
        Assert.NotNull(adminData);
        Assert.True(adminData.CommandOverrides["fake_command"]);
        Assert.False(adminData.CommandOverrides["css"]);
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
        // Existing player
        Assert.False(AdminManager.PlayerHasPermissions((SteamID)76561197960265731, "@runtime/flag1"));
        AdminManager.AddPlayerPermissions((SteamID)76561197960265731, "@runtime/flag1");
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265731, "@runtime/flag1"));
        
        // Non-existent player
        Assert.False(AdminManager.PlayerHasPermissions((SteamID)76561197960265730, "@runtime/flag1"));
        AdminManager.AddPlayerPermissions((SteamID)76561197960265730, "@runtime/flag1");
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265730, "@runtime/flag1"));
        
        AdminManager.ClearPlayerPermissions((SteamID)76561197960265730);
        Assert.False(AdminManager.PlayerHasPermissions((SteamID)76561197960265730, "@runtime/flag1"));
        Assert.Empty(AdminManager.GetPlayerAdminData((SteamID)76561197960265730)!.GetAllFlags());
    }

    [Fact]
    public void ShouldAddPlayerToGroup()
    {
        // AddPlayerToGroup will create a new AdminData instance here, as "STEAM_0:1:3" does not
        // exist internally at this stage. Afterwards, AddPlayerToGroup will add "#css/root" as
        // a group.
        AdminManager.AddPlayerToGroup(new SteamID("STEAM_0:1:3"), "#css/root");
        var adminData = AdminManager.GetPlayerAdminData(new SteamID("STEAM_0:1:3"));
        Assert.NotNull(adminData);
        Assert.True(adminData.Groups.Any());
        Assert.Equal("#css/root", adminData.Groups.Single());
    }

    [Fact]
    public void ShouldAddPlayerToGroupAtRuntime()
    {
        // Similar to ShouldPlayerAddToGroup, but different in that we create AdminData
        // manually first, then add the player into the group.
        // Aimed to resolve an issue raised on Discord by r3troattack.

        AdminManager.CreateAdminData((SteamID)76561197960265999);
        Assert.False(AdminManager.PlayerInGroup((SteamID)76561197960265999, "#css/root"));
        AdminManager.AddPlayerToGroup((SteamID)76561197960265999, "#css/root");

        var adminData = AdminManager.GetPlayerAdminData((SteamID)76561197960265999);
        Assert.NotNull(adminData);
        Assert.True(adminData.Groups.Any());
        Assert.Equal("#css/root", adminData.Groups.Single());
    }

    [Fact]
    public void ShouldAddPlayerPermissionOverridesAtRuntime()
    {
        Assert.False(AdminManager.PlayerHasCommandOverride((SteamID)76561197960265731, "runtime_command"));
        AdminManager.SetPlayerCommandOverride((SteamID)76561197960265731, "runtime_command", true);
        Assert.True(AdminManager.PlayerHasCommandOverride((SteamID)76561197960265731, "runtime_command"));
    }
    
    [Fact]
    public void ShouldAddCommandPermissionOverridesAtRuntime()
    {
        Assert.False(AdminManager.CommandIsOverriden("runtime_command_a"));
        AdminManager.AddPermissionOverride("runtime_command_a", "@runtime/override");
        Assert.True(AdminManager.CommandIsOverriden("runtime_command_a"));
        Assert.Equal("@runtime/override", AdminManager.GetPermissionOverrides("runtime_command_a").Single());
    }

    [Fact]
    public void ShouldAddCommandPermissionOverridesWithEmpty()
    {
        Assert.False(AdminManager.CommandIsOverriden("runtime_command_b"));
        AdminManager.AddPermissionOverride("runtime_command_b");
        Assert.True(AdminManager.CommandIsOverriden("runtime_command_b"));
        Assert.False(AdminManager.GetPermissionOverrides("runtime_command_b").Any());
    }

    [Fact]
    public void ShouldAllowRootFlagInRequiresPermissions()
    {
        Assert.False(AdminManager.PlayerHasPermissions((SteamID)76561197960265732, "@css/root"));
        AdminManager.AddPlayerPermissions((SteamID)76561197960265732, "@css/root");
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265732, "@css/root"));

        var adminData = AdminManager.GetPlayerAdminData((SteamID)76561197960265732);
        Assert.NotNull(adminData);
        Assert.True(adminData.DomainHasRootFlag("css"));

        var requirePerms = new RequiresPermissions("@css/ban");
        Assert.NotNull(requirePerms);
        Assert.True(requirePerms.CanExecuteCommand((SteamID)76561197960265732));
    }

    [Fact]
    public void ShouldRequiresPermissionsPass()
    {
        Assert.False(AdminManager.PlayerHasPermissions((SteamID)76561197960265738, "@css/ban"));
        AdminManager.AddPlayerPermissions((SteamID)76561197960265738, "@css/ban");
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265738, "@css/ban"));

        var requirePerms = new RequiresPermissions("@css/ban");
        Assert.NotNull(requirePerms);
        Assert.True(requirePerms.CanExecuteCommand((SteamID)76561197960265738));
    }

    [Fact]
    public void ShouldRequiresPermissionsOrPassSameDomain()
    {
        Assert.False(AdminManager.PlayerHasPermissions((SteamID)76561197960265739, "@css/ban"));
        AdminManager.AddPlayerPermissions((SteamID)76561197960265739, "@css/ban");
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265739, "@css/ban"));

        var requirePerms = new RequiresPermissionsOr("@css/ban", "@css/kick");
        Assert.NotNull(requirePerms);
        Assert.True(requirePerms.CanExecuteCommand((SteamID)76561197960265739));
    }

    [Fact]
    public void ShouldRequiresPermissionsOrPassDifferentDomains()
    {
        Assert.False(AdminManager.PlayerHasPermissions((SteamID)76561197960265740, "@css/ban"));
        AdminManager.AddPlayerPermissions((SteamID)76561197960265740, "@css/ban");
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265740, "@css/ban"));

        var requirePerms = new RequiresPermissionsOr("@css/ban", "@domain2/flag");
        Assert.NotNull(requirePerms);
        Assert.True(requirePerms.CanExecuteCommand((SteamID)76561197960265740));
    }

    [Fact]
    public void ShouldRequiresPermissionsCheckFail()
    {
        Assert.False(AdminManager.PlayerHasPermissions((SteamID)76561197960265735, "@css/ban"));
        AdminManager.AddPlayerPermissions((SteamID)76561197960265735, "@css/ban");
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265735, "@css/ban"));

        var requirePerms = new RequiresPermissions("@css/kick");
        Assert.NotNull(requirePerms);
        Assert.False(requirePerms.CanExecuteCommand((SteamID)76561197960265735));
    }

    [Fact]
    public void ShouldRequiresPermissionsOrCheckFailSameDomain()
    {
        Assert.False(AdminManager.PlayerHasPermissions((SteamID)76561197960265736, "@css/ban"));
        AdminManager.AddPlayerPermissions((SteamID)76561197960265736, "@css/ban");
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265736, "@css/ban"));

        var requirePerms = new RequiresPermissionsOr("@css/kick", "@css/vote");
        Assert.NotNull(requirePerms);
        Assert.False(requirePerms.CanExecuteCommand((SteamID)76561197960265736));
    }

    [Fact]
    public void ShouldRequiresPermissionsOrCheckFailDifferentDomains()
    {
        Assert.False(AdminManager.PlayerHasPermissions((SteamID)76561197960265737, "@css/ban"));
        AdminManager.AddPlayerPermissions((SteamID)76561197960265737, "@css/ban");
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265737, "@css/ban"));

        var requirePerms = new RequiresPermissionsOr("@css/kick", "@domain2/flag");
        Assert.NotNull(requirePerms);
        Assert.False(requirePerms.CanExecuteCommand((SteamID)76561197960265737));
    }

    [Fact]
    public void ShouldAllowRootFlagInRequiresPermissionsOr()
    {
        Assert.False(AdminManager.PlayerHasPermissions((SteamID)76561197960265733, "@css/root"));
        AdminManager.AddPlayerPermissions((SteamID)76561197960265733, "@css/root");
        Assert.True(AdminManager.PlayerHasPermissions((SteamID)76561197960265733, "@css/root"));

        var adminData = AdminManager.GetPlayerAdminData((SteamID)76561197960265733);
        Assert.NotNull(adminData);
        Assert.True(adminData.DomainHasRootFlag("css"));

        var requirePerms = new RequiresPermissionsOr("@css/ban", "@css/kick", "@domain2/flag");
        Assert.NotNull(requirePerms);
        Assert.True(requirePerms.CanExecuteCommand((SteamID)76561197960265733));
    }
}
