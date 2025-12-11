#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("vote_ended")]
public class EventVoteEnded : GameEvent
{
    public EventVoteEnded(IntPtr pointer) : base(pointer){}
    public EventVoteEnded(bool force) : base("vote_ended", force){}
    
}
#nullable restore
