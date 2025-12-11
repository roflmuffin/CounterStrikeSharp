using System.Runtime.CompilerServices;

namespace CounterStrikeSharp.API.Modules.Cvars;

public class ConVar
{
    public ushort AccessIndex { get; protected set; }

    public ConVar(ushort accessIndex)
    {
        AccessIndex = accessIndex;
    }

    public string Name => NativeAPI.GetConvarName(AccessIndex);
    public string Description => NativeAPI.GetConvarHelpText(AccessIndex);

    /// <summary>
    /// The underlying data type of the ConVar.
    /// </summary>
    public ConVarType Type => (ConVarType)NativeAPI.GetConvarType(AccessIndex);

    /// <summary>
    /// The ConVar flags as defined by <see cref="ConVarFlags"/>.
    /// </summary>
    public ConVarFlags Flags
    {
        get => (ConVarFlags)NativeAPI.GetConvarFlags(AccessIndex);
        set => NativeAPI.SetConvarFlags(AccessIndex, (ulong)value);
    }

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

        var address = NativeAPI.GetConvarValueAddress(AccessIndex);
        if (address == IntPtr.Zero)
        {
            throw new InvalidOperationException($"ConVar {Name} is not initialized or does not have a value.");
        }

        return ref Unsafe.AsRef<T>((void*)address);
    }

    public void SetValue<T>(T value)
    {
        NativeAPI.SetConvarValue(AccessIndex, value);
    }

    /// <summary>
    /// Used to access reference value types, i.e. Vector, QAngle
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetNativeValue<T>() where T : NativeObject
    {
        return NativeAPI.GetConvarValue<T>(AccessIndex);
    }

    /// <summary>
    /// String value of the ConVar.
    /// </summary>
    /// <remarks>String is a special exception as we have to marshal the string to UTF8 on the send/receive to unmanaged code.
    /// </remarks>
    public string StringValue
    {
        get => NativeAPI.GetConvarValueAsString(AccessIndex);
        set => NativeAPI.SetConvarValueAsString(AccessIndex, value);
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
        var accessIndex = NativeAPI.GetConvarAccessIndexByName(name);
        if (accessIndex == 0) return null;

        return new ConVar(accessIndex);
    }
}