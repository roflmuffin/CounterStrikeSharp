#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_decal")]
public class EventPlayerDecal : GameEvent
{
    public EventPlayerDecal(IntPtr pointer) : base(pointer){}
    public EventPlayerDecal(bool force) : base("player_decal", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
