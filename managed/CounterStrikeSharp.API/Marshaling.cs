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
        var color = Marshal.ReadInt32(pointer);
        var alpha = (byte)((color >> 24) & 0xFF);
        var blue = (byte)((color >> 16) & 0xFF);
        var green = (byte)((color >> 8) & 0xFF);
        var red = (byte)(color & 0xFF);
        return Color.FromArgb(alpha, red, green, blue);
    }

    public void ManagedToNative(IntPtr pointer, Color managedObj)
    {
        Marshal.WriteInt32(pointer, (managedObj.A << 24) | (managedObj.B << 16) | (managedObj.G << 8) | managedObj.R);
    }
}