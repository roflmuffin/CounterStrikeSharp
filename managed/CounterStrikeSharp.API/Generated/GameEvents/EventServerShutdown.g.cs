#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("server_shutdown")]
public class EventServerShutdown : GameEvent
{
    public EventServerShutdown(IntPtr pointer) : base(pointer){}
    public EventServerShutdown(bool force) : base("server_shutdown", force){}
    // reason why server was shut down
    public string Reason
    {
        get => Get<string>("reason");
        set => Set<string>("reason", value);
    }
}
#nullable restore
