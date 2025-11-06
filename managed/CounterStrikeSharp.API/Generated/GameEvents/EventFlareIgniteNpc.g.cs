#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("flare_ignite_npc")]
public class EventFlareIgniteNpc : GameEvent
{
    public EventFlareIgniteNpc(IntPtr pointer) : base(pointer){}
    public EventFlareIgniteNpc(bool force) : base("flare_ignite_npc", force){}
    // entity ignited
    public long Entindex
    {
        get => Get<long>("entindex");
        set => Set<long>("entindex", value);
    }
}
#nullable restore
