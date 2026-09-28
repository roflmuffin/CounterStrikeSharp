#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hostname_changed")]
public class EventHostnameChanged : GameEvent
{
    public EventHostnameChanged(IntPtr pointer) : base(pointer){}
    public EventHostnameChanged(bool force) : base("hostname_changed", force){}
    
    public string Hostname
    {
        get => Get<string>("hostname");
        set => Set<string>("hostname", value);
    }
}
#nullable restore
