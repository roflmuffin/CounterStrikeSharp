using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Cvars;

public class ConVar<T>
{
    public ushort AccessIndex { get; private set; }

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

    public T Value
    {
        get
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
                    if (type != typeof(float))
                        throw new InvalidOperationException(
                            $"ConVar is a {Type} but you are trying to get a {type} value.");
                    break;
                case ConVarType.Float64:
                    if (type != typeof(double))
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
                    if (type != typeof(string))
                        throw new InvalidOperationException(
                            $"ConVar is a {Type} but you are trying to get a {type} value.");
                    break;
                case ConVarType.Qangle:
                    if (type != typeof(QAngle))
                        throw new InvalidOperationException(
                            $"ConVar is a {Type} but you are trying to get a {type} value.");
                    break;
                case ConVarType.Vector2:
                    if (type != typeof(Vector2D))
                        throw new InvalidOperationException(
                            $"ConVar is a {Type} but you are trying to get a {type} value.");
                    break;
                case ConVarType.Vector3:
                    if (type != typeof(Vector))
                        throw new InvalidOperationException(
                            $"ConVar is a {Type} but you are trying to get a {type} value.");
                    break;
                case ConVarType.Vector4:
                    if (type != typeof(Vector4D))
                        throw new InvalidOperationException(
                            $"ConVar is a {Type} but you are trying to get a {type} value.");
                    break;
                default:
                    throw new InvalidOperationException($"Unknown ConVar type: {Type}");
            }

            return NativeAPI.GetConvarValue<T>(AccessIndex);
        }
        set => NativeAPI.SetConvarValue(AccessIndex, value);
    }

    public string ValueAsString
    {
        get => NativeAPI.GetConvarValueAsString(AccessIndex);
        set => NativeAPI.SetConvarValueAsString(AccessIndex, value);
    }

    public static ConVar<T>? Find(string name)
    {
        var accessIndex = NativeAPI.GetConvarAccessIndexByName(name);
        if (accessIndex == 0) return null;

        return new ConVar<T>(accessIndex);
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
        return $"ConVar [name={Name}, value={Value}, description={Description}, type={Type}, flags={Flags}]";
    }

    public sealed record ConVarCreationOptions
    {
        public required string Name { get; init; }
        public required T DefaultValue { get; init; }
        public string Description { get; init; } = string.Empty;
        public ConVarFlags Flags { get; init; } = ConVarFlags.FCVAR_NONE;
        public T? MinValue { get; init; }
        public T? MaxValue { get; init; }
    }

    public static ConVar<T>? Create(ConVarCreationOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Name))
            throw new ArgumentException("ConVar name cannot be null or whitespace.", nameof(options.Name));

        return Create(options.Name, options.DefaultValue, options.Description, options.Flags, options.MinValue, options.MaxValue);
    }

    public static ConVar<T>? Create(string name, T defaultValue, string description = "", ConVarFlags flags = ConVarFlags.FCVAR_NONE,
        T? minValue = default, T? maxValue = default)
    {
        var type = typeof(T);
        var conVarType = type switch
        {
            _ when type == typeof(bool) => ConVarType.Bool,
            _ when type == typeof(float) => ConVarType.Float32,
            _ when type == typeof(double) => ConVarType.Float64,
            _ when type == typeof(ushort) => ConVarType.UInt16,
            _ when type == typeof(short) => ConVarType.Int16,
            _ when type == typeof(uint) => ConVarType.UInt32,
            _ when type == typeof(int) => ConVarType.Int32,
            _ when type == typeof(long) => ConVarType.Int64,
            _ when type == typeof(ulong) => ConVarType.UInt64,
            _ when type == typeof(string) => ConVarType.String,
            _ when type == typeof(QAngle) => ConVarType.Qangle,
            _ when type == typeof(Vector2D) => ConVarType.Vector2,
            _ when type == typeof(Vector) => ConVarType.Vector3,
            _ when type == typeof(Vector4D) => ConVarType.Vector4,
            _ => throw new InvalidOperationException($"Unsupported type: {type}")
        };

        var accessIndex = NativeAPI.CreateConvar(name, (short)conVarType, description, (UInt64)flags, minValue != null, maxValue != null,
            defaultValue,
            minValue,
            maxValue);
        if (accessIndex == 0) return null;

        return new ConVar<T>(accessIndex);
    }

    public void Delete()
    {
        if (AccessIndex == 0)
            throw new InvalidOperationException("Cannot delete a ConVar that has not been created or found.");

        NativeAPI.DeleteConvar(AccessIndex);
        AccessIndex = 0;
    }
}