#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("silencer_detach")]
public class EventSilencerDetach : GameEvent
{
    public EventSilencerDetach(IntPtr pointer) : base(pointer){}
    public EventSilencerDetach(bool force) : base("silencer_detach", force){}
    
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
