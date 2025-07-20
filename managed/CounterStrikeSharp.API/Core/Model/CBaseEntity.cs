using System;
using System.Numerics;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace CounterStrikeSharp.API.Core;

public partial class CBaseEntity
{
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    /// <exception cref="ArgumentNullException">At least one parameter must be specified</exception>
    public void Teleport(Vector? position = null, QAngle? angles = null, Vector? velocity = null)
    {
        Guard.IsValidEntity(this);

        if (position == null && angles == null && velocity == null)
            throw new ArgumentException("At least one parameter must be specified");

        nint _position = position?.Handle ?? 0;
        nint _angles = angles?.Handle ?? 0;
        nint _velocity = velocity?.Handle ?? 0;
        nint _handle = Handle;

        VirtualFunction.CreateVoid<IntPtr, IntPtr, IntPtr, IntPtr>(_handle, GameData.GetOffset("CBaseEntity_Teleport"))(_handle, _position,
            _angles, _velocity);
    }

    /// <summary>
    /// Teleports the entity to the specified position, angles, and velocity using Vector3 parameters.
    /// This overload is optimized for memory efficiency by directly working with a Vector3 struct.
    /// </summary>
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    /// <exception cref="ArgumentException">At least one parameter must be specified</exception>
    public void Teleport(Vector3? position = null, Vector3? angles = null, Vector3? velocity = null)
    {
        Guard.IsValidEntity(this);

        if (position == null && angles == null && velocity == null)
            throw new ArgumentException("At least one parameter must be specified");

        unsafe
        {
            void* positionPtr = null, anglePtr = null, velocityPtr = null;

            if (position.HasValue)
            {
                var pos = position.Value;
                positionPtr = &pos;
            }

            if (angles.HasValue)
            {
                var ang = angles.Value;
                anglePtr = &ang;
            }

            if (velocity.HasValue)
            {
                var vel = velocity.Value;
                velocityPtr = &vel;
            }

            VirtualFunction.CreateVoid<IntPtr, IntPtr, IntPtr, IntPtr>(Handle, GameData.GetOffset("CBaseEntity_Teleport"))(Handle,
                (nint)positionPtr,
                (nint)anglePtr, (nint)velocityPtr);
        }
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

        return (T)Activator.CreateInstance(typeof(T), Marshal.ReadIntPtr(SubclassID.Handle + 4));
    }

    /// <summary>
    /// Emit a soundevent to all players.
    /// </summary>
    /// <param name="soundEventName">The name of the soundevent to emit.</param>
    /// <param name="recipients">The recipients of the soundevent.</param>
    /// <param name="volume">The volume of the soundevent.</param>
    /// <param name="pitch">The pitch of the soundevent.</param>
    /// <returns>The sound event guid.</returns>
    public uint EmitSound(string soundEventName, RecipientFilter? recipients = null, float volume = 1f, float pitch = 0)
    {
        Guard.IsValidEntity(this);

        if (recipients == null)
        {
            recipients = new RecipientFilter();
            recipients.AddAllPlayers();
        }

        return NativeAPI.EmitSoundFilter(recipients.GetRecipientMask(), this.Index, soundEventName, volume, pitch);
    }
}
