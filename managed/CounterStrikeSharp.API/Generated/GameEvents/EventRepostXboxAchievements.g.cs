#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("repost_xbox_achievements")]
public class EventRepostXboxAchievements : GameEvent
{
    public EventRepostXboxAchievements(IntPtr pointer) : base(pointer){}
    public EventRepostXboxAchievements(bool force) : base("repost_xbox_achievements", force){}
    // splitscreen ID
    public int Splitscreenplayer
    {
        get => Get<int>("splitscreenplayer");
        set => Set<int>("splitscreenplayer", value);
    }
}
#nullable restore
