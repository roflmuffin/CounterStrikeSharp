#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hostage_killed")]
public class EventHostageKilled : GameEvent
{
    public EventHostageKilled(IntPtr pointer) : base(pointer){}
    public EventHostageKilled(bool force) : base("hostage_killed", force){}
    // hostage entity index
    public int Hostage
    {
        get => Get<int>("hostage");
        set => Set<int>("hostage", value);
    }
// player who killed the hostage
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
