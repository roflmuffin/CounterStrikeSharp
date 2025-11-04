#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("instructor_start_lesson")]
public class EventInstructorStartLesson : GameEvent
{
    public EventInstructorStartLesson(IntPtr pointer) : base(pointer){}
    public EventInstructorStartLesson(bool force) : base("instructor_start_lesson", force){}
    // Name of the lesson to start.  Must match instructor_lesson.txt
    public virtual string HintName
    {
        get => Get<string>("hint_name");
        set => Set<string>("hint_name", value);
    }
// entity id that the hint should display at. Leave empty if controller target
    public virtual long HintTarget
    {
        get => Get<long>("hint_target");
        set => Set<long>("hint_target", value);
    }
// The player who this lesson is intended for
    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }

    public virtual int VrControllerType
    {
        get => Get<int>("vr_controller_type");
        set => Set<int>("vr_controller_type", value);
    }

    public virtual int VrMovementType
    {
        get => Get<int>("vr_movement_type");
        set => Set<int>("vr_movement_type", value);
    }

    public virtual bool VrSingleController
    {
        get => Get<bool>("vr_single_controller");
        set => Set<bool>("vr_single_controller", value);
    }
}
#nullable restore
