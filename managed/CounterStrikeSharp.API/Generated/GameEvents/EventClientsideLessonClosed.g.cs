#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("clientside_lesson_closed")]
public class EventClientsideLessonClosed : GameEvent
{
    public EventClientsideLessonClosed(IntPtr pointer) : base(pointer){}
    public EventClientsideLessonClosed(bool force) : base("clientside_lesson_closed", force){}
    
    public string LessonName
    {
        get => Get<string>("lesson_name");
        set => Set<string>("lesson_name", value);
    }
}
#nullable restore
