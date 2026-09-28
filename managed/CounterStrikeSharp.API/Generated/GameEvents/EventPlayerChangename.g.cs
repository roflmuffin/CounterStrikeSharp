#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("player_changename")]
public class EventPlayerChangename : GameEvent
{
    public EventPlayerChangename(IntPtr pointer) : base(pointer){}
    public EventPlayerChangename(bool force) : base("player_changename", force){}
    // players new name
    public string Newname
    {
        get => Get<string>("newname");
        set => Set<string>("newname", value);
    }
// players old (current) name
    public string Oldname
    {
        get => Get<string>("oldname");
        set => Set<string>("oldname", value);
    }
// user ID on server
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
