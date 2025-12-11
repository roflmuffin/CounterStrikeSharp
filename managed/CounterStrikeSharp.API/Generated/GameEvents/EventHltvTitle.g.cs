#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hltv_title")]
public class EventHltvTitle : GameEvent
{
    public EventHltvTitle(IntPtr pointer) : base(pointer){}
    public EventHltvTitle(bool force) : base("hltv_title", force){}
    
    public string Text
    {
        get => Get<string>("text");
        set => Set<string>("text", value);
    }
}
#nullable restore
