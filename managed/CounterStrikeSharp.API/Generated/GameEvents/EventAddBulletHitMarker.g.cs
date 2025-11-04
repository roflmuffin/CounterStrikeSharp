#nullable enable
using System;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

[EventName("add_bullet_hit_marker")]
public class EventAddBulletHitMarker : GameEvent
{
    public EventAddBulletHitMarker(IntPtr pointer) : base(pointer){}
    public EventAddBulletHitMarker(bool force) : base("add_bullet_hit_marker", force){}
    
    public virtual int AngX
    {
        get => Get<int>("ang_x");
        set => Set<int>("ang_x", value);
    }

    public virtual int AngY
    {
        get => Get<int>("ang_y");
        set => Set<int>("ang_y", value);
    }

    public virtual int AngZ
    {
        get => Get<int>("ang_z");
        set => Set<int>("ang_z", value);
    }

    public virtual int Bone
    {
        get => Get<int>("bone");
        set => Set<int>("bone", value);
    }

    public virtual bool Hit
    {
        get => Get<bool>("hit");
        set => Set<bool>("hit", value);
    }

    public virtual int PosX
    {
        get => Get<int>("pos_x");
        set => Set<int>("pos_x", value);
    }

    public virtual int PosY
    {
        get => Get<int>("pos_y");
        set => Set<int>("pos_y", value);
    }

    public virtual int PosZ
    {
        get => Get<int>("pos_z");
        set => Set<int>("pos_z", value);
    }

    public virtual int StartX
    {
        get => Get<int>("start_x");
        set => Set<int>("start_x", value);
    }

    public virtual int StartY
    {
        get => Get<int>("start_y");
        set => Set<int>("start_y", value);
    }

    public virtual int StartZ
    {
        get => Get<int>("start_z");
        set => Set<int>("start_z", value);
    }

    public virtual CCSPlayerController? Userid
    {
        get => GetPlayer("userid");
        set => SetPlayer("userid", value);
    }
}
#nullable restore
