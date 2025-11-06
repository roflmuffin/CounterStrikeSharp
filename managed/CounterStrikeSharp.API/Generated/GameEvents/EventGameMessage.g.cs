#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("game_message")]
public class EventGameMessage : GameEvent
{
    public EventGameMessage(IntPtr pointer) : base(pointer){}
    public EventGameMessage(bool force) : base("game_message", force){}
    // 0 = console, 1 = HUD
    public int Target
    {
        get => Get<int>("target");
        set => Set<int>("target", value);
    }
// the message text
    public string Text
    {
        get => Get<string>("text");
        set => Set<string>("text", value);
    }
}
#nullable restore
