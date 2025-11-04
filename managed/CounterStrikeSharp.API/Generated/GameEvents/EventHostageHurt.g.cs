#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hostage_hurt")]
public class EventHostageHurt : GameEvent
{
    public EventHostageHurt(IntPtr pointer) : base(pointer){}
    public EventHostageHurt(bool force) : base("hostage_hurt", force){}
    // hostage entity index
    public virtual int Hostage
    {
        get => Get<int>("hostage");
        set => Set<int>("hostage", value);
    }
// player who hurt the hostage
    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
