using System;

namespace CounterStrikeSharp.API.Core.Attributes.Registration;

[AttributeUsage(AttributeTargets.Method)]
public class GameEventHandlerAttribute : Attribute
{
    public GameEventHandlerAttribute()
    {
    }
}