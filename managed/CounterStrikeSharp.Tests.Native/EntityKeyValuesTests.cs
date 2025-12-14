using System.Drawing;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using Xunit;

public class EntityKeyValuesTests
{
    [Fact]
    public async Task CanCreateAndSetValues()
    {
        using var kv = new CEntityKeyValues();
        kv.SetString("name", "test_entity");
        kv.SetInt("health", 100);
        kv.SetFloat("speed", 5.5f);
        kv.SetDouble("double", Double.MaxValue);
        kv.SetVector("position", new Vector(1.0f, 2.0f, 3.0f));
        kv.SetAngle("view_angle", new QAngle(90.0f, 45.0f, 12.5f));
        kv.SetColor("color", Color.FromArgb(255, 128, 64, 32));
        kv.SetEHandle("owner", new CEntityHandle((uint)12345));

        Assert.Equal("test_entity", kv.GetString("name"));
        Assert.Equal(100, kv.GetInt("health"));
        Assert.Equal(5.5f, kv.GetFloat("speed"));
        Assert.Equal(Double.MaxValue, kv.GetDouble("double"));
        var position = kv.GetVector("position");
        Assert.Equal(position.X, 1.0f);
        Assert.Equal(position.Y, 2.0f);
        Assert.Equal(position.Z, 3.0f);

        var angle = kv.GetAngle("view_angle");
        Assert.Equal(angle.X, 90.0f);
        Assert.Equal(angle.Y, 45.0f);
        Assert.Equal(angle.Z, 12.5f);

        Assert.Equal(Color.FromArgb(255, 128, 64, 32), kv.GetColor("color"));
        Assert.Equal((uint)12345, kv.GetEHandle("owner").Raw);
    }

    [Fact]
    public async Task CanSpawnEntityWithKeyValues()
    {
        var light = Utilities.CreateEntityByName<CBarnLight>("light_barn")!;
        using var kv = new CEntityKeyValues();

        kv.SetColor("color", Color.BlanchedAlmond);
        kv.SetFloat("brightness", 750.0f);
        kv.SetBool("enabled", true);
        kv.SetVector("size_params", new Vector(60.0f, 120.0f, 0.05f));
        kv.SetInt("directlight", 3);
        light.DispatchSpawn(kv);

        Assert.Equal(750.0f, light.Brightness);
        Assert.Equal(Color.BlanchedAlmond.ToArgb(), light.Color.ToArgb());
        Assert.True(light.Enabled);
        Assert.Equal(60.0f, light.SizeParams.X);
        Assert.Equal(120.0f, light.SizeParams.Y);
        Assert.Equal(0.05f, light.SizeParams.Z);
        Assert.Equal(3, light.DirectLight);

        light.Remove();
    }
}
