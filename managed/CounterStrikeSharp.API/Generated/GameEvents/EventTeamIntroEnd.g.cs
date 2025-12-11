#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("team_intro_end")]
public class EventTeamIntroEnd : GameEvent
{
    public EventTeamIntroEnd(IntPtr pointer) : base(pointer){}
    public EventTeamIntroEnd(bool force) : base("team_intro_end", force){}
    
}
#nullable restore
