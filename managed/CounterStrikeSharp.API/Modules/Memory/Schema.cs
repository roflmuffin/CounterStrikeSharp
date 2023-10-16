using System;
using System.IO;
using CounterStrikeSharp.API.Core;

namespace CounterStrikeSharp.API.Modules.Memory;

public class Schema
{
    public static T GetSchemaValue<T>(IntPtr handle, string className, string propertyName)
    {
        return NativeAPI.GetSchemaValueByName<T>(handle, (int)typeof(T).ToDataType(), className, propertyName);
    }
    
    public static void SetSchemaValue<T>(IntPtr handle, string className, string propertyName, T value)
    {
        NativeAPI.SetSchemaValueByName<T>(handle, (int)typeof(T).ToDataType(), className, propertyName, value);
    }
}