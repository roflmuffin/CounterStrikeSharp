using System;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Core;
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
    public ConVarFlags Flags => (ConVarFlags)NativeAPI.GetConvarFlags(AccessIndex);

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
                case ConVarType.Color:
                    if (type != typeof(Vector))
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

    public static ConVar<T>? Find(string name)
    {
        var accessIndex = NativeAPI.GetConvarAccessIndexByName(name);
        if (accessIndex == 0) return null;

        return new ConVar<T>(accessIndex);
    }
}