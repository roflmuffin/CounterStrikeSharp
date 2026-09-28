#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("game_newmap")]
public class EventGameNewmap : GameEvent
{
    public EventGameNewmap(IntPtr pointer) : base(pointer){}
    public EventGameNewmap(bool force) : base("game_newmap", force){}
    // map name
    public string Mapname
    {
        get => Get<string>("mapname");
        set => Set<string>("mapname", value);
    }
// true if this is a transition from one map to another
    public bool Transition
    {
        get => Get<bool>("transition");
        set => Set<bool>("transition", value);
    }
}
#nullable restore
