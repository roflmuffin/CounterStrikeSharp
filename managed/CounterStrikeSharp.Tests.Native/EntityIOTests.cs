using System.Linq;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Moq;
using Xunit;

namespace NativeTestsPlugin;

public class EntityIOTests
{
    [Fact]
    public void GetDesignerName_ReturnsCorrectName()
    {
        var world = Utilities.FindAllEntitiesByDesignerName<CWorld>("worldent").FirstOrDefault();

        Assert.NotNull(world);
        Assert.Equal("worldent", world.DesignerName);
    }

    [Fact]
    public void GetEntityFromIndex_ReturnsValidPointer()
    {
        var world = Utilities.GetEntityFromIndex<CWorld>(0);
        Assert.NotNull(world);

        Assert.NotNull(world);
        Assert.Equal("worldent", world.DesignerName);
    }

    [Fact]
    public void GetEntityFromHandle_Works()
    {
        var world = Utilities.FindAllEntitiesByDesignerName<CWorld>("worldent").FirstOrDefault();

        Assert.NotNull(world);

        var worldFromHandle = world.EntityHandle.Get().As<CWorld>();

        Assert.Equal(world.Handle, worldFromHandle.Handle);
    }

    [Fact]
    public async Task AcceptInput_DoesNotThrow()
    {
        var entity = Utilities.CreateEntityByName<CBaseModelEntity>("prop_dynamic");

        Assert.NotNull(entity);

        var mock = new Mock<Action>();
        var callback = FunctionReference.Create(mock.Object);

        try
        {
            NativeAPI.HookEntityOutput("prop_dynamic", "OnUser1", callback, HookMode.Pre);
            NativeAPI.AcceptInput(entity.Handle, "FireUser1", IntPtr.Zero, IntPtr.Zero, "", 0);
            await WaitOneFrame();

            Assert.Single(mock.Invocations);

            // Test unhook
            NativeAPI.UnhookEntityOutput("prop_dynamic", "OnUser1", callback, HookMode.Pre);
            NativeAPI.AcceptInput(entity.Handle, "FireUser1", IntPtr.Zero, IntPtr.Zero, "", 0);

            await WaitOneFrame();
            Assert.Single(mock.Invocations);
        }
        finally
        {
            entity.Remove();
        }
    }
}
