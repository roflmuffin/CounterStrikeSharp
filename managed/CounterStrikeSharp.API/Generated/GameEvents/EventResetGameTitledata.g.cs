#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("reset_game_titledata")]
public class EventResetGameTitledata : GameEvent
{
    public EventResetGameTitledata(IntPtr pointer) : base(pointer){}
    public EventResetGameTitledata(bool force) : base("reset_game_titledata", force){}
    // Controller id of user
    public int Controllerid
    {
        get => Get<int>("controllerId");
        set => Set<int>("controllerId", value);
    }
}
#nullable restore
