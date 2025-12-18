using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Events;
using System.Threading.Tasks;
using Xunit;

namespace NativeTestsPlugin;

public class GameEventTests
{
    [Fact]
    public async Task CanRegisterAndDeregisterEventHandlers()
    {
        int callCount = 0;
        var callback = FunctionReference.Create((EventPlayerConnect @event) =>
        {
            Assert.NotNull(@event);
            Assert.NotEmpty(@event.Name);
            Assert.True(@event.Bot);
            callCount++;
        });

        NativeAPI.IssueServerCommand("bot_quota 0; bot_quota_mode normal");
        await WaitOneFrame();

        NativeAPI.HookEvent("player_connect", callback, true);

        // Test hooking
        NativeAPI.IssueServerCommand("bot_kick");
        NativeAPI.IssueServerCommand("bot_add");
        await WaitOneFrame();

        Assert.Equal(1, callCount);
        NativeAPI.UnhookEvent("player_connect", callback, true);

        // Test unhooking
        NativeAPI.IssueServerCommand("bot_kick");
        NativeAPI.IssueServerCommand("bot_add");
        await WaitOneFrame();
        Assert.Equal(1, callCount);
    }

    [Fact]
    public void CreateEvent_ReturnsValidPointer()
    {
        var eventPtr = NativeAPI.CreateEvent("player_hurt", true);

        try
        {
            Assert.NotEqual(IntPtr.Zero, eventPtr);
        }
        finally
        {
            if (eventPtr != IntPtr.Zero)
            {
                NativeAPI.FreeEvent(eventPtr);
            }
        }
    }

    [Fact]
    public void GetEventName_ReturnsCorrectName()
    {
        var eventPtr = NativeAPI.CreateEvent("player_hurt", true);

        try
        {
            Assert.NotEqual(IntPtr.Zero, eventPtr);

            var eventName = NativeAPI.GetEventName(eventPtr);

            Assert.Equal("player_hurt", eventName);
        }
        finally
        {
            if (eventPtr != IntPtr.Zero)
            {
                NativeAPI.FreeEvent(eventPtr);
            }
        }
    }

    [Fact]
    public void SetAndGetEventBool_Works()
    {
        var eventPtr = NativeAPI.CreateEvent("player_hurt", true);

        try
        {
            Assert.NotEqual(IntPtr.Zero, eventPtr);

            NativeAPI.SetEventBool(eventPtr, "blind", true);
            var result = NativeAPI.GetEventBool(eventPtr, "blind");

            Assert.True(result);
        }
        finally
        {
            if (eventPtr != IntPtr.Zero)
            {
                NativeAPI.FreeEvent(eventPtr);
            }
        }
    }

    [Fact]
    public void SetAndGetEventInt_Works()
    {
        var eventPtr = NativeAPI.CreateEvent("player_hurt", true);

        try
        {
            Assert.NotEqual(IntPtr.Zero, eventPtr);

            var testValue = 42;
            NativeAPI.SetEventInt(eventPtr, "health", testValue);
            var result = NativeAPI.GetEventInt(eventPtr, "health");

            Assert.Equal(testValue, result);
        }
        finally
        {
            if (eventPtr != IntPtr.Zero)
            {
                NativeAPI.FreeEvent(eventPtr);
            }
        }
    }

    [Fact]
    public void SetAndGetEventFloat_Works()
    {
        var eventPtr = NativeAPI.CreateEvent("player_hurt", true);

        try
        {
            Assert.NotEqual(IntPtr.Zero, eventPtr);

            var testValue = 3.14f;
            NativeAPI.SetEventFloat(eventPtr, "dmg_health", testValue);
            var result = NativeAPI.GetEventFloat(eventPtr, "dmg_health");

            Assert.Equal(testValue, result, 3);
        }
        finally
        {
            if (eventPtr != IntPtr.Zero)
            {
                NativeAPI.FreeEvent(eventPtr);
            }
        }
    }

    [Fact]
    public void SetAndGetEventString_Works()
    {
        var eventPtr = NativeAPI.CreateEvent("player_hurt", true);

        try
        {
            Assert.NotEqual(IntPtr.Zero, eventPtr);

            var testValue = "test_weapon";
            NativeAPI.SetEventString(eventPtr, "weapon", testValue);
            var result = NativeAPI.GetEventString(eventPtr, "weapon");

            Assert.Equal(testValue, result);
        }
        finally
        {
            if (eventPtr != IntPtr.Zero)
            {
                NativeAPI.FreeEvent(eventPtr);
            }
        }
    }

    [Fact]
    public void SetAndGetEventUint64_Works()
    {
        var eventPtr = NativeAPI.CreateEvent("player_hurt", true);

        try
        {
            Assert.NotEqual(IntPtr.Zero, eventPtr);

            ulong testValue = 76561198012345678UL;
            NativeAPI.SetEventUint64(eventPtr, "attacker_pawn", testValue);
            var result = NativeAPI.GetEventUint64(eventPtr, "attacker_pawn");

            Assert.Equal(testValue, result);
        }
        finally
        {
            if (eventPtr != IntPtr.Zero)
            {
                NativeAPI.FreeEvent(eventPtr);
            }
        }
    }

    [Fact]
    public void FreeEvent_DoesNotThrow()
    {
        var eventPtr = NativeAPI.CreateEvent("player_hurt", true);

        Assert.NotEqual(IntPtr.Zero, eventPtr);

        var exception = Record.Exception(() => NativeAPI.FreeEvent(eventPtr));

        Assert.Null(exception);
    }

    [Fact]
    public void GameEvent_HighLevelApi_Works()
    {
        var gameEvent = new GameEvent("player_hurt", true);

        try
        {
            Assert.NotNull(gameEvent);
            Assert.NotEqual(IntPtr.Zero, gameEvent.Handle);

            gameEvent.Set("health", 75);
            var health = gameEvent.Get<int>("health");

            Assert.Equal(75, health);
        }
        finally
        {
            gameEvent.Free();
        }
    }
}
