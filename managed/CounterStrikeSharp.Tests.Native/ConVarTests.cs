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
}