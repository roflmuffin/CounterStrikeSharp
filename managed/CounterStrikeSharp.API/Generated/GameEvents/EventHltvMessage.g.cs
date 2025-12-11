#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hltv_message")]
public class EventHltvMessage : GameEvent
{
    public EventHltvMessage(IntPtr pointer) : base(pointer){}
    public EventHltvMessage(bool force) : base("hltv_message", force){}
    
    public string Text
    {
        get => Get<string>("text");
        set => Set<string>("text", value);
    }
}
#nullable restore
