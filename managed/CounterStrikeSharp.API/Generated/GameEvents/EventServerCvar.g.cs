#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("server_cvar")]
public class EventServerCvar : GameEvent
{
    public EventServerCvar(IntPtr pointer) : base(pointer){}
    public EventServerCvar(bool force) : base("server_cvar", force){}
    // cvar name, eg "mp_roundtime"
    public string Cvarname
    {
        get => Get<string>("cvarname");
        set => Set<string>("cvarname", value);
    }
// new cvar value
    public string Cvarvalue
    {
        get => Get<string>("cvarvalue");
        set => Set<string>("cvarvalue", value);
    }
}
#nullable restore
