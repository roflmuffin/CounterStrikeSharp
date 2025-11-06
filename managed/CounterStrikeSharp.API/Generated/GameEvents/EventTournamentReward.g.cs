#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("tournament_reward")]
public class EventTournamentReward : GameEvent
{
    public EventTournamentReward(IntPtr pointer) : base(pointer){}
    public EventTournamentReward(bool force) : base("tournament_reward", force){}
    
    public long Accountid
    {
        get => Get<long>("accountid");
        set => Set<long>("accountid", value);
    }

    public long Defindex
    {
        get => Get<long>("defindex");
        set => Set<long>("defindex", value);
    }

    public long Totalrewards
    {
        get => Get<long>("totalrewards");
        set => Set<long>("totalrewards", value);
    }
}
#nullable restore
