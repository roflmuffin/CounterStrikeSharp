#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("nav_generate")]
public class EventNavGenerate : GameEvent
{
    public EventNavGenerate(IntPtr pointer) : base(pointer){}
    public EventNavGenerate(bool force) : base("nav_generate", force){}
    
}
#nullable restore
