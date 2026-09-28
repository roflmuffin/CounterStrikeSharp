#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("teamplay_broadcast_audio")]
public class EventTeamplayBroadcastAudio : GameEvent
{
    public EventTeamplayBroadcastAudio(IntPtr pointer) : base(pointer){}
    public EventTeamplayBroadcastAudio(bool force) : base("teamplay_broadcast_audio", force){}
    // name of the sound to emit
    public string Sound
    {
        get => Get<string>("sound");
        set => Set<string>("sound", value);
    }
// unique team id
    public int Team
    {
        get => Get<int>("team");
        set => Set<int>("team", value);
    }
}
#nullable restore
