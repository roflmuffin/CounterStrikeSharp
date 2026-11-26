using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Logging;
using FastGenericNew;
using Microsoft.Extensions.Logging;

namespace CounterStrikeSharp.API.Modules.Memory;

public class Schema
{
    record SchemaKey(string ClassName, string PropertyName);

    // Offset per field, or a negative miss from the native lookup (cached too, so a miss is logged once).
    private static readonly ConcurrentDictionary<SchemaKey, int> _schemaOffsets = new();
    private const int SchemaClassMissing = -1;

    // Seams for unit tests (NativeAPI is not callable outside the server).
    internal static Func<string, string, int> LookupField = NativeAPI.LookupSchemaField;

    private static ILogger? _logger;

    // Setting null restores the core logger.
    [System.Diagnostics.CodeAnalysis.AllowNull]
    internal static ILogger Logger
    {
        get => _logger ??= CoreLogging.Factory.CreateLogger("Schema");
        set => _logger = value;
    }

    internal static void ClearCache() => _schemaOffsets.Clear();

    private static HashSet<string> _cs2BadList = new HashSet<string>()
    {
        "m_bIsValveDS",
        "m_bIsQuestEligible",
        // "m_iItemDefinitionIndex", // as of 2023.11.11 this is currently not blocked
        "m_iEntityLevel",
        "m_iItemIDHigh",
        "m_iItemIDLow",
        "m_iAccountID",
        "m_iEntityQuality",

        "m_bInitialized",
        "m_szCustomName",
        "m_iAttributeDefinitionIndex",
        "m_iRawValue32",
        "m_iRawInitialValue32",
        "m_flValue", // MNetworkAlias "m_iRawValue32"
        "m_flInitialValue", // MNetworkAlias "m_iRawInitialValue32"
        "m_bSetBonus",
        "m_nRefundableCurrency",

        "m_OriginalOwnerXuidLow",
        "m_OriginalOwnerXuidHigh",

        "m_nFallbackPaintKit",
        "m_nFallbackSeed",
        "m_flFallbackWear",
        "m_nFallbackStatTrak",

        "m_iCompetitiveWins",
        "m_iCompetitiveRanking",
        "m_iCompetitiveRankType",
        "m_iCompetitiveRankingPredicted_Win",
        "m_iCompetitiveRankingPredicted_Loss",
        "m_iCompetitiveRankingPredicted_Tie",

        "m_nActiveCoinRank",
        "m_nMusicID",
    };

    public static int GetClassSize(string className) => NativeAPI.GetSchemaClassSize(className);

    /// <summary>
    /// The offset of a schema field in its class. Unlike the accessors below, this throws for a missing field in every
    /// <see cref="CoreConfig.SchemaMissMode"/>: the caller would add the offset to a pointer itself.
    /// </summary>
    /// <exception cref="SchemaFieldNotFoundException">The class or field is not in the schema.</exception>
    public static short GetSchemaOffset(string className, string propertyName)
    {
        ThrowIfGuidelinesBlock(className, propertyName);

        var offset = LookupOffset(className, propertyName);
        if (offset < 0) throw new SchemaFieldNotFoundException(className, propertyName, offset == SchemaClassMissing);

        return (short)offset;
    }

    /// <summary>The offset, or false without logging if the field is missing (for fields only some classes have).</summary>
    internal static bool TryGetSchemaOffset(string className, string propertyName, out short offset)
    {
        var found = LookupOffset(className, propertyName, logMiss: false);
        offset = found < 0 ? (short)0 : (short)found;
        return found >= 0;
    }

    private static void ThrowIfGuidelinesBlock(string className, string propertyName)
    {
        if (CoreConfig.FollowCS2ServerGuidelines && _cs2BadList.Contains(propertyName))
        {
            throw new Exception($"Cannot set or get '{className}::{propertyName}' with \"FollowCS2ServerGuidelines\" option enabled.");
        }
    }

    // The cached native lookup: the offset, or a negative miss. The first miss of a field is logged once (unless a
    // caller only probes), in the wording of the current SchemaMissMode.
    private static int LookupOffset(string className, string propertyName, bool logMiss = true)
    {
        var key = new SchemaKey(className, propertyName);
        if (_schemaOffsets.TryGetValue(key, out var offset)) return offset;

        offset = LookupField(className, propertyName);
        if (!_schemaOffsets.TryAdd(key, offset) || offset >= 0 || !logMiss) return offset;

        var reason = offset == SchemaClassMissing ? "class missing" : "field missing";
        if (CoreConfig.SchemaMissMode == SchemaMissMode.Throw)
        {
            Logger.LogError("Schema field {ClassName}::{FieldName} not found ({Reason}); accessing it throws",
                className, propertyName, reason);
        }
        else
        {
            Logger.LogWarning(
                "Schema field {ClassName}::{FieldName} not found ({Reason}); reads return the default value and writes are dropped. " +
                "A later release will throw instead (SchemaMissMode: throw)",
                className, propertyName, reason);
        }

        return offset;
    }

    /// <summary>
    /// The offset of a field for an accessor, or null for a missing field in <see cref="SchemaMissMode.Warn"/>: the
    /// accessor then returns the type's default value or drops the write. Never 0 for a missing field.
    /// </summary>
    /// <exception cref="SchemaFieldNotFoundException">The field is missing and the mode is <see cref="SchemaMissMode.Throw"/>.</exception>
    private static short? AccessorOffset(string className, string propertyName)
    {
        var offset = LookupOffset(className, propertyName);
        if (offset >= 0) return (short)offset;
        if (CoreConfig.SchemaMissMode == SchemaMissMode.Throw)
            throw new SchemaFieldNotFoundException(className, propertyName, offset == SchemaClassMissing);
        return null;
    }

    // Same, after the FollowCS2ServerGuidelines check.
    private static short? GuardedAccessorOffset(string className, string propertyName)
    {
        ThrowIfGuidelinesBlock(className, propertyName);
        return AccessorOffset(className, propertyName);
    }

    // A location nothing else reads: a missing field's reads see the default value, its writes go nowhere.
    private static ref T Discard<T>() => ref (new T[1])[0];

    public static bool IsSchemaFieldNetworked(string className, string propertyName)
    {
        return NativeAPI.IsSchemaFieldNetworked(className, propertyName);
    }

    // Accessors: a missing field throws SchemaFieldNotFoundException in SchemaMissMode.Throw; in Warn, reads return the
    // type's default value (an empty string for strings) and writes are dropped.

    public static T GetSchemaValue<T>(IntPtr handle, string className, string propertyName)
    {
        if (handle == IntPtr.Zero) throw new ArgumentNullException(nameof(handle), "Schema target points to null.");

        if (AccessorOffset(className, propertyName) == null) return default!;
        return NativeAPI.GetSchemaValueByName<T>(handle, (int)typeof(T).ToDataType(), className, propertyName);
    }

    public static void SetSchemaValue<T>(IntPtr handle, string className, string propertyName, T value)
    {
        if (handle == IntPtr.Zero) throw new ArgumentNullException(nameof(handle), "Schema target points to null.");

        if (GuardedAccessorOffset(className, propertyName) == null) return;
        NativeAPI.SetSchemaValueByName<T>(handle, (int)typeof(T).ToDataType(), className, propertyName, value);
    }

    public static T GetDeclaredClass<T>(IntPtr pointer, string className, string memberName)
    {
        if (pointer == IntPtr.Zero) throw new ArgumentNullException(nameof(pointer), "Schema target points to null.");

        if (GuardedAccessorOffset(className, memberName) is not { } offset) return default!;
        return FastNew.CreateInstance<T, IntPtr>(pointer + offset);
    }

    public static unsafe ref T GetRef<T>(IntPtr pointer, string className, string memberName)
    {
        if (pointer == IntPtr.Zero) throw new ArgumentNullException(nameof(pointer), "Schema target points to null.");

        if (GuardedAccessorOffset(className, memberName) is not { } offset) return ref Discard<T>();
        return ref Unsafe.AsRef<T>((void*)(pointer + offset));
    }

    public static T GetPointer<T>(IntPtr pointer)
    {
        var pointerTo = Marshal.ReadIntPtr(pointer);
        if (pointerTo == IntPtr.Zero)
        {
            return default;
        }

        return FastNew.CreateInstance<T, IntPtr>(pointerTo);
    }

    public static T GetPointer<T>(IntPtr pointer, string className, string memberName)
    {
        if (pointer == IntPtr.Zero) throw new ArgumentNullException(nameof(pointer), "Schema target points to null.");

        if (GuardedAccessorOffset(className, memberName) is not { } offset) return default!;
        var pointerTo = Marshal.ReadIntPtr(pointer + offset);
        if (pointerTo == IntPtr.Zero)
        {
            return default;
        }

        return FastNew.CreateInstance<T, IntPtr>(pointerTo);
    }

    public static unsafe Span<T> GetFixedArray<T>(IntPtr pointer, string className, string memberName, int count)
    {
        if (pointer == IntPtr.Zero) throw new ArgumentNullException(nameof(pointer), "Schema target points to null.");

        if (GuardedAccessorOffset(className, memberName) is not { } offset) return new T[count];
        Span<T> span = new((void*)(pointer + offset), count);
        return span;
    }

    /// <summary>
    /// Reads a string from the specified pointer, class name, and member name.
    /// These are for non-networked strings, which are just stored as raw char bytes on the server.
    /// </summary>
    /// <returns></returns>
    public static string GetString(IntPtr pointer, string className, string memberName)
    {
        return GetSchemaValue<string>(pointer, className, memberName) ?? string.Empty;
    }

    /// <summary>
    /// Reads a UTF8 encoded string from the specified pointer, class name, and member name.
    /// These are for networked strings, which need to be read differently.
    /// </summary>
    /// <param name="pointer"></param>
    /// <param name="className"></param>
    /// <param name="memberName"></param>
    /// <returns></returns>
    public static string GetUtf8String(IntPtr pointer, string className, string memberName)
    {
        if (GuardedAccessorOffset(className, memberName) is not { } offset) return string.Empty;
        return Utilities.ReadStringUtf8(pointer + offset);
    }

    // Used to write to `string_t` and `char*` pointer type strings
    public unsafe static void SetString(IntPtr pointer, string className, string memberName, string value)
    {
        SetSchemaValue(pointer, className, memberName, value);
    }

    // Used to write to the char[] specified at the schema location, i.e. char m_iszPlayerName[128]; 
    internal unsafe static void SetStringBytes(IntPtr pointer, string className, string memberName, string value, int maxLength)
    {
        if (GuardedAccessorOffset(className, memberName) == null) return;
        var handle = GetSchemaValue<IntPtr>(pointer, className, memberName);

        var bytes = Encoding.UTF8.GetBytes(value);
        if (bytes.Length >= maxLength)
        {
            throw new ArgumentException($"String length exceeds maximum length of {maxLength - 1}");
        }

        for (int i = 0; i < bytes.Length; i++)
        {
            Unsafe.Write((void*)(handle.ToInt64() + i), bytes[i]);
        }

        Unsafe.Write((void*)(handle.ToInt64() + bytes.Length), (byte)0);
    }

    public static T GetCustomMarshalledType<T>(IntPtr pointer, string className, string memberName)
    {
        if (GuardedAccessorOffset(className, memberName) is not { } offset) return default!;
        var type = typeof(T);
        object result = type switch
        {
            _ when type == typeof(Color) => Marshaling.ColorMarshaler.NativeToManaged(pointer + offset),
            _ => throw new NotSupportedException(),
        };

        return (T)result;
    }

    public static void SetCustomMarshalledType<T>(IntPtr pointer, string className, string memberName, T value)
    {
        if (GuardedAccessorOffset(className, memberName) is not { } offset) return;
        var type = typeof(T);
        switch (type)
        {
            case var _ when value is Color c:
                Marshaling.ColorMarshaler.ManagedToNative(pointer + offset, c);
                break;
            default:
                throw new NotSupportedException();
        }
    }
}