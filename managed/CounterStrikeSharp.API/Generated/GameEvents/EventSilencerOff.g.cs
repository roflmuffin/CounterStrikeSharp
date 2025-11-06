#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("silencer_off")]
public class EventSilencerOff : GameEvent
{
    public EventSilencerOff(IntPtr pointer) : base(pointer){}
    public EventSilencerOff(bool force) : base("silencer_off", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
