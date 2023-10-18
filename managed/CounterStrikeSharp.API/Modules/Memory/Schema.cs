using System;
using System.Collections.Generic;
using System.IO;
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
}