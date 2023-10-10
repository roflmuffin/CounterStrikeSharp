using System;
using CounterStrikeSharp.API.Modules.Events;

namespace CounterStrikeSharp.API.Core.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public class GameEventHandlerAttribute : Attribute
{
    public GameEventHandlerAttribute()
    {
    }
}