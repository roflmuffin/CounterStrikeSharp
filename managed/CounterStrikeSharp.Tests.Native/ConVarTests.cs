using System.Threading.Tasks;
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

        var boolConVar = ConVar.Find("sv_cheats");
        Assert.NotNull(boolConVar);
        Assert.Equal("sv_cheats", boolConVar.Name);
        Assert.Equal(ConVarType.Bool, boolConVar.Type);
        Assert.Equal(ConVarFlags.FCVAR_NOTIFY | ConVarFlags.FCVAR_REPLICATED | ConVarFlags.FCVAR_RELEASE, boolConVar.Flags);
        Assert.True(boolConVar.GetPrimitiveValue<bool>());

        boolConVar.GetPrimitiveValue<bool>() = false;
        Assert.False(boolConVar.GetPrimitiveValue<bool>());
        Server.ExecuteCommand("sv_cheats 1");
        await WaitOneFrame();
    }

    [Fact]
    public async Task IntConVar()
    {
        Server.ExecuteCommand("mp_td_dmgtokick 300");
        await WaitOneFrame();

        var intConVar = ConVar.Find("mp_td_dmgtokick");
        Assert.NotNull(intConVar);
        Assert.Equal("mp_td_dmgtokick", intConVar.Name);
        Assert.Equal(ConVarType.Int32, intConVar.Type);
        Assert.Equal(300, intConVar.GetPrimitiveValue<int>());

        intConVar.GetPrimitiveValue<int>() = 500;
        Assert.Equal(500, intConVar.GetPrimitiveValue<int>());
    }

    [Fact]
    public async Task FloatConVar()
    {
        Server.ExecuteCommand("inferno_damage 40.0");
        await WaitOneFrame();

        var floatConVar = ConVar.Find("inferno_damage");
        Assert.NotNull(floatConVar);
        Assert.Equal(ConVarType.Float32, floatConVar.Type);
        Assert.Equal(40.0, floatConVar.GetPrimitiveValue<float>());

        floatConVar.GetPrimitiveValue<float>() = 50.0f;
        Assert.Equal(50.0f, floatConVar.GetPrimitiveValue<float>());
    }

    [Fact]
    public async Task VectorConVar()
    {
        Server.ExecuteCommand("fog_color -1 -1 -1");
        await WaitOneFrame();

        var vectorConVar = ConVar.Find("fog_color");
        Assert.NotNull(vectorConVar);
        Assert.Equal(-1, vectorConVar.GetNativeValue<Vector>().X);
        Assert.Equal(-1, vectorConVar.GetNativeValue<Vector>().Y);
        Assert.Equal(-1, vectorConVar.GetNativeValue<Vector>().Z);

        var vec = vectorConVar.GetNativeValue<Vector>();
        vec.X = 0;
        vec.Y = 0;
        vec.Z = 0;
        Assert.Equal(0, vectorConVar.GetNativeValue<Vector>().X);
        Assert.Equal(0, vectorConVar.GetNativeValue<Vector>().Y);
        Assert.Equal(0, vectorConVar.GetNativeValue<Vector>().Z);
    }

    [Fact]
    public async Task StringConVar()
    {
        Server.ExecuteCommand("mp_backup_round_file backup");
        await WaitOneFrame();

        var stringConVar = ConVar.Find("mp_backup_round_file");
        Assert.NotNull(stringConVar);
        Assert.Equal("backup", stringConVar.StringValue);

        stringConVar.StringValue = "new_backup";
    }
}