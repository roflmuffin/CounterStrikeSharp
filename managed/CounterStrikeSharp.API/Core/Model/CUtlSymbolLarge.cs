namespace CounterStrikeSharp.API.Core;

public class CUtlSymbolLarge : NativeObject
{
    public CUtlSymbolLarge(IntPtr pointer) : base(pointer)
    {
    }

    public bool IsValid => Handle != IntPtr.Zero;

    public string String
    {
        get
        {
            return NativeAPI.GetStringFromSymbolLarge(Handle);
        }
    }
}
