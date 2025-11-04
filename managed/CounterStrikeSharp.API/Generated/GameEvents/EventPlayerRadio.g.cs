#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_radio")]
public class EventPlayerRadio : GameEvent
{
    public EventPlayerRadio(IntPtr pointer) : base(pointer){}
    public EventPlayerRadio(bool force) : base("player_radio", force){}
    
    public virtual int Slot
    {
        get => Get<int>("slot");
        set => Set<int>("slot", value);
    }

    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
