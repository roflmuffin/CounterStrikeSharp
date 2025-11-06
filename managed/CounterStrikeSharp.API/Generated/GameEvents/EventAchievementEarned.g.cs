#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("achievement_earned")]
public class EventAchievementEarned : GameEvent
{
    public EventAchievementEarned(IntPtr pointer) : base(pointer){}
    public EventAchievementEarned(bool force) : base("achievement_earned", force){}
    // achievement ID
    public int Achievement
    {
        get => Get<int>("achievement");
        set => Set<int>("achievement", value);
    }
// entindex of the player
    public CCSPlayerController? Player
    {
        get => GetPlayer("player");
        set => SetPlayer("player", value);
    }
}
#nullable restore
