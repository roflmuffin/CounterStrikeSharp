#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("instructor_server_hint_stop")]
public class EventInstructorServerHintStop : GameEvent
{
    public EventInstructorServerHintStop(IntPtr pointer) : base(pointer){}
    public EventInstructorServerHintStop(bool force) : base("instructor_server_hint_stop", force){}
    // entity id of the env_instructor_hint that fired the event
    public long HintEntindex
    {
        get => Get<long>("hint_entindex");
        set => Set<long>("hint_entindex", value);
    }
// The hint to stop. Will stop ALL hints with this name
    public string HintName
    {
        get => Get<string>("hint_name");
        set => Set<string>("hint_name", value);
    }
}
#nullable restore
