#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hostage_follows")]
public class EventHostageFollows : GameEvent
{
    public EventHostageFollows(IntPtr pointer) : base(pointer){}
    public EventHostageFollows(bool force) : base("hostage_follows", force){}
    // hostage entity index
    public int Hostage
    {
        get => Get<int>("hostage");
        set => Set<int>("hostage", value);
    }
// player who touched the hostage
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
