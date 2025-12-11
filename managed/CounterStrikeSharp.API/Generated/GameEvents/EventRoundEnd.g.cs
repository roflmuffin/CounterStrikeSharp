#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("round_end")]
public class EventRoundEnd : GameEvent
{
    public EventRoundEnd(IntPtr pointer) : base(pointer){}
    public EventRoundEnd(bool force) : base("round_end", force){}
    // server-generated legacy value
    public int Legacy
    {
        get => Get<int>("legacy");
        set => Set<int>("legacy", value);
    }
// end round message
    public string Message
    {
        get => Get<string>("message");
        set => Set<string>("message", value);
    }
// if set, don't play round end music, because action is still on-going
    public int Nomusic
    {
        get => Get<int>("nomusic");
        set => Set<int>("nomusic", value);
    }
// total number of players alive at the end of round, used for statistics gathering, computed on the server in the event client is in replay when receiving this message
    public int PlayerCount
    {
        get => Get<int>("player_count");
        set => Set<int>("player_count", value);
    }
// reson why team won
    public int Reason
    {
        get => Get<int>("reason");
        set => Set<int>("reason", value);
    }

    public float Time
    {
        get => Get<float>("time");
        set => Set<float>("time", value);
    }
// winner team/user i
    public int Winner
    {
        get => Get<int>("winner");
        set => Set<int>("winner", value);
    }
}
#nullable restore
