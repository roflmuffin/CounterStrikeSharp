using System.Reflection.Metadata;

namespace CounterStrikeSharp.API.Core;

public class CVariant : NativeObject
{
    public CVariant(IntPtr pointer) : base(pointer)
    {
    }

    public bool IsValid => Handle != IntPtr.Zero;

    public fieldtype_t FieldType => (fieldtype_t)NativeAPI.GetVariantType(Handle);

    public T Get<T>()
    {
        var type = typeof(T);
        object result = type switch
        {
            _ when type == typeof(float) => NativeAPI.GetVariantFloat(Handle),
            _ when type == typeof(int) => NativeAPI.GetVariantInt(Handle),
            _ when type == typeof(uint) => NativeAPI.GetVariantUint(Handle),
            _ when type == typeof(string) => NativeAPI.GetVariantString(Handle),
            _ when type == typeof(bool) => NativeAPI.GetVariantBool(Handle),
            _ => throw new NotSupportedException(),
        };

        return (T)result;
    }

    public void Set<T>(T value)
    {
        var type = typeof(T);
        switch (type)
        {
            case var _ when value is float f:
                NativeAPI.SetVariantFloat(Handle, f);
            break;
            case var _ when value is int i:
                NativeAPI.SetVariantInt(Handle, i);
            break;
            case var _ when value is uint ui:
                NativeAPI.SetVariantUint(Handle, ui);
            break;
            case var _ when value is bool b:
                NativeAPI.SetVariantBool(Handle, b);
            break;
            case var _ when value is string s:
                NativeAPI.SetVariantString(Handle, s);
            break;

            default:
            throw new NotSupportedException();
        }
    }
}
