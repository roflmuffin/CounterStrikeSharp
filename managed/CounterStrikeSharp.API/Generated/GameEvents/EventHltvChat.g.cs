#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("hltv_chat")]
public class EventHltvChat : GameEvent
{
    public EventHltvChat(IntPtr pointer) : base(pointer){}
    public EventHltvChat(bool force) : base("hltv_chat", force){}
    // steam id
    public ulong Steamid
    {
        get => Get<ulong>("steamID");
        set => Set<ulong>("steamID", value);
    }

    public string Text
    {
        get => Get<string>("text");
        set => Set<string>("text", value);
    }
}
#nullable restore
