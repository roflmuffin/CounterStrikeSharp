#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("switch_team")]
public class EventSwitchTeam : GameEvent
{
    public EventSwitchTeam(IntPtr pointer) : base(pointer){}
    public EventSwitchTeam(bool force) : base("switch_team", force){}
    // average rank of human players
    public int AvgRank
    {
        get => Get<int>("avg_rank");
        set => Set<int>("avg_rank", value);
    }

    public int Numctslotsfree
    {
        get => Get<int>("numCTSlotsFree");
        set => Set<int>("numCTSlotsFree", value);
    }
// number of active players on both T and CT
    public int Numplayers
    {
        get => Get<int>("numPlayers");
        set => Set<int>("numPlayers", value);
    }
// number of spectators
    public int Numspectators
    {
        get => Get<int>("numSpectators");
        set => Set<int>("numSpectators", value);
    }

    public int Numtslotsfree
    {
        get => Get<int>("numTSlotsFree");
        set => Set<int>("numTSlotsFree", value);
    }
}
#nullable restore
