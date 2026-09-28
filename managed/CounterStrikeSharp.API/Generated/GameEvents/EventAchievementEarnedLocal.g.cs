#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("achievement_earned_local")]
public class EventAchievementEarnedLocal : GameEvent
{
    public EventAchievementEarnedLocal(IntPtr pointer) : base(pointer){}
    public EventAchievementEarnedLocal(bool force) : base("achievement_earned_local", force){}
    // achievement ID
    public int Achievement
    {
        get => Get<int>("achievement");
        set => Set<int>("achievement", value);
    }
// splitscreen ID
    public int Splitscreenplayer
    {
        get => Get<int>("splitscreenplayer");
        set => Set<int>("splitscreenplayer", value);
    }
}
#nullable restore
