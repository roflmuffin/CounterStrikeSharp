using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Utils;
using Xunit;

namespace NativeTestsPlugin;

public class ConVarTests
{
    [Fact]
    public async Task BoolConVar()
    {
        Server.ExecuteCommand("sv_cheats 1");
        await WaitOneFrame();

        var boolConVar = ConVar<bool>.Find("sv_cheats");
        Assert.NotNull(boolConVar);
        Assert.Equal("sv_cheats", boolConVar.Name);
        Assert.Equal(ConVarType.Bool, boolConVar.Type);
        Assert.Equal(ConVarFlags.FCVAR_NOTIFY | ConVarFlags.FCVAR_REPLICATED | ConVarFlags.FCVAR_RELEASE, boolConVar.Flags);
        Assert.True(boolConVar.Value);

        boolConVar.Value = false;
        Assert.False(boolConVar.Value);
    }

    [Fact]
    public async Task IntConVar()
    {
        Server.ExecuteCommand("mp_td_dmgtokick 300");
        await WaitOneFrame();

        var intConVar = ConVar<int>.Find("mp_td_dmgtokick");
        Assert.NotNull(intConVar);
        Assert.Equal("mp_td_dmgtokick", intConVar.Name);
        Assert.Equal(ConVarType.Int32, intConVar.Type);
        Assert.Equal(300, intConVar.Value);

        intConVar.Value = 500;
        Assert.Equal(500, intConVar.Value);
    }

    [Fact]
    public async Task FloatConVar()
    {
        Server.ExecuteCommand("inferno_damage 40.0");
        await WaitOneFrame();

        var floatConVar = ConVar<float>.Find("inferno_damage");
        Assert.NotNull(floatConVar);
        Assert.Equal(ConVarType.Float32, floatConVar.Type);
        Assert.Equal(40.0, floatConVar.Value);

        floatConVar.Value = 50.0f;
        Assert.Equal(50.0f, floatConVar.Value);
    }

    [Fact]
    public async Task VectorConVar()
    {
        Server.ExecuteCommand("fog_color -1 -1 -1");
        await WaitOneFrame();

        var vectorConVar = ConVar<Vector>.Find("fog_color");
        Assert.NotNull(vectorConVar);
        Assert.Equal(-1, vectorConVar.Value.X);
        Assert.Equal(-1, vectorConVar.Value.Y);
        Assert.Equal(-1, vectorConVar.Value.Z);

        vectorConVar.Value = new Vector(0, 0, 0);
        Assert.Equal(0, vectorConVar.Value.X);
        Assert.Equal(0, vectorConVar.Value.Y);
        Assert.Equal(0, vectorConVar.Value.Z);
    }

    [Fact]
    public async Task StringConVar()
    {
        Server.ExecuteCommand("mp_backup_round_file backup");
        await WaitOneFrame();

        var stringConVar = ConVar<string>.Find("mp_backup_round_file");
        Assert.NotNull(stringConVar);
        Assert.Equal("backup", stringConVar.Value);

        stringConVar.Value = "new_backup";
    }

    [Fact]
    public void CreateBoolConVar()
    {
        ConVar<bool>.Find("test_bool_convar")?.Delete();

        var conVar = new ConVar<bool>("test_bool_convar", "Test boolean ConVar", true, ConVarFlags.FCVAR_NOTIFY);
        Assert.NotNull(conVar);
        Assert.Equal("test_bool_convar", conVar.Name);
        Assert.Equal(ConVarType.Bool, conVar.Type);
        Assert.Equal("Test boolean ConVar", conVar.Description);
        Assert.Equal(
            ConVarFlags.FCVAR_NOTIFY | ConVarFlags.FCVAR_GAMEDLL | ConVarFlags.FCVAR_RELEASE | ConVarFlags.FCVAR_CLIENT_CAN_EXECUTE,
            conVar.Flags);
        Assert.True(conVar.Value);

        conVar.Delete();

        var found = ConVar<bool>.Find("test_bool_convar");
        Assert.Null(found);
    }

