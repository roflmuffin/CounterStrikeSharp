namespace CounterStrikeSharp.API.Natives;

public interface INativeHandle
{
    public IntPtr Handle { get; }
    public bool IsValid { get; }
}
