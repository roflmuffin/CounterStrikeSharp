namespace CounterStrikeSharp.API.Core;

/// <summary>
/// Placeholder for CVariant
/// <see href="https://github.com/alliedmodders/hl2sdk/blob/cs2/public/variant.h"/>
/// <remarks>A lot of entity outputs do not use this value</remarks>
/// </summary>
public class CVariant : NativeObject
{
    public CVariant(IntPtr pointer) : base(pointer)
    {
    }
}