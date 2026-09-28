#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_mvp")]
public class EventRoundMvp : GameEvent
{
    public EventRoundMvp(IntPtr pointer) : base(pointer){}
    public EventRoundMvp(bool force) : base("round_mvp", force){}
    
    public long Musickitid
    {
        get => Get<long>("musickitid");
        set => Set<long>("musickitid", value);
    }

    public long Musickitmvps
    {
        get => Get<long>("musickitmvps");
        set => Set<long>("musickitmvps", value);
    }

    public int Nomusic
    {
        get => Get<int>("nomusic");
        set => Set<int>("nomusic", value);
    }

    public int Reason
    {
        get => Get<int>("reason");
        set => Set<int>("reason", value);
    }

    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }

    public long Value
    {
        get => Get<long>("value");
        set => Set<long>("value", value);
    }
}
#nullable restore
