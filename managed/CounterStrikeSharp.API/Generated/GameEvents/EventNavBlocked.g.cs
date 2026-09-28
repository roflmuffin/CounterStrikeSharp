#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("nav_blocked")]
public class EventNavBlocked : GameEvent
{
    public EventNavBlocked(IntPtr pointer) : base(pointer){}
    public EventNavBlocked(bool force) : base("nav_blocked", force){}
    
    public long Area
    {
        get => Get<long>("area");
        set => Set<long>("area", value);
    }

    public bool Blocked
    {
        get => Get<bool>("blocked");
        set => Set<bool>("blocked", value);
    }
}
#nullable restore
