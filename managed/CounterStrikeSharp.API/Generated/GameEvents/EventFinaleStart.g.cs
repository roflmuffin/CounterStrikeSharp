#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("finale_start")]
public class EventFinaleStart : GameEvent
{
    public EventFinaleStart(IntPtr pointer) : base(pointer){}
    public EventFinaleStart(bool force) : base("finale_start", force){}
    
    public int Rushes
    {
        get => Get<int>("rushes");
        set => Set<int>("rushes", value);
    }
}
#nullable restore
