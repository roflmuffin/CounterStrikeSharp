using System.Linq;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Xunit;

namespace NativeTestsPlugin;

public class EntityTests
{
    [Fact]
    public void FindEntityAndAccessSchemaMembers()
    {
        var world = Utilities.FindAllEntitiesByDesignerName<CWorld>("worldent").FirstOrDefault();

        Assert.NotNull(world);
        Assert.Equal("worldent", world.DesignerName);
        Assert.Equal((uint)0, world.Index);

        Assert.Multiple(() =>
        {
            Assert.Equal(0, world.AbsOrigin.X);
            Assert.Equal(0, world.AbsOrigin.Y);
            Assert.Equal(0, world.AbsOrigin.Z);
        });
    }

}