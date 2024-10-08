// <auto-generated />
#nullable enable
#pragma warning disable CS1591

using System;
using System.Diagnostics;
using System.Drawing;
using CounterStrikeSharp;
using CounterStrikeSharp.API.Modules.Events;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core.Attributes;

namespace CounterStrikeSharp.API.Core;

public partial class AutoRoomDoorwayPairs_t : NativeObject
{
    public AutoRoomDoorwayPairs_t (IntPtr pointer) : base(pointer) {}

	// vP1
	[SchemaMember("AutoRoomDoorwayPairs_t", "vP1")]
	public Vector VP1 => Schema.GetDeclaredClass<Vector>(this.Handle, "AutoRoomDoorwayPairs_t", "vP1");

	// vP2
	[SchemaMember("AutoRoomDoorwayPairs_t", "vP2")]
	public Vector VP2 => Schema.GetDeclaredClass<Vector>(this.Handle, "AutoRoomDoorwayPairs_t", "vP2");

}
