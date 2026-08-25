/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Sounds;

/// <summary>
/// A sound event built and sent as a network message, which lets sound operator parameters such
/// as volume and pitch be chosen per emit.
/// </summary>
/// <example>
/// <code>
/// using var sound = new SoundEvent("Weapon_AK47.Single");
/// sound.SourceEntityIndex = (int)player.Index;
/// sound.SetParam(SoundEvent.Volume, 0.5f);
/// sound.SetParam(SoundEvent.Pitch, 1.5f);
/// sound.EmitToAll();
/// </code>
/// </example>
public class SoundEvent : NativeObject, IDisposable
{
    /// <summary>Volume the sound plays at, where 1 leaves it unchanged.</summary>
    public const string Volume = "public.volume";

    /// <summary>Playback rate, where 1 leaves it unchanged, 2 raises it an octave and 0.5 lowers it one.</summary>
    public const string Pitch = "public.pitch";

    /// <summary>World position the sound plays from.</summary>
    public const string Position = "public.position";

    private bool _disposed;

    /// <param name="name">Name of the sound event, for example <c>Weapon_AK47.Single</c>.</param>
    /// <remarks>
    /// The name is resolved by the client, so the server cannot tell a valid one from a typo. A name
    /// that does not exist plays nothing and reports no error.
    /// </remarks>
    public SoundEvent(string name) : base(NativeAPI.SoundEventCreate(name))
    {
    }

    /// <summary>
    /// Entity the sound is attached to. Leave it at -1 to emit a sound with no source entity.
    /// </summary>
    public int SourceEntityIndex { get; set; } = -1;

    /// <summary>
    /// Sets a sound operator parameter. Setting the same parameter twice keeps the later value.
    /// </summary>
    /// <param name="name">
    /// Any <c>public.*</c> parameter name. <see cref="Volume"/>, <see cref="Pitch"/> and
    /// <see cref="Position"/> are the ones this API was tested against; whether any other parameter
    /// has an effect depends on the operator stack behind the sound.
    /// </param>
    /// <param name="value">Value to send.</param>
    public void SetParam(string name, float value) => NativeAPI.SoundEventSetFloat(Handle, name, value);

    /// <inheritdoc cref="SetParam(string,float)"/>
    public void SetParam(string name, int value) => NativeAPI.SoundEventSetInt(Handle, name, value);

    /// <inheritdoc cref="SetParam(string,float)"/>
    public void SetParam(string name, Vector value) => NativeAPI.SoundEventSetVector(Handle, name, value.Handle);

    /// <summary>
    /// Emits the sound to the given players.
    /// </summary>
    /// <returns>The guid the sound was started with, which <see cref="Stop"/> takes.</returns>
    public int Emit(RecipientFilter recipients) =>
        NativeAPI.SoundEventEmit(Handle, SourceEntityIndex, recipients.GetRecipientMask());

    /// <inheritdoc cref="Emit"/>
    public int EmitToAll()
    {
        var recipients = new RecipientFilter();
        recipients.AddAllPlayers();

        return Emit(recipients);
    }

    /// <inheritdoc cref="Emit"/>
    public int EmitTo(CCSPlayerController player) => Emit(new RecipientFilter(player));

    /// <summary>
    /// Stops a sound started by <see cref="Emit"/>, using the guid it returned.
    /// </summary>
    public static void Stop(int guid, RecipientFilter? recipients = null)
    {
        if (recipients == null)
        {
            recipients = new RecipientFilter();
            recipients.AddAllPlayers();
        }

        NativeAPI.SoundEventStop(guid, recipients.GetRecipientMask());
    }

    private void ReleaseUnmanagedResources()
    {
        if (_disposed) return;

        _disposed = true;

        // The finalizer runs off the game thread, where natives cannot be called.
        var handle = Handle;
        Server.NextFrame(() => NativeAPI.SoundEventRelease(handle));
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~SoundEvent()
    {
        ReleaseUnmanagedResources();
    }
}
