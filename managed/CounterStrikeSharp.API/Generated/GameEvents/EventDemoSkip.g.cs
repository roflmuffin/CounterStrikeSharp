#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("demo_skip")]
public class EventDemoSkip : GameEvent
{
    public EventDemoSkip(IntPtr pointer) : base(pointer){}
    public EventDemoSkip(bool force) : base("demo_skip", force){}
    // CSVCMsgList_GameEvents
    public int DotaHeroChaseList
    {
        get => Get<int>("dota_hero_chase_list");
        set => Set<int>("dota_hero_chase_list", value);
    }

    public int Local
    {
        get => Get<int>("local");
        set => Set<int>("local", value);
    }
// current playback tick
    public long PlaybackTick
    {
        get => Get<long>("playback_tick");
        set => Set<long>("playback_tick", value);
    }
// tick we're going to
    public long SkiptoTick
    {
        get => Get<long>("skipto_tick");
        set => Set<long>("skipto_tick", value);
    }
// CSVCMsgList_UserMessages
    public int UserMessageList
    {
        get => Get<int>("user_message_list");
        set => Set<int>("user_message_list", value);
    }
}
#nullable restore