    [Fact]
    public void CreateVectorConVar()
    {
        ConVar<Vector>.Find("test_vector_convar")?.Delete();

        var conVar = new ConVar<Vector>(new ConVar<Vector>.ConVarCreationOptions()
        {
            Name = "test_vector_convar",
            DefaultValue = new Vector(1, 2, 3),
            Description = "Test vector ConVar",
            Flags = ConVarFlags.FCVAR_NOTIFY,
            MinValue = new Vector(0, 0, 0),
            MaxValue = new Vector(100, 100, 100)
        });

        Assert.NotNull(conVar);
        Assert.Equal("test_vector_convar", conVar.Name);
        Assert.Equal(ConVarType.Vector3, conVar.Type);
        Assert.Equal("Test vector ConVar", conVar.Description);
        Assert.Equal(
            ConVarFlags.FCVAR_NOTIFY | ConVarFlags.FCVAR_GAMEDLL | ConVarFlags.FCVAR_RELEASE | ConVarFlags.FCVAR_CLIENT_CAN_EXECUTE,
            conVar.Flags);
        Assert.Equal(1, conVar.Value.X);
        Assert.Equal(2, conVar.Value.Y);
        Assert.Equal(3, conVar.Value.Z);

        conVar.Value = new Vector(500, 500, 500);

        // Test min/max constraints
        Assert.Equal(100, conVar.Value.X);
        Assert.Equal(100, conVar.Value.Y);
        Assert.Equal(100, conVar.Value.Z);

        conVar.Delete();

        var found = ConVar<Vector>.Find("test_vector_convar");
        Assert.Null(found);
    }

    [Fact]
    public void CreateStringConVar()
    {
        ConVar<string>.Find("test_string_convar")?.Delete();

        var conVar = new ConVar<string>("test_string_convar", "Test string ConVar", "default_value", ConVarFlags.FCVAR_NOTIFY);
        Assert.NotNull(conVar);
        Assert.Equal("test_string_convar", conVar.Name);
        Assert.Equal(ConVarType.String, conVar.Type);
        Assert.Equal("Test string ConVar", conVar.Description);
        Assert.Equal(
            ConVarFlags.FCVAR_NOTIFY | ConVarFlags.FCVAR_GAMEDLL | ConVarFlags.FCVAR_RELEASE | ConVarFlags.FCVAR_CLIENT_CAN_EXECUTE,
            conVar.Flags);
        Assert.Equal("default_value", conVar.Value);

        conVar.Delete();

        var found = ConVar<string>.Find("test_string_convar");
        Assert.Null(found);
    }

    [Fact]
    public void CreateFloatConVar()
    {
        ConVar<float>.Find("test_float_convar")?.Delete();

        var conVar = new ConVar<float>(new ConVar<float>.ConVarCreationOptions()
        {
            Name = "test_float_convar",
            DefaultValue = 1.23f,
            Description = "Test float ConVar",
            Flags = ConVarFlags.FCVAR_NOTIFY,
            MinValue = 0f,
            MaxValue = 25f
        });
        Assert.NotNull(conVar);
        Assert.Equal("test_float_convar", conVar.Name);
        Assert.Equal(ConVarType.Float32, conVar.Type);
        Assert.Equal("Test float ConVar", conVar.Description);
        Assert.Equal(
            ConVarFlags.FCVAR_NOTIFY | ConVarFlags.FCVAR_GAMEDLL | ConVarFlags.FCVAR_RELEASE | ConVarFlags.FCVAR_CLIENT_CAN_EXECUTE,
            conVar.Flags);
        Assert.Equal(1.23f, conVar.Value);

        // Test min/max constraints
        conVar.Value = 50.0f;
        Assert.Equal(25.0f, conVar.Value);
        Assert.Equal("25.000000", conVar.ValueAsString);

        conVar.ValueAsString = "10.5";
        Assert.Equal(10.5f, conVar.Value);

        conVar.Delete();

        var found = ConVar<float>.Find("test_float_convar");
        Assert.Null(found);
    }

    [Fact]
    public void CreateIntConVar()
    {
        ConVar<int>.Find("test_int_convar")?.Delete();

        var conVar = new ConVar<int>(new ConVar<int>.ConVarCreationOptions()
        {
            Name = "test_int_convar",
            DefaultValue = 42,
            Description = "Test int ConVar",
            Flags = ConVarFlags.FCVAR_NOTIFY,
            MinValue = 0,
            MaxValue = 100
        });
        Assert.NotNull(conVar);
        Assert.Equal("test_int_convar", conVar.Name);
        Assert.Equal(ConVarType.Int32, conVar.Type);
        Assert.Equal("Test int ConVar", conVar.Description);
        Assert.Equal(
            ConVarFlags.FCVAR_NOTIFY | ConVarFlags.FCVAR_GAMEDLL | ConVarFlags.FCVAR_RELEASE | ConVarFlags.FCVAR_CLIENT_CAN_EXECUTE,
            conVar.Flags);
        Assert.Equal(42, conVar.Value);

        // Test min/max constraints
        conVar.Value = 150;
        Assert.Equal(100, conVar.Value);

        conVar.Delete();

        var found = ConVar<int>.Find("test_int_convar");
        Assert.Null(found);
    }
}