#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("instructor_close_lesson")]
public class EventInstructorCloseLesson : GameEvent
{
    public EventInstructorCloseLesson(IntPtr pointer) : base(pointer){}
    public EventInstructorCloseLesson(bool force) : base("instructor_close_lesson", force){}
    // Name of the lesson to start.  Must match instructor_lesson.txt
    public string HintName
    {
        get => Get<string>("hint_name");
        set => Set<string>("hint_name", value);
    }
// The player who this lesson is intended for
    public CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
