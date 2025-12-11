#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("difficulty_changed")]
public class EventDifficultyChanged : GameEvent
{
    public EventDifficultyChanged(IntPtr pointer) : base(pointer){}
    public EventDifficultyChanged(bool force) : base("difficulty_changed", force){}
    
    public int Newdifficulty
    {
        get => Get<int>("newDifficulty");
        set => Set<int>("newDifficulty", value);
    }

    public int Olddifficulty
    {
        get => Get<int>("oldDifficulty");
        set => Set<int>("oldDifficulty", value);
    }
// new difficulty as string
    public string Strdifficulty
    {
        get => Get<string>("strDifficulty");
        set => Set<string>("strDifficulty", value);
    }
}
#nullable restore
