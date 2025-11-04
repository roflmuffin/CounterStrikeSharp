#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hltv_rank_entity")]
public class EventHltvRankEntity : GameEvent
{
    public EventHltvRankEntity(IntPtr pointer) : base(pointer){}
    public EventHltvRankEntity(bool force) : base("hltv_rank_entity", force){}
    // ranking, how interesting is this entity to view
    public virtual float Rank
    {
        get => Get<float>("rank");
        set => Set<float>("rank", value);
    }
// best/closest target entity
    public virtual CCSPlayerController? Target
    {
        get => GetPlayer("target");
        set => SetPlayer("target", value);
    }
// player slot
    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
