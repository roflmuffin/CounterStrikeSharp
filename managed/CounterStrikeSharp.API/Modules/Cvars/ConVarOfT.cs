using System.Diagnostics.CodeAnalysis;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Cvars;

public class ConVarBase : IEquatable<ConVarBase>
{
    internal const ushort InvalidAccessIndex = ushort.MaxValue;

    public ushort AccessIndex { get; protected set; }
    internal bool Owned { get; set; }

    public ConVarBase(ushort accessIndex)
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

    public string ValueAsString
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

    public void Delete()
    {
        if (AccessIndex == InvalidAccessIndex)
            throw new InvalidOperationException("Cannot delete a ConVar that has not been created or found.");

        NativeAPI.DeleteConvar(AccessIndex);
        AccessIndex = InvalidAccessIndex;
    }

    public bool IsValid => AccessIndex != InvalidAccessIndex;

    public ConVar<T> As<T>()
    {
        return new ConVar<T>(AccessIndex);
    }

    public bool Equals(ConVarBase? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return AccessIndex == other.AccessIndex;
    }

    public override bool Equals(object? obj)
    {
        return obj is ConVarBase other && Equals(other);
    }

    public override int GetHashCode()
    {
        return AccessIndex.GetHashCode();
    }
}

public class ConVar<T> : ConVarBase, IEquatable<ConVar<T>>
{
    public ConVar(ushort accessIndex) : base(accessIndex)
    {
    }

    public ConVar(string name, string description, T defaultValue, ConVarFlags flags = ConVarFlags.FCVAR_NONE)
        : this(new ConVarCreationOptions<T>
        {
            Name = name,
            DefaultValue = defaultValue,
            Description = description,
            Flags = flags
        })
    {
    }

    private static ConVarType GetValueType()
    {
        var type = typeof(T);
        return type switch
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
    }

    public ConVar(ConVarCreationOptions<T> options) : base(0)
    {
        AccessIndex = NativeAPI.CreateConvar(options.Name, (short)GetValueType(), options.Description, (UInt64)options.Flags,
            options.HasMinValue, options.HasMaxValue,
            options.DefaultValue,
            options.HasMinValue ? options.MinValue : options.DefaultValue,
            options.HasMaxValue ? options.MaxValue : options.DefaultValue);

        if (AccessIndex == InvalidAccessIndex)
        {
            throw new InvalidOperationException($"Failed to create ConVar '{options.Name}' with type '{typeof(T)}'.");
        }

        Owned = true;
    }

    /// <summary>
    /// Gets or updates the existing lower bound for a numeric, vector, or angle ConVar.
    /// </summary>
    /// <remarks>
    /// Throws if no lower bound exists or the new minimum exceeds the maximum.
    /// Changes apply to subsequent value assignments; the current value is not reclamped.
    /// For native objects, assign a new value rather than modifying the returned object's components.
    /// </remarks>
    public T MinValue
    {
        get => NativeAPI.GetConvarBound<T>(AccessIndex, true, (short)GetValueType());
        set => NativeAPI.SetConvarBound(AccessIndex, true, (short)GetValueType(), value);
    }

    /// <summary>
    /// Gets or updates the existing upper bound for a numeric, vector, or angle ConVar.
    /// </summary>
    /// <remarks>
    /// Throws if no upper bound exists or the new maximum is below the minimum.
    /// Changes apply to subsequent value assignments; the current value is not reclamped.
    /// For native objects, assign a new value rather than modifying the returned object's components.
    /// </remarks>
    public T MaxValue
    {
        get => NativeAPI.GetConvarBound<T>(AccessIndex, false, (short)GetValueType());
        set => NativeAPI.SetConvarBound(AccessIndex, false, (short)GetValueType(), value);
    }

    /// <summary>Tries to read the lower bound, returning false and default(T) when it is not set.</summary>
    /// <remarks>Invalid ConVars and mismatched value types still throw.</remarks>
    public bool TryGetMinValue([MaybeNullWhen(false)] out T value) => TryGetBound(true, out value);

    /// <summary>Tries to read the upper bound, returning false and default(T) when it is not set.</summary>
    /// <remarks>Invalid ConVars and mismatched value types still throw.</remarks>
    public bool TryGetMaxValue([MaybeNullWhen(false)] out T value) => TryGetBound(false, out value);

    private bool TryGetBound(bool minimum, [MaybeNullWhen(false)] out T value)
    {
        var type = (short)GetValueType();
        if (!NativeAPI.HasConvarBound(AccessIndex, minimum, type))
        {
            value = default;
            return false;
        }

        value = NativeAPI.GetConvarBound<T>(AccessIndex, minimum, type);
        return true;
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
        set
        {
            var expectedType = GetValueType();
            if (Type != expectedType)
                throw new InvalidOperationException(
                    $"ConVar is a {Type} but you are trying to set a {typeof(T)} value.");

            NativeAPI.SetConvarValue(AccessIndex, value);
        }
    }

    public static ConVar<T>? Find(string name)
    {
        var accessIndex = NativeAPI.GetConvarAccessIndexByName(name);
        if (accessIndex == 0) return null;

        return new ConVar<T>(accessIndex);
    }

    public bool Equals(ConVar<T>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return AccessIndex == other.AccessIndex;
    }

    public override bool Equals(object? obj)
    {
        return obj is ConVarBase other && Equals(other);
    }

    public override int GetHashCode()
    {
        return AccessIndex.GetHashCode();
    }

    public override string ToString()
    {
        return $"ConVar [name={Name}, value={Value}, description={Description}, type={Type}, flags={Flags}]";
    }
}

public sealed record ConVarCreationOptions<T>
{
    public required string Name { get; init; }
    public required T DefaultValue { get; init; }
    public string Description { get; init; } = string.Empty;
    public ConVarFlags Flags { get; init; } = ConVarFlags.FCVAR_NONE;
    private T? _minValue;
    private T? _maxValue;

    internal bool HasMinValue { get; private set; }
    internal bool HasMaxValue { get; private set; }

    /// <summary>Optional lower bound. Omit this property for no minimum; zero and false are valid bounds.</summary>
    public T? MinValue
    {
        get => _minValue;
        init
        {
            _minValue = value;
            HasMinValue = value is not null;
        }
    }

    /// <summary>Optional upper bound. Omit this property for no maximum; zero and false are valid bounds.</summary>
    public T? MaxValue
    {
        get => _maxValue;
        init
        {
            _maxValue = value;
            HasMaxValue = value is not null;
        }
    }
}