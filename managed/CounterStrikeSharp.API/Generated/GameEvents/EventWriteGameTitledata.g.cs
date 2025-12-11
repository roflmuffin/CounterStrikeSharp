#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("write_game_titledata")]
public class EventWriteGameTitledata : GameEvent
{
    public EventWriteGameTitledata(IntPtr pointer) : base(pointer){}
    public EventWriteGameTitledata(bool force) : base("write_game_titledata", force){}
    // Controller id of user
    public int Controllerid
    {
        get => Get<int>("controllerId");
        set => Set<int>("controllerId", value);
    }
}
#nullable restore
