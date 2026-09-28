#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_full_update")]
public class EventPlayerFullUpdate : GameEvent
{
    public EventPlayerFullUpdate(IntPtr pointer) : base(pointer){}
    public EventPlayerFullUpdate(bool force) : base("player_full_update", force){}
    // Number of this full update
    public int Count
    {
        get => Get<int>("count");
        set => Set<int>("count", value);
    }
// user ID on server
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
