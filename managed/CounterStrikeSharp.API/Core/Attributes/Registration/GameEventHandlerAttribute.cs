using System;

namespace CounterStrikeSharp.API.Core.Attributes.Registration;

[AttributeUsage(AttributeTargets.Method)]
public class GameEventHandlerAttribute : Attribute
{
    public HookMode Mode { get; set; }
    public GameEventHandlerAttribute(HookMode mode = HookMode.Post)
    {
        Mode = mode;
    }
}