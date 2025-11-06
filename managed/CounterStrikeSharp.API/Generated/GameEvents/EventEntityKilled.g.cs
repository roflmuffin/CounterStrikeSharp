#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("entity_killed")]
public class EventEntityKilled : GameEvent
{
    public EventEntityKilled(IntPtr pointer) : base(pointer){}
    public EventEntityKilled(bool force) : base("entity_killed", force){}
    
    public long Damagebits
    {
        get => Get<long>("damagebits");
        set => Set<long>("damagebits", value);
    }

    public long EntindexAttacker
    {
        get => Get<long>("entindex_attacker");
        set => Set<long>("entindex_attacker", value);
    }

    public long EntindexInflictor
    {
        get => Get<long>("entindex_inflictor");
        set => Set<long>("entindex_inflictor", value);
    }

    public long EntindexKilled
    {
        get => Get<long>("entindex_killed");
        set => Set<long>("entindex_killed", value);
    }
}
#nullable restore
