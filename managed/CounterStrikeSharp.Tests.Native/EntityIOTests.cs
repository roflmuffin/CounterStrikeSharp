using System.Linq;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
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
        var isHooked = false;

        try
        {
            NativeAPI.HookEntityOutput("prop_dynamic", "OnUser1", callback, HookMode.Pre);
            isHooked = true;
            NativeAPI.AcceptInput(entity.Handle, "FireUser1", IntPtr.Zero, IntPtr.Zero, "", 0);
            await WaitOneFrame();

            Assert.Single(mock.Invocations);

            // Test unhook
            NativeAPI.UnhookEntityOutput("prop_dynamic", "OnUser1", callback, HookMode.Pre);
            isHooked = false;
            NativeAPI.AcceptInput(entity.Handle, "FireUser1", IntPtr.Zero, IntPtr.Zero, "", 0);

            await WaitOneFrame();
            Assert.Single(mock.Invocations);
        }
        finally
        {
            if (isHooked)
            {
                NativeAPI.UnhookEntityOutput("prop_dynamic", "OnUser1", callback, HookMode.Pre);
            }

            FunctionReference.Remove(callback.Identifier);
            entity.Remove();
        }
    }

    [Fact]
    public async Task AddEntityIOEvent_WithLegacyOutputId_FiresOutput()
    {
        var entity = Utilities.CreateEntityByName<CBaseModelEntity>("prop_dynamic");

        Assert.NotNull(entity);

        var mock = new Mock<Action>();
        var callback = FunctionReference.Create(mock.Object);
        var isHooked = false;

        try
        {
            NativeAPI.HookEntityOutput("prop_dynamic", "OnUser2", callback, HookMode.Pre);
            isHooked = true;
            entity.AddEntityIOEvent("FireUser2", outputId: 12345);
            await WaitOneFrame();

            Assert.Single(mock.Invocations);
        }
        finally
        {
            if (isHooked)
            {
                NativeAPI.UnhookEntityOutput("prop_dynamic", "OnUser2", callback, HookMode.Pre);
            }

            FunctionReference.Remove(callback.Identifier);
            entity.Remove();
        }
    }

    // Accept-input does not depend on entity output hooks: it still works when they are disabled (ADR-0002).
    [Fact]
    public async Task AcceptInput_Kill_RemovesEntity()
    {
        var entity = Utilities.CreateEntityByName<CBaseModelEntity>("prop_dynamic");

        Assert.NotNull(entity);

        entity.AcceptInput("Kill");
        await WaitOneFrame();

        Assert.False(entity.IsValid);
    }

    // A plugin's entity output hook registers either way; it fires only when entity output hooks are available.
    [Fact]
    public async Task HookEntityOutput_FiresOnlyWhenOutputHooksAreAvailable()
    {
        var entity = Utilities.CreateEntityByName<CBaseModelEntity>("prop_dynamic");

        Assert.NotNull(entity);

        var fired = 0;
        EntityIO.EntityOutputHandler handler = (_, _, _, _, _, _) =>
        {
            fired++;
            return HookResult.Continue;
        };

        NativeTestsPlugin.Instance.HookEntityOutput("prop_dynamic", "OnUser3", handler);
        try
        {
            entity.AcceptInput("FireUser3");
            await WaitOneFrame();

            Assert.Equal(EngineFeatures.IsAvailable(EngineFeature.EntityOutputHooks) ? 1 : 0, fired);
        }
        finally
        {
            NativeTestsPlugin.Instance.UnhookEntityOutput("prop_dynamic", "OnUser3", handler);
            entity.Remove();
        }
    }
}
