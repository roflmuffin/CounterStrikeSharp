using System;
using System.Linq;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Sounds;
using CounterStrikeSharp.API.Modules.Utils;
using Xunit;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace NativeTestsPlugin;

public class SoundEventTests
{
    private const string TestSound = "Weapon_AK47.Single";

    private static CCSPlayerController FindPlayer()
    {
        var player = Utilities.GetPlayers().FirstOrDefault(p => p is { IsValid: true });
        Assert.NotNull(player);

        return player!;
    }

    [Fact]
    public void Create_ReturnsValidHandle()
    {
        using var sound = new SoundEvent(TestSound);

        Assert.NotEqual(IntPtr.Zero, sound.Handle);
    }

    [Fact]
    public async Task Emit_ReturnsIncreasingGuids()
    {
        await Server.NextFrameAsync(() =>
        {
            using var first = new SoundEvent(TestSound);
            using var second = new SoundEvent(TestSound);

            var firstGuid = first.EmitToAll();
            var secondGuid = second.EmitToAll();

            Assert.NotEqual(0, firstGuid);
            Assert.True(secondGuid > firstGuid, $"expected guid {secondGuid} to follow {firstGuid}");
        });
    }

    [Fact]
    public async Task SetParam_AcceptsEveryValueType()
    {
        await Server.NextFrameAsync(() =>
        {
            using var sound = new SoundEvent(TestSound);
            sound.SourceEntityIndex = (int)FindPlayer().Index;
            sound.SetParam(SoundEvent.Volume, 0.5f);
            sound.SetParam(SoundEvent.Pitch, 1.5f);
            sound.SetParam(SoundEvent.Position, new Vector(1, 2, 3));
            sound.SetParam("public.relevant_player", 1);

            Assert.NotEqual(0, sound.EmitToAll());
        });
    }

    [Fact]
    public async Task SetParam_SameParameterTwice_KeepsWorking()
    {
        await Server.NextFrameAsync(() =>
        {
            using var sound = new SoundEvent(TestSound);
            sound.SetParam(SoundEvent.Volume, 0.1f);
            sound.SetParam(SoundEvent.Volume, 0.9f);

            Assert.NotEqual(0, sound.EmitToAll());
        });
    }

    [Fact]
    public async Task EmitTo_SendsToASinglePlayer()
    {
        await Server.NextFrameAsync(() =>
        {
            using var sound = new SoundEvent(TestSound);

            Assert.NotEqual(0, sound.EmitTo(FindPlayer()));
        });
    }

    [Fact]
    public async Task Stop_EndsASoundThatWasStarted()
    {
        await Server.NextFrameAsync(() =>
        {
            using var sound = new SoundEvent(TestSound);
            var guid = sound.EmitToAll();

            SoundEvent.Stop(guid);
        });
    }

    [Fact]
    public async Task Dispose_IsSafeToCallTwice()
    {
        await Server.NextFrameAsync(() =>
        {
            var sound = new SoundEvent(TestSound);
            sound.EmitToAll();

            sound.Dispose();
            sound.Dispose();
        });
    }
}

/// <summary>
/// Plays a sound to every player on the server, sweeping volume, pitch and then position so that the
/// result can be checked by ear. Run it on its own with <c>css_itest AudibleSoundTest</c>.
/// </summary>
public class AudibleSoundTest
{
    private const string SweepSound = "Weapon_AK47.Single";

    private static void Play(float volume, float pitch, float sideways = 0f)
    {
        foreach (var player in Utilities.GetPlayers().Where(p => p is { IsValid: true, IsBot: false }))
        {
            using var sound = new SoundEvent(SweepSound);
            sound.SetParam(SoundEvent.Volume, volume);
            sound.SetParam(SoundEvent.Pitch, pitch);

            var origin = player.PlayerPawn.Value?.AbsOrigin;
            if (sideways != 0f && origin != null)
            {
                // 0 places the sound in the world rather than at the listener.
                sound.SourceEntityIndex = 0;
                sound.SetParam(SoundEvent.Position, new Vector(origin.X + sideways, origin.Y, origin.Z));
            }

            sound.EmitTo(player);
        }
    }

    private static async Task Step(string what, float volume, float pitch, float sideways = 0f)
    {
        await Server.NextFrameAsync(() =>
        {
            Server.PrintToChatAll($" \x04{what}");
            Play(volume, pitch, sideways);
        });

        await TestUtils.WaitForSeconds(1.2f);
    }

    [Fact]
    public async Task SweepsVolumeThenPitchThenPosition()
    {
        await Step("volume 0.1", 0.1f, 1.0f);
        await Step("volume 0.4", 0.4f, 1.0f);
        await Step("volume 1.0, louder each time", 1.0f, 1.0f);

        await Step("pitch 0.5, lower", 1.0f, 0.5f);
        await Step("pitch 1.0", 1.0f, 1.0f);
        await Step("pitch 2.0, higher", 1.0f, 2.0f);

        await Step("at the listener", 1.0f, 1.0f);
        await Step("600 units to the side", 1.0f, 1.0f, 600f);
        await Step("2500 units to the side, further off", 1.0f, 1.0f, 2500f);
    }
}
