#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("game_end")]
public class EventGameEnd : GameEvent
{
    public EventGameEnd(IntPtr pointer) : base(pointer){}
    public EventGameEnd(bool force) : base("game_end", force){}
    // winner team/user id
    public int Winner
    {
        get => Get<int>("winner");
        set => Set<int>("winner", value);
    }
}
#nullable restore
