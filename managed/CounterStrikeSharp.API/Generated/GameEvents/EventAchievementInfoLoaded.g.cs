#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("achievement_info_loaded")]
public class EventAchievementInfoLoaded : GameEvent
{
    public EventAchievementInfoLoaded(IntPtr pointer) : base(pointer){}
    public EventAchievementInfoLoaded(bool force) : base("achievement_info_loaded", force){}
    
}
#nullable restore
