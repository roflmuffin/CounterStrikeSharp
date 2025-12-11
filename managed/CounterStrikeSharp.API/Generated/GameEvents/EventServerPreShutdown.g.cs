#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("server_pre_shutdown")]
public class EventServerPreShutdown : GameEvent
{
    public EventServerPreShutdown(IntPtr pointer) : base(pointer){}
    public EventServerPreShutdown(bool force) : base("server_pre_shutdown", force){}
    // reason why server is about to be shut down
    public string Reason
    {
        get => Get<string>("reason");
        set => Set<string>("reason", value);
    }
}
#nullable restore
