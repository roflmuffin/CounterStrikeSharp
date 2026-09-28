#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("vote_options")]
public class EventVoteOptions : GameEvent
{
    public EventVoteOptions(IntPtr pointer) : base(pointer){}
    public EventVoteOptions(bool force) : base("vote_options", force){}
    // Number of options - up to MAX_VOTE_OPTIONS
    public int Count
    {
        get => Get<int>("count");
        set => Set<int>("count", value);
    }

    public string Option1
    {
        get => Get<string>("option1");
        set => Set<string>("option1", value);
    }

    public string Option2
    {
        get => Get<string>("option2");
        set => Set<string>("option2", value);
    }

    public string Option3
    {
        get => Get<string>("option3");
        set => Set<string>("option3", value);
    }

    public string Option4
    {
        get => Get<string>("option4");
        set => Set<string>("option4", value);
    }

    public string Option5
    {
        get => Get<string>("option5");
        set => Set<string>("option5", value);
    }
}
#nullable restore
