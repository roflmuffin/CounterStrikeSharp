#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("silencer_on")]
public class EventSilencerOn : GameEvent
{
    public EventSilencerOn(IntPtr pointer) : base(pointer){}
    public EventSilencerOn(bool force) : base("silencer_on", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
