#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hostage_rescued")]
public class EventHostageRescued : GameEvent
{
    public EventHostageRescued(IntPtr pointer) : base(pointer){}
    public EventHostageRescued(bool force) : base("hostage_rescued", force){}
    // hostage entity index
    public int Hostage
    {
        get => Get<int>("hostage");
        set => Set<int>("hostage", value);
    }
// rescue site index
    public int Site
    {
        get => Get<int>("site");
        set => Set<int>("site", value);
    }
// player who rescued the hostage
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
