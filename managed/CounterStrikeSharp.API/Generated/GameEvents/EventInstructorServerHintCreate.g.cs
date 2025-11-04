#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("instructor_server_hint_create")]
public class EventInstructorServerHintCreate : GameEvent
{
    public EventInstructorServerHintCreate(IntPtr pointer) : base(pointer){}
    public EventInstructorServerHintCreate(bool force) : base("instructor_server_hint_create", force){}
    // the hint caption that only the activator sees e.g. "#YouPushedItGood"
    public virtual string HintActivatorCaption
    {
        get => Get<string>("hint_activator_caption");
        set => Set<string>("hint_activator_caption", value);
    }
// userid id of the activator
    public virtual CCSPlayerController? HintActivatorUserid
    {
        get => GetPlayer("hint_activator_userid");
        set => SetPlayer("hint_activator_userid", value);
    }
// if false, the hint will dissappear if the target entity is invisible
    public virtual bool HintAllowNodrawTarget
    {
        get => Get<bool>("hint_allow_nodraw_target");
        set => Set<bool>("hint_allow_nodraw_target", value);
    }
// bindings to use when use_binding is the onscreen icon
    public virtual string HintBinding
    {
        get => Get<string>("hint_binding");
        set => Set<string>("hint_binding", value);
    }
// the hint caption. e.g. "#ThisIsDangerous"
    public virtual string HintCaption
    {
        get => Get<string>("hint_caption");
        set => Set<string>("hint_caption", value);
    }
// the hint color in "r,g,b" format where each component is 0-255
    public virtual string HintColor
    {
        get => Get<string>("hint_color");
        set => Set<string>("hint_color", value);
    }
// entity id of the env_instructor_hint that fired the event
    public virtual long HintEntindex
    {
        get => Get<long>("hint_entindex");
        set => Set<long>("hint_entindex", value);
    }
// hint flags
    public virtual long HintFlags
    {
        get => Get<long>("hint_flags");
        set => Set<long>("hint_flags", value);
    }
// if true, the hint caption will show even if the hint is occluded
    public virtual bool HintForcecaption
    {
        get => Get<bool>("hint_forcecaption");
        set => Set<bool>("hint_forcecaption", value);
    }
// gamepad bindings to use when use_binding is the onscreen icon
    public virtual string HintGamepadBinding
    {
        get => Get<string>("hint_gamepad_binding");
        set => Set<string>("hint_gamepad_binding", value);
    }
// the hint icon to use when the hint is offscreen. e.g. "icon_alert"
    public virtual string HintIconOffscreen
    {
        get => Get<string>("hint_icon_offscreen");
        set => Set<string>("hint_icon_offscreen", value);
    }
// how far on the z axis to offset the hint from entity origin
    public virtual float HintIconOffset
    {
        get => Get<float>("hint_icon_offset");
        set => Set<float>("hint_icon_offset", value);
    }
// the hint icon to use when the hint is onscreen. e.g. "icon_alert_red"
    public virtual string HintIconOnscreen
    {
        get => Get<string>("hint_icon_onscreen");
        set => Set<string>("hint_icon_onscreen", value);
    }
// Path for Panorama layout file
    public virtual string HintLayoutfile
    {
        get => Get<string>("hint_layoutfile");
        set => Set<string>("hint_layoutfile", value);
    }
// if true, only the local player will see the hint
    public virtual bool HintLocalPlayerOnly
    {
        get => Get<bool>("hint_local_player_only");
        set => Set<bool>("hint_local_player_only", value);
    }
// what to name the hint. For referencing it again later (e.g. a kill command for the hint instead of a timeout)
    public virtual string HintName
    {
        get => Get<string>("hint_name");
        set => Set<string>("hint_name", value);
    }
// if true, the hint will not show when outside the player view
    public virtual bool HintNooffscreen
    {
        get => Get<bool>("hint_nooffscreen");
        set => Set<bool>("hint_nooffscreen", value);
    }
// range before the hint is culled
    public virtual float HintRange
    {
        get => Get<float>("hint_range");
        set => Set<float>("hint_range", value);
    }
// type name so that messages of the same type will replace each other
    public virtual string HintReplaceKey
    {
        get => Get<string>("hint_replace_key");
        set => Set<string>("hint_replace_key", value);
    }
// Game sound to play
    public virtual string HintStartSound
    {
        get => Get<string>("hint_start_sound");
        set => Set<string>("hint_start_sound", value);
    }
// entity id that the hint should display at
    public virtual long HintTarget
    {
        get => Get<long>("hint_target");
        set => Set<long>("hint_target", value);
    }
// how long in seconds until the hint automatically times out, 0 = never
    public virtual int HintTimeout
    {
        get => Get<int>("hint_timeout");
        set => Set<int>("hint_timeout", value);
    }
// Height offset for attached panels
    public virtual float HintVrHeightOffset
    {
        get => Get<float>("hint_vr_height_offset");
        set => Set<float>("hint_vr_height_offset", value);
    }
// offset for attached panels
    public virtual float HintVrOffsetX
    {
        get => Get<float>("hint_vr_offset_x");
        set => Set<float>("hint_vr_offset_x", value);
    }
// offset for attached panels
    public virtual float HintVrOffsetY
    {
        get => Get<float>("hint_vr_offset_y");
        set => Set<float>("hint_vr_offset_y", value);
    }
// offset for attached panels
    public virtual float HintVrOffsetZ
    {
        get => Get<float>("hint_vr_offset_z");
        set => Set<float>("hint_vr_offset_z", value);
    }
// Attachment type for the Panorama panel
    public virtual int HintVrPanelType
    {
        get => Get<int>("hint_vr_panel_type");
        set => Set<int>("hint_vr_panel_type", value);
    }
// user ID of the player that triggered the hint
    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
