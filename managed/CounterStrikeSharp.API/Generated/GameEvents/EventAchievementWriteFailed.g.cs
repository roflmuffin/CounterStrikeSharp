#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("achievement_write_failed")]
public class EventAchievementWriteFailed : GameEvent
{
    public EventAchievementWriteFailed(IntPtr pointer) : base(pointer){}
    public EventAchievementWriteFailed(bool force) : base("achievement_write_failed", force){}
    
}
#nullable restore
