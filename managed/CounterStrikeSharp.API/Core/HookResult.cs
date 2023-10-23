namespace CounterStrikeSharp.API.Core;

public enum HookResult
{
    Continue = 0,
    Changed = 1,
    Handled = 3,
    Stop = 4,
}