#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_hintmessage")]
public class EventPlayerHintmessage : GameEvent
{
    public EventPlayerHintmessage(IntPtr pointer) : base(pointer){}
    public EventPlayerHintmessage(bool force) : base("player_hintmessage", force){}
    // localizable string of a hint
    public string Hintmessage
    {
        get => Get<string>("hintmessage");
        set => Set<string>("hintmessage", value);
    }
}
#nullable restore
