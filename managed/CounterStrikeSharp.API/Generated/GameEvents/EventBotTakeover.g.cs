#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("bot_takeover")]
public class EventBotTakeover : GameEvent
{
    public EventBotTakeover(IntPtr pointer) : base(pointer){}
    public EventBotTakeover(bool force) : base("bot_takeover", force){}
    
    public CCSPlayerController? Botid
    {
        get => GetPlayer("botid");
        set => SetPlayer("botid", value);
    }

    public float P
    {
        get => Get<float>("p");
        set => Set<float>("p", value);
    }

    public float R
    {
        get => Get<float>("r");
        set => Set<float>("r", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }

    public float Y
    {
        get => Get<float>("y");
        set => Set<float>("y", value);
    }
}
#nullable restore
