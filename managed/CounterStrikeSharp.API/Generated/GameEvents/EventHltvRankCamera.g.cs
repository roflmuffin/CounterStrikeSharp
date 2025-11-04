#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hltv_rank_camera")]
public class EventHltvRankCamera : GameEvent
{
    public EventHltvRankCamera(IntPtr pointer) : base(pointer){}
    public EventHltvRankCamera(bool force) : base("hltv_rank_camera", force){}
    // fixed camera index
    public virtual int Index
    {
        get => Get<int>("index");
        set => Set<int>("index", value);
    }
// ranking, how interesting is this camera view
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
}
#nullable restore
