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

public partial class CTriggerSave : CBaseTrigger
{
    public CTriggerSave (IntPtr pointer) : base(pointer) {}

	// m_bForceNewLevelUnit
	[SchemaMember("CTriggerSave", "m_bForceNewLevelUnit")]
	public ref bool ForceNewLevelUnit => ref Schema.GetRef<bool>(this.Handle, "CTriggerSave", "m_bForceNewLevelUnit");

	// m_fDangerousTimer
	[SchemaMember("CTriggerSave", "m_fDangerousTimer")]
	public ref float DangerousTimer => ref Schema.GetRef<float>(this.Handle, "CTriggerSave", "m_fDangerousTimer");

	// m_minHitPoints
	[SchemaMember("CTriggerSave", "m_minHitPoints")]
	public ref Int32 MinHitPoints => ref Schema.GetRef<Int32>(this.Handle, "CTriggerSave", "m_minHitPoints");

}