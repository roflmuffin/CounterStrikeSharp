using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Core;

public partial class CEntityInstance : IEquatable<CEntityInstance>
{
    public CEntityInstance(IntPtr pointer) : base(pointer)
    {
    }

    public CEntityInstance(uint rawHandle) : base(rawHandle)
    {
    }
    
    /// <summary>
    /// Checks that the entity handle is valid and the handle points to a valid entity
    /// </summary>
    public bool IsValid => EntityHandle.IsValid && Handle != IntPtr.Zero;

    [Obsolete("Use Index instead", true)]
    public CEntityIndex? EntityIndex => new CEntityIndex(EntityHandle.Index);
    
    public uint Index => EntityHandle.Index;
    
    public string DesignerName => IsValid ? Entity?.DesignerName : null;

    public void Remove() => VirtualFunctions.UTIL_Remove(this.Handle);
    
    public bool Equals(CEntityInstance? other)
    {
        return this.EntityHandle.Equals(other?.EntityHandle);
    }

    public override bool Equals(object? obj)
    {
        return obj is CEntityInstance other && Equals(other);
    }

    public override int GetHashCode()
    {
        return this.Handle.GetHashCode();
    }

    public static bool operator ==(CEntityInstance? left, CEntityInstance? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(CEntityInstance? left, CEntityInstance? right)
    {
        return !Equals(left, right);
    }
    
    /// <summary>
    /// Calls a named input method on an entity.
    /// <example>
    /// <code>
    /// entity.AcceptInput("Break");
    /// </code>
    /// </example>
    /// </summary>
    /// <param name="inputName">Input action name</param>
    /// <param name="activator">Entity which initiated the action, <see langword="null"/> for no entity</param>
    /// <param name="caller">Entity that is sending the event, <see langword="null"/> for no entity</param>
    /// <param name="value">String variant value to send with the event</param>
    /// <param name="outputId">Unknown, defaults to 0</param>
    public void AcceptInput(string inputName, CEntityInstance? activator = null, CEntityInstance? caller = null, string value = "", int outputId = 0)
    {
        VirtualFunctions.AcceptInput(Handle, inputName, activator?.Handle ?? IntPtr.Zero, caller?.Handle ?? IntPtr.Zero, value, 0);
    }
}

public partial class CEntityIdentity
{
    public unsafe CEntityInstance EntityInstance => new(Unsafe.Read<IntPtr>((void*)Handle));
    public unsafe CHandle<CEntityInstance> EntityHandle => new(this.Handle + 0x10);
}