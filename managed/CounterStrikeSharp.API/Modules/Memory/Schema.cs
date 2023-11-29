using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Memory;

public class Schema
{
    private static Dictionary<Tuple<string, string>, short> _schemaOffsets = new();

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

    public static short GetSchemaOffset(string className, string propertyName)
    {
        if (CoreConfig.FollowCS2ServerGuidelines && _cs2BadList.Contains(propertyName))
        {
            throw new Exception($"Cannot set or get '{className}::{propertyName}' with \"FollowCS2ServerGuidelines\" option enabled.");
        }

        var key = new Tuple<string, string>(className, propertyName);
        if (!_schemaOffsets.TryGetValue(key, out var offset))
        {
            offset = NativeAPI.GetSchemaOffset(className, propertyName);
            _schemaOffsets.Add(key, offset);
        }

        return offset;
    }

    public static T GetSchemaValue<T>(IntPtr handle, string className, string propertyName)
    {
        return NativeAPI.GetSchemaValueByName<T>(handle, (int)typeof(T).ToDataType(), className, propertyName);
    }

    public static void SetSchemaValue<T>(IntPtr handle, string className, string propertyName, T value)
    {
        if (CoreConfig.FollowCS2ServerGuidelines && _cs2BadList.Contains(propertyName))
        {
            throw new Exception($"Cannot set or get '{className}::{propertyName}' with \"FollowCS2ServerGuidelines\" option enabled.");
        }

        NativeAPI.SetSchemaValueByName<T>(handle, (int)typeof(T).ToDataType(), className, propertyName, value);
    }

    public static T GetDeclaredClass<T>(IntPtr pointer, string className, string memberName)
    {
        return (T)Activator.CreateInstance(typeof(T), pointer + GetSchemaOffset(className, memberName));
    }

    public static unsafe ref T GetRef<T>(IntPtr pointer, string className, string memberName)
    {
        return ref Unsafe.AsRef<T>((void*)(pointer + GetSchemaOffset(className, memberName)));
    }

    public static T GetPointer<T>(IntPtr pointer)
    {
        var pointerTo = Marshal.ReadIntPtr(pointer);
        if (pointerTo == IntPtr.Zero)
        {
            return default;
        }

        return (T)Activator.CreateInstance(typeof(T), pointerTo);
    }

    public static T GetPointer<T>(IntPtr pointer, string className, string memberName)
    {
        var pointerTo = Marshal.ReadIntPtr(pointer + GetSchemaOffset(className, memberName));
        if (pointerTo == IntPtr.Zero)
        {
            return default;
        }

        return (T)Activator.CreateInstance(typeof(T), pointerTo);
    }

    public static unsafe Span<T> GetFixedArray<T>(IntPtr pointer, string className, string memberName, int count)
    {
        Span<T> span = new((void*)(pointer + GetSchemaOffset(className, memberName)), count);
        return span;
    }

    /// <summary>
    /// Reads a string from the specified pointer, class name, and member name.
    /// These are for non-networked strings, which are just stored as raw char bytes on the server.
    /// </summary>
    /// <returns></returns>
    public static string GetString(IntPtr pointer, string className, string memberName)
    {
        return GetSchemaValue<string>(pointer, className, memberName);
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
        return Utilities.ReadStringUtf8(pointer + GetSchemaOffset(className, memberName));
    }

    public static void SetString(IntPtr pointer, string className, string memberName, string value)
    {
        SetSchemaValue(pointer, className, memberName, value);
    }
    
   
    public static T GetCustomMarshalledType<T>(IntPtr pointer, string className, string memberName)
    {
        var type = typeof(T);
        object result = type switch
        {
            _ when type == typeof(Color) => Marshaling.ColorMarshaler.NativeToManaged(pointer + GetSchemaOffset(className, memberName)),
            _ => throw new NotSupportedException(),
        };

        return (T)result;
    }
    
    public static void SetCustomMarshalledType<T>(IntPtr pointer, string className, string memberName, T value)
    {
        var type = typeof(T);
        switch (type)
        {
            case var _ when value is Color c:
                Marshaling.ColorMarshaler.ManagedToNative(pointer + GetSchemaOffset(className, memberName), c);
                break;
            default:
                throw new NotSupportedException();
        }
    }
}