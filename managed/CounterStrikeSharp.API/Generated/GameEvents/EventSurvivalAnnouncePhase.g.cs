#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("survival_announce_phase")]
public class EventSurvivalAnnouncePhase : GameEvent
{
    public EventSurvivalAnnouncePhase(IntPtr pointer) : base(pointer){}
    public EventSurvivalAnnouncePhase(bool force) : base("survival_announce_phase", force){}
    // The phase #
    public int Phase
    {
        get => Get<int>("phase");
        set => Set<int>("phase", value);
    }
}
#nullable restore
