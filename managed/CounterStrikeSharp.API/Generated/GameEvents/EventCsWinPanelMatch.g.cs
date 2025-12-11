#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("cs_win_panel_match")]
public class EventCsWinPanelMatch : GameEvent
{
    public EventCsWinPanelMatch(IntPtr pointer) : base(pointer){}
    public EventCsWinPanelMatch(bool force) : base("cs_win_panel_match", force){}
    
}
#nullable restore
