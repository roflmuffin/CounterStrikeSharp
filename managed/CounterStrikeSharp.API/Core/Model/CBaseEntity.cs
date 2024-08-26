using System;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CBaseEntity
{
    /// <exception cref="InvalidOperationException">Entity is not valid</exception>
    /// <exception cref="ArgumentNullException">No valid argument</exception>
    public void Teleport(Vector? position = null, QAngle? angles = null, Vector? velocity = null)
    {
        Guard.IsValidEntity(this);
    
        if (position == null && angles == null && velocity == null)
            throw new ArgumentNullException("No valid argument");
    
        nint _position = position?.Handle ?? 0;
        nint _angles = angles?.Handle ?? 0;
        nint _velocity = velocity?.Handle ?? 0;
    
        VirtualFunction.CreateVoid<IntPtr, IntPtr, IntPtr, IntPtr>(Handle, GameData.GetOffset("CBaseEntity_Teleport"))(Handle, _position, _angles, _velocity);
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
}
