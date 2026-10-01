using System.Collections.Generic;
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
    public async Task SingleEntityPostOutput_FiltersOrdersAndUnhooks()
    {
        var entity = Utilities.CreateEntityByName<CBaseModelEntity>("prop_dynamic");
        var other = Utilities.CreateEntityByName<CBaseModelEntity>("prop_dynamic");
        Assert.NotNull(entity);
        Assert.NotNull(other);

        var plugin = NativeTestsPlugin.Instance;
        var calls = new List<string>();
        EntityIO.EntityOutputHandler pre = (_, _, _, _, _, _) =>
        {
            calls.Add("pre");
            return HookResult.Continue;
        };
        EntityIO.EntityOutputHandler post = (_, _, _, _, _, _) =>
        {
            calls.Add("post");
            return HookResult.Continue;
        };

        try
        {
            // Register post first so registration order cannot masquerade as hook mode.
            plugin.HookSingleEntityOutput(entity, "OnUser3", post, HookMode.Post);
            plugin.HookSingleEntityOutput(entity, "OnUser3", pre);

            other.AcceptInput("FireUser3");
            await WaitOneFrame();
            Assert.Empty(calls);

            entity.AcceptInput("FireUser3");
            await WaitOneFrame();
            Assert.Equal(new[] { "pre", "post" }, calls);

            plugin.UnhookSingleEntityOutput(entity, "OnUser3", post);
            calls.Clear();
            entity.AcceptInput("FireUser3");
            await WaitOneFrame();
            Assert.Equal(new[] { "pre" }, calls);
        }
        finally
        {
            plugin.UnhookSingleEntityOutput(entity, "OnUser3", post);
            plugin.UnhookSingleEntityOutput(entity, "OnUser3", pre);
            entity.Remove();
            other.Remove();
        }
    }

    [Fact]
    public async Task SingleEntityPostOutput_DisposeRemovesHook()
    {
        var entity = Utilities.CreateEntityByName<CBaseModelEntity>("prop_dynamic");
        Assert.NotNull(entity);
        var plugin = new OutputTestPlugin();
        var calls = 0;
        EntityIO.EntityOutputHandler handler = (_, _, _, _, _, _) =>
        {
            calls++;
            return HookResult.Continue;
        };

        try
        {
            plugin.HookSingleEntityOutput(entity, "OnUser4", handler, HookMode.Post);
            entity.AcceptInput("FireUser4");
            await WaitOneFrame();
            Assert.Equal(1, calls);

            plugin.Dispose();
            entity.AcceptInput("FireUser4");
            await WaitOneFrame();
            Assert.Equal(1, calls);
        }
        finally
        {
            plugin.Dispose();
            entity.Remove();
        }
    }

    private sealed class OutputTestPlugin : BasePlugin
    {
        public override string ModuleName => "Entity output test";
        public override string ModuleVersion => "1.0.0";
    }

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
}
