#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("vote_changed")]
public class EventVoteChanged : GameEvent
{
    public EventVoteChanged(IntPtr pointer) : base(pointer){}
    public EventVoteChanged(bool force) : base("vote_changed", force){}
    
    public int Novotes
    {
        get => Get<int>("noVotes");
        set => Set<int>("noVotes", value);
    }

    public int Potentialvotes
    {
        get => Get<int>("potentialVotes");
        set => Set<int>("potentialVotes", value);
    }

    public int VoteOption1
    {
        get => Get<int>("vote_option1");
        set => Set<int>("vote_option1", value);
    }

    public int VoteOption2
    {
        get => Get<int>("vote_option2");
        set => Set<int>("vote_option2", value);
    }

    public int VoteOption3
    {
        get => Get<int>("vote_option3");
        set => Set<int>("vote_option3", value);
    }

    public int VoteOption4
    {
        get => Get<int>("vote_option4");
        set => Set<int>("vote_option4", value);
    }

    public int VoteOption5
    {
        get => Get<int>("vote_option5");
        set => Set<int>("vote_option5", value);
    }

    public int Yesvotes
    {
        get => Get<int>("yesVotes");
        set => Set<int>("yesVotes", value);
    }
}
#nullable restore
