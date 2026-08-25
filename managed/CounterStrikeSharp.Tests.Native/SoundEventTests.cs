using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Sounds;
using CounterStrikeSharp.API.Modules.UserMessages;
using Xunit;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace NativeTestsPlugin;

public class SoundEventTests
{
    // gameevents.proto: GE_SosStartSoundEvent
    private const int SosStartSoundEvent = 208;

    private const string TestSound = "Weapon_AK47.Single";

    // MurmurHash2 of the parameter name under the Source 2 string token seed. Hardcoded so that a
    // change to the way parameters are packed shows up as a failing test rather than silence.
    private const uint VolumeHash = 0xBD6054E9;
    private const uint PitchHash = 0x929A57A4;
    private const uint PositionHash = 0x5A7CCE4D;

    private const byte TypeInt = 0x02;
    private const byte TypeFloat = 0x08;
    private const byte TypeVector = 0x0A;

    private readonly record struct Parameter(uint Hash, byte Type, byte[] Value);

    private readonly record struct CapturedSound(uint Guid, uint Hash, byte[]? Packed);

    /// <summary>
    /// Runs <paramref name="emit"/> with a hook on the sound event message and returns what the
    /// hook saw. Values are read inside the hook because the message only lives for that call, and
    /// the game may well emit its own sounds in the same frame, so callers pick theirs out by guid.
    /// </summary>
    private static List<CapturedSound> Capture(Action emit)
    {
        var captured = new List<CapturedSound>();

        HookResult Handler(UserMessage message)
        {
            captured.Add(new CapturedSound(message.ReadUInt("soundevent_guid"), message.ReadUInt("soundevent_hash"),
                message.HasField("packed_params") ? message.ReadBytes("packed_params") : null));

            return HookResult.Continue;
        }

        NativeTestsPlugin.Instance.HookUserMessage(SosStartSoundEvent, Handler);

        try
        {
            emit();
        }
        finally
        {
            NativeTestsPlugin.Instance.UnhookUserMessage(SosStartSoundEvent, Handler);
        }

        return captured;
    }

    /// <summary>
    /// Emits one sound and returns the message it produced.
    /// </summary>
    private static CapturedSound EmitAndCapture(Action<SoundEvent> setup)
    {
        var guid = 0;

        var captured = Capture(() =>
        {
            using var sound = new SoundEvent(TestSound);
            setup(sound);
            guid = sound.EmitToAll();
        });

        Assert.NotEqual(0, guid);

        return Assert.Single(captured, sound => sound.Guid == (uint)guid);
    }

    private static List<Parameter> Unpack(byte[] packed)
    {
        var parameters = new List<Parameter>();

        for (var at = 0; at + 7 <= packed.Length;)
        {
            var hash = BitConverter.ToUInt32(packed, at);
            var type = packed[at + 4];
            var size = BitConverter.ToUInt16(packed, at + 5);
            at += 7;

            Assert.True(at + size <= packed.Length, "parameter runs past the end of the blob");
            parameters.Add(new Parameter(hash, type, packed[at..(at + size)]));
            at += size;
        }

        return parameters;
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
    public async Task Emit_HashesTheSoundNameTheWayTheGameDoes()
    {
        // A sound the bots will not set off by themselves, so anything the game emits under this
        // hash came from the EmitSound call below.
        const string quietSound = "UIPanorama.popup_accept_match_beep";

        var seen = new List<CapturedSound>();
        var ourGuid = 0u;

        HookResult Handler(UserMessage message)
        {
            seen.Add(new CapturedSound(message.ReadUInt("soundevent_guid"), message.ReadUInt("soundevent_hash"),
                message.HasField("packed_params") ? message.ReadBytes("packed_params") : null));

            return HookResult.Continue;
        }

        await Server.NextFrameAsync(() =>
        {
            var player = Utilities.GetPlayers().FirstOrDefault(p => p is { IsValid: true });
            Assert.NotNull(player);

            NativeTestsPlugin.Instance.HookUserMessage(SosStartSoundEvent, Handler);

            using (var sound = new SoundEvent(quietSound))
            {
                sound.SourceEntityIndex = (int)player!.Index;
                ourGuid = (uint)sound.EmitToAll();
            }

            // The game does not post its own message inside this call, hence the wait below.
            player!.EmitSound(quietSound);
        });

        await TestUtils.WaitForSeconds(0.6f);
        await Server.NextFrameAsync(() => NativeTestsPlugin.Instance.UnhookUserMessage(SosStartSoundEvent, Handler));

        var ours = Assert.Single(seen, sound => sound.Guid == ourGuid);
        var theirs = seen.Where(sound => sound.Guid != ourGuid).ToList();

        Assert.NotEmpty(theirs);
        Assert.Contains(theirs, sound => sound.Hash == ours.Hash);
    }

    [Fact]
    public async Task SetParam_PacksEveryParameterIntoTheMessage()
    {
        await Server.NextFrameAsync(() =>
        {
            var captured = EmitAndCapture(sound =>
            {
                sound.SetParam(SoundEvent.Volume, 0.25f);
                sound.SetParam(SoundEvent.Pitch, 1.5f);
                sound.SetParam(SoundEvent.Position, new Vector(1, 2, 3));
            });

            Assert.NotNull(captured.Packed);
            var parameters = Unpack(captured.Packed!);
            Assert.Equal(3, parameters.Count);

            var volume = parameters.Single(p => p.Hash == VolumeHash);
            Assert.Equal(TypeFloat, volume.Type);
            Assert.Equal(0.25f, BitConverter.ToSingle(volume.Value));

            var pitch = parameters.Single(p => p.Hash == PitchHash);
            Assert.Equal(TypeFloat, pitch.Type);
            Assert.Equal(1.5f, BitConverter.ToSingle(pitch.Value));

            var position = parameters.Single(p => p.Hash == PositionHash);
            Assert.Equal(TypeVector, position.Type);
            Assert.Equal(12, position.Value.Length);
            Assert.Equal(1f, BitConverter.ToSingle(position.Value, 0));
            Assert.Equal(2f, BitConverter.ToSingle(position.Value, 4));
            Assert.Equal(3f, BitConverter.ToSingle(position.Value, 8));
        });
    }

    [Fact]
    public async Task SetParam_SameParameterTwice_SendsOnlyTheLastValue()
    {
        await Server.NextFrameAsync(() =>
        {
            var captured = EmitAndCapture(sound =>
            {
                sound.SetParam(SoundEvent.Volume, 0.1f);
                sound.SetParam(SoundEvent.Volume, 0.9f);
            });

            Assert.NotNull(captured.Packed);
            var volume = Assert.Single(Unpack(captured.Packed!), p => p.Hash == VolumeHash);
            Assert.Equal(0.9f, BitConverter.ToSingle(volume.Value));
        });
    }

    [Fact]
    public async Task SetParam_Int_UsesTheIntegerType()
    {
        await Server.NextFrameAsync(() =>
        {
            var captured = EmitAndCapture(sound => sound.SetParam("public.relevant_player", 7));

            Assert.NotNull(captured.Packed);
            var parameter = Assert.Single(Unpack(captured.Packed!));
            Assert.Equal(TypeInt, parameter.Type);
            Assert.Equal(7, BitConverter.ToInt32(parameter.Value));
        });
    }

    [Fact]
    public async Task Emit_WithNoParameters_SendsNoBlob()
    {
        await Server.NextFrameAsync(() => { Assert.Null(EmitAndCapture(_ => { }).Packed); });
    }
}
