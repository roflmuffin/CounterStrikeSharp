#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("endmatch_cmm_start_reveal_items")]
public class EventEndmatchCmmStartRevealItems : GameEvent
{
    public EventEndmatchCmmStartRevealItems(IntPtr pointer) : base(pointer){}
    public EventEndmatchCmmStartRevealItems(bool force) : base("endmatch_cmm_start_reveal_items", force){}
    
}
#nullable restore
