using System.Drawing;
using System.Linq;
using System.Numerics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Xunit;

namespace NativeTestsPlugin;

public class SchemaPropertyTests : IDisposable
{
    private CBaseModelEntity entity;
    private CCSPlayerController player;

    public SchemaPropertyTests()
    {
        this.entity = Utilities.CreateEntityByName<CBaseModelEntity>("prop_dynamic")!;
        this.player = Utilities.GetPlayers().First();
    }

    [Fact]
    public void SchemaProperty_Utf8String()
    {
        this.entity.LastDamageSourceName = "TestSource";
        Assert.Equal("TestSource", this.entity.LastDamageSourceName);
    }

    [Fact]
    public void SchemaProperty_String()
    {
        this.player.PlayerName = "TestName";
        Assert.Equal("TestName", this.player.PlayerName);
    }

    [Fact]
    public void SchemaProperty_Int()
    {
        this.entity.Health = 75;
        Assert.Equal(75, this.entity.Health);
    }

    [Fact]
    public void SchemaProperty_Float()
    {
        this.entity.Friction = 0.8f;
        Assert.Equal(0.8f, this.entity.Friction);
    }

    [Fact]
    public void SchemaProperty_Bool()
    {
        this.entity.AllowFadeInView = true;
        Assert.True(this.entity.AllowFadeInView);
    }

    [Fact]
    public void SchemaProperty_Vector()
    {
        this.entity.DecalPosition.X = 100;
        this.entity.DecalPosition.Y = 200;
        this.entity.DecalPosition.Z = 300;
        Assert.Equal((Vector3)this.entity.DecalPosition, new Vector3(100, 200, 300));
    }

    [Fact]
    public void SchemaProperty_Enum()
    {
        this.entity.RenderMode = RenderMode_t.kRenderTransAlphaAdd;
        Assert.Equal(RenderMode_t.kRenderTransAlphaAdd, this.entity.RenderMode);
    }

    [Fact]
    public void SchemaProperty_ByteArray()
    {
        for (int i = 0; i < this.entity.DisabledHitGroups.Length; i++)
        {
            this.entity.DisabledHitGroups[i] = (uint)i;
        }

        for (int i = 0; i < this.entity.DisabledHitGroups.Length; i++)
        {
            Assert.Equal((uint)i, this.entity.DisabledHitGroups[i]);
        }
    }

    [Fact]
    public void SchemaProperty_Handle()
    {
        this.entity.OwnerEntity.Raw = this.player.EntityHandle.Raw;
        Assert.True(this.entity.OwnerEntity.IsValid);
        Assert.Equal(this.player.EntityHandle.Raw, this.entity.OwnerEntity.Raw);
    }

    [Fact]
    public void SchemaProperty_CustomMarshalledType_Color()
    {
        var color = Color.FromArgb(255, 128, 64, 32);
        this.entity.Render = color;
        Assert.Equal(color, this.entity.Render);
    }

    public void Dispose()
    {
        this.entity.Remove();
    }
}
