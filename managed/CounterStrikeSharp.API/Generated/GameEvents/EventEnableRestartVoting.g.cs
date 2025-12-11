#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("enable_restart_voting")]
public class EventEnableRestartVoting : GameEvent
{
    public EventEnableRestartVoting(IntPtr pointer) : base(pointer){}
    public EventEnableRestartVoting(bool force) : base("enable_restart_voting", force){}
    
    public bool Enable
    {
        get => Get<bool>("enable");
        set => Set<bool>("enable", value);
    }
}
#nullable restore
