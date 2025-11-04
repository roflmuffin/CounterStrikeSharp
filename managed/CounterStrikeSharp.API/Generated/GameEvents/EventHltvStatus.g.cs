#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hltv_status")]
public class EventHltvStatus : GameEvent
{
    public EventHltvStatus(IntPtr pointer) : base(pointer){}
    public EventHltvStatus(bool force) : base("hltv_status", force){}
    // number of HLTV spectators
    public virtual long Clients
    {
        get => Get<long>("clients");
        set => Set<long>("clients", value);
    }
// disptach master IP:port
    public virtual string Master
    {
        get => Get<string>("master");
        set => Set<string>("master", value);
    }
// number of HLTV proxies
    public virtual int Proxies
    {
        get => Get<int>("proxies");
        set => Set<int>("proxies", value);
    }
// number of HLTV slots
    public virtual long Slots
    {
        get => Get<long>("slots");
        set => Set<long>("slots", value);
    }
}
#nullable restore
