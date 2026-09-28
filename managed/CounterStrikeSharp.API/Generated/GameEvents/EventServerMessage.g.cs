#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("server_message")]
public class EventServerMessage : GameEvent
{
    public EventServerMessage(IntPtr pointer) : base(pointer){}
    public EventServerMessage(bool force) : base("server_message", force){}
    // the message text
    public string Text
    {
        get => Get<string>("text");
        set => Set<string>("text", value);
    }
}
#nullable restore
