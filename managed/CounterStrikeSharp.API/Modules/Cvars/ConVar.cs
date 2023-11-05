using System;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Cvars;

public class ConVar
{
    public IntPtr Handle { get; }

    public ConVar(IntPtr handle)
    {
        Handle = handle;
    }

    public string Name => Utilities.ReadStringUtf8(Handle);
    public string Description => Utilities.ReadStringUtf8(Handle + 32);

    /// <summary>
    /// The underlying data type of the ConVar.
    /// </summary>
    public unsafe ref ConVarType Type => ref Unsafe.AsRef<ConVarType>((void*)(Handle + 40));
    
    /// <summary>
    /// The ConVar flags as defined by <see cref="ConVarFlags"/>.
    /// </summary>
    public unsafe ref ConVarFlags Flags => ref Unsafe.AsRef<ConVarFlags>((void*)(Handle + 48));

    /// <summary>
    /// Used to access primitive value types, i.e. <see langword="bool"/>, <see langword="float"/>, <see langword="int"/>, etc.
    /// </summary>
    /// <typeparam name="T">The type of value to retrieve</typeparam>
    public unsafe ref T GetPrimitiveValue<T>()
    {
        var type = typeof(T);
        switch (Type)
        {
            case ConVarType.Bool:
                if (type != typeof(bool))
                    throw new InvalidOperationException(
                        $"ConVar is a {Type} but you are trying to get a {type} value.");
                break;
            case ConVarType.Float32:
            case ConVarType.Float64:
                if (type != typeof(float))
                    throw new InvalidOperationException(
                        $"ConVar is a {Type} but you are trying to get a {type} value.");
                break;
            case ConVarType.UInt16:
                if (type != typeof(ushort))
                    throw new InvalidOperationException(
                        $"ConVar is a {Type} but you are trying to get a {type} value.");
                break;
            case ConVarType.Int16:
                if (type != typeof(short))
                    throw new InvalidOperationException(
                        $"ConVar is a {Type} but you are trying to get a {type} value.");
                break;
            case ConVarType.UInt32:
                if (type != typeof(uint))
                    throw new InvalidOperationException(
                        $"ConVar is a {Type} but you are trying to get a {type} value.");
                break;
            case ConVarType.Int32:
                if (type != typeof(int))
                    throw new InvalidOperationException(
                        $"ConVar is a {Type} but you are trying to get a {type} value.");
                break;
            case ConVarType.Int64:
                if (type != typeof(long))
                    throw new InvalidOperationException(
                        $"ConVar is a {Type} but you are trying to get a {type} value.");
                break;
            case ConVarType.UInt64:
                if (type != typeof(ulong))
                    throw new InvalidOperationException(
                        $"ConVar is a {Type} but you are trying to get a {type} value.");
                break;
            case ConVarType.String:
            case ConVarType.Qangle:
            case ConVarType.Vector2:
            case ConVarType.Vector3:
            case ConVarType.Vector4:
            case ConVarType.Color:
                throw new InvalidOperationException("Reference types must be accessed using `GetReferenceValue`");
        }

        return ref Unsafe.AsRef<T>((void*)(Handle + 64));
    }

    public void SetValue<T>(T value)
    {
        GetPrimitiveValue<T>() = value;
    }

    /// <summary>
    /// Used to access reference value types, i.e. Vector, QAngle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetNativeValue<T>() where T : NativeObject
    {
        return (T)Activator.CreateInstance(typeof(T), Handle + 64);
    }

    /// <summary>
    /// String value of the ConVar.
    /// </summary>
    /// <remarks>String is a special exception as we have to marshal the string to UTF8 on the send/receive to unmanaged code.
    /// </remarks>
    public string StringValue
    {
        get => Utilities.ReadStringUtf8(Handle + 64);
        set => NativeAPI.SetConvarStringValue(Handle, value);
    }


    /// <summary>
    /// Shorthand for checking the <see cref="ConVarFlags.FCVAR_NOTIFY"/> flag.
    /// </summary>
    public bool Public
    {
        get => Flags.HasFlag(ConVarFlags.FCVAR_NOTIFY);
        set
        {
            if (value)
            {
                Flags |= ConVarFlags.FCVAR_NOTIFY;
            }
            else
            {
                Flags &= ~ConVarFlags.FCVAR_NOTIFY;
            }
        }
    }

    public override string ToString()
    {
        return $"ConVar [name={Name}, description={Description}, type={Type}, flags={Flags}]";
    }

    /// <summary>
    /// Finds a ConVar by name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static ConVar? Find(string name)
    {
        var ptr = NativeAPI.FindConvar(name);
        if (ptr == IntPtr.Zero) return null;

        return new ConVar(ptr);
    }
}