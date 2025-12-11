#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("read_game_titledata")]
public class EventReadGameTitledata : GameEvent
{
    public EventReadGameTitledata(IntPtr pointer) : base(pointer){}
    public EventReadGameTitledata(bool force) : base("read_game_titledata", force){}
    // Controller id of user
    public int Controllerid
    {
        get => Get<int>("controllerId");
        set => Set<int>("controllerId", value);
    }
}
#nullable restore
