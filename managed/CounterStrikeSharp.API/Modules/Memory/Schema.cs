using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Memory;

public class Schema
{
    private static Dictionary<Tuple<string, string>, short> _schemaOffsets = new();

    public static short GetSchemaOffset(string className, string propertyName)
    {
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

    public static unsafe T GetPointer<T>(IntPtr pointer)
    {
        var pointerTo = Unsafe.Read<IntPtr>((void*)pointer);
        if (pointerTo == IntPtr.Zero)
        {
            return default;
        }

        return (T)Activator.CreateInstance(typeof(T), pointerTo);
    }

    public static unsafe T GetPointer<T>(IntPtr pointer, string className, string memberName)
    {
        var pointerTo = Unsafe.Read<IntPtr>((void*)(pointer + GetSchemaOffset(className, memberName)));
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
}