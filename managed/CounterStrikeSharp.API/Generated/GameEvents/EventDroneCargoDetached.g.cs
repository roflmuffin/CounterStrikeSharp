#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("drone_cargo_detached")]
public class EventDroneCargoDetached : GameEvent
{
    public EventDroneCargoDetached(IntPtr pointer) : base(pointer){}
    public EventDroneCargoDetached(bool force) : base("drone_cargo_detached", force){}
    
    public virtual int Cargo
    {
        get => Get<int>("cargo");
        set => Set<int>("cargo", value);
    }

    public virtual bool Delivered
    {
        get => Get<bool>("delivered");
        set => Set<bool>("delivered", value);
    }

    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
