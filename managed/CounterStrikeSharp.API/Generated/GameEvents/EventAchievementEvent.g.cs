#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("achievement_event")]
public class EventAchievementEvent : GameEvent
{
    public EventAchievementEvent(IntPtr pointer) : base(pointer){}
    public EventAchievementEvent(bool force) : base("achievement_event", force){}
    // non-localized name of achievement
    public string AchievementName
    {
        get => Get<string>("achievement_name");
        set => Set<string>("achievement_name", value);
    }
// # of steps toward achievement
    public int CurVal
    {
        get => Get<int>("cur_val");
        set => Set<int>("cur_val", value);
    }
// total # of steps in achievement
    public int MaxVal
    {
        get => Get<int>("max_val");
        set => Set<int>("max_val", value);
    }
}
#nullable restore
