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

public partial class CScriptItem : CItem
{
    public CScriptItem (IntPtr pointer) : base(pointer) {}

	// m_OnPlayerPickup
	[SchemaMember("CScriptItem", "m_OnPlayerPickup")]
	public CEntityIOOutput OnPlayerPickup => Schema.GetDeclaredClass<CEntityIOOutput>(this.Handle, "CScriptItem", "m_OnPlayerPickup");

	// m_MoveTypeOverride
	[SchemaMember("CScriptItem", "m_MoveTypeOverride")]
	public ref MoveType_t MoveTypeOverride => ref Schema.GetRef<MoveType_t>(this.Handle, "CScriptItem", "m_MoveTypeOverride");

}
