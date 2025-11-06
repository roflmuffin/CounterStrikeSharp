#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("vote_passed")]
public class EventVotePassed : GameEvent
{
    public EventVotePassed(IntPtr pointer) : base(pointer){}
    public EventVotePassed(bool force) : base("vote_passed", force){}
    
    public string Details
    {
        get => Get<string>("details");
        set => Set<string>("details", value);
    }

    public string Param1
    {
        get => Get<string>("param1");
        set => Set<string>("param1", value);
    }
// this event is reliable
    public int Reliable
    {
        get => Get<int>("reliable");
        set => Set<int>("reliable", value);
    }

    public int Team
    {
        get => Get<int>("team");
        set => Set<int>("team", value);
    }
}
#nullable restore
