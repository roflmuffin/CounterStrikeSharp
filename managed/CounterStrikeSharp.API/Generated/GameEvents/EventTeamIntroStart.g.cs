#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("team_intro_start")]
public class EventTeamIntroStart : GameEvent
{
    public EventTeamIntroStart(IntPtr pointer) : base(pointer){}
    public EventTeamIntroStart(bool force) : base("team_intro_start", force){}
    
}
#nullable restore
