using System;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CBaseEntity
{
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void Teleport(Vector position, QAngle angles, Vector velocity)
    {
        Guard.IsValidEntity(this);

        VirtualFunction.CreateVoid<IntPtr, IntPtr, IntPtr, IntPtr>(Handle, GameData.GetOffset("CBaseEntity_Teleport"))(
            Handle, position.Handle, angles.Handle, velocity.Handle);
    }

    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public void DispatchSpawn()
    {
        Guard.IsValidEntity(this);

        VirtualFunctions.CBaseEntity_DispatchSpawn(Handle, IntPtr.Zero);
    }

    /// <summary>
    /// Shorthand for accessing an entity's CBodyComponent?.SceneNode?.AbsOrigin;
    /// </summary>
    public Vector? AbsOrigin => CBodyComponent?.SceneNode?.AbsOrigin;

    /// <summary>
    /// Shorthand for accessing an entity's CBodyComponent?.SceneNode?.AbsRotation;
    /// </summary>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    public QAngle? AbsRotation => CBodyComponent?.SceneNode?.AbsRotation;

    public T? GetVData<T>() where T : CEntitySubclassVDataBase
    {
        Guard.IsValidEntity(this);

        return (T) Activator.CreateInstance(typeof(T), Marshal.ReadIntPtr(SubclassID.Handle + 4));
    }

    /// <summary>
    /// Plays a sound on an entity.
    /// WARNING: Parameters might not work on the sound event other than soundName and filter
    /// </summary>
    /// <param name="soundName">Sound event name</param>
    /// <param name="soundLevel">A modifier for the distance this sound will reach.</param>
    /// <param name="pitch">The pitch applied to the sound. 100 means the pitch is not changed.</param>
    /// <param name="volume">The volume of the sound.</param>
    /// <param name="channel">The sound channel.</param>
    /// <param name="soundFlags">The flags of the sound.</param>
    /// <param name="filter">If set, the sound will only be networked to the clients in the filter.</param>
    /// <exception cref="InvalidOperationException">Invalid entity</exception>
    public void EmitSound(string soundName, soundlevel_t soundLevel = soundlevel_t.SNDLVL_NORM, int pitch = 100, float volume = 1f, int channel = 0, int soundFlags = 0, CRecipientFilter? filter = null)
    {
        Guard.IsValidEntity(this);

        if (filter != null)
            NativeAPI.EmitSoundFilter(this.Index, soundName, soundLevel, pitch, volume, channel, soundFlags, true, filter.GetRecipientCount(), filter.GetRecipientsArray());
        else
            NativeAPI.EmitSoundFilter(this.Index, soundName, soundLevel, pitch, volume, channel, soundFlags, false, 0, Array.Empty<object>());
    }
}