using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CounterStrikeSharp.API;

public static class Marshaling
{
    public static ColorMarshaler ColorMarshaler = new();
}

public interface ICustomMarshal<T>
{
    T NativeToManaged(IntPtr pointer);

    void ManagedToNative(IntPtr pointer, T managedObj);
}

public class ColorMarshaler : ICustomMarshal<Color>
{
    public Color NativeToManaged(IntPtr pointer)
    {
        return Color.FromArgb(Marshal.ReadInt32(pointer));
    }

    public void ManagedToNative(IntPtr pointer, Color managedObj)
    {
        Marshal.WriteInt32(pointer, managedObj.ToArgb());
    }
}