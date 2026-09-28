#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("firstbombs_incoming_warning")]
public class EventFirstbombsIncomingWarning : GameEvent
{
    public EventFirstbombsIncomingWarning(IntPtr pointer) : base(pointer){}
    public EventFirstbombsIncomingWarning(bool force) : base("firstbombs_incoming_warning", force){}
    
    public bool Global
    {
        get => Get<bool>("global");
        set => Set<bool>("global", value);
    }
}
#nullable restore
