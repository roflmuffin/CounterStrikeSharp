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

public partial class CEnvCombinedLightProbeVolume : CBaseEntity
{
    public CEnvCombinedLightProbeVolume (IntPtr pointer) : base(pointer) {}

	// m_Color
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_Color")]
	public Color Color
	{
		get { return Schema.GetCustomMarshalledType<Color>(this.Handle, "CEnvCombinedLightProbeVolume", "m_Color"); }
		set { Schema.SetCustomMarshalledType<Color>(this.Handle, "CEnvCombinedLightProbeVolume", "m_Color", value); }
	}

	// m_flBrightness
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_flBrightness")]
	public ref float Brightness => ref Schema.GetRef<float>(this.Handle, "CEnvCombinedLightProbeVolume", "m_flBrightness");

	// m_hCubemapTexture
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_hCubemapTexture")]
	public CStrongHandle<InfoForResourceTypeCTextureBase> CubemapTexture => Schema.GetDeclaredClass<CStrongHandle<InfoForResourceTypeCTextureBase>>(this.Handle, "CEnvCombinedLightProbeVolume", "m_hCubemapTexture");

	// m_bCustomCubemapTexture
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_bCustomCubemapTexture")]
	public ref bool CustomCubemapTexture => ref Schema.GetRef<bool>(this.Handle, "CEnvCombinedLightProbeVolume", "m_bCustomCubemapTexture");

	// m_hLightProbeTexture
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_hLightProbeTexture")]
	public CStrongHandle<InfoForResourceTypeCTextureBase> LightProbeTexture => Schema.GetDeclaredClass<CStrongHandle<InfoForResourceTypeCTextureBase>>(this.Handle, "CEnvCombinedLightProbeVolume", "m_hLightProbeTexture");

	// m_hLightProbeDirectLightIndicesTexture
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_hLightProbeDirectLightIndicesTexture")]
	public CStrongHandle<InfoForResourceTypeCTextureBase> LightProbeDirectLightIndicesTexture => Schema.GetDeclaredClass<CStrongHandle<InfoForResourceTypeCTextureBase>>(this.Handle, "CEnvCombinedLightProbeVolume", "m_hLightProbeDirectLightIndicesTexture");

	// m_hLightProbeDirectLightScalarsTexture
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_hLightProbeDirectLightScalarsTexture")]
	public CStrongHandle<InfoForResourceTypeCTextureBase> LightProbeDirectLightScalarsTexture => Schema.GetDeclaredClass<CStrongHandle<InfoForResourceTypeCTextureBase>>(this.Handle, "CEnvCombinedLightProbeVolume", "m_hLightProbeDirectLightScalarsTexture");

	// m_hLightProbeDirectLightShadowsTexture
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_hLightProbeDirectLightShadowsTexture")]
	public CStrongHandle<InfoForResourceTypeCTextureBase> LightProbeDirectLightShadowsTexture => Schema.GetDeclaredClass<CStrongHandle<InfoForResourceTypeCTextureBase>>(this.Handle, "CEnvCombinedLightProbeVolume", "m_hLightProbeDirectLightShadowsTexture");

	// m_vBoxMins
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_vBoxMins")]
	public Vector BoxMins => Schema.GetDeclaredClass<Vector>(this.Handle, "CEnvCombinedLightProbeVolume", "m_vBoxMins");

	// m_vBoxMaxs
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_vBoxMaxs")]
	public Vector BoxMaxs => Schema.GetDeclaredClass<Vector>(this.Handle, "CEnvCombinedLightProbeVolume", "m_vBoxMaxs");

	// m_bMoveable
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_bMoveable")]
	public ref bool Moveable => ref Schema.GetRef<bool>(this.Handle, "CEnvCombinedLightProbeVolume", "m_bMoveable");

	// m_nHandshake
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_nHandshake")]
	public ref Int32 Handshake => ref Schema.GetRef<Int32>(this.Handle, "CEnvCombinedLightProbeVolume", "m_nHandshake");

	// m_nEnvCubeMapArrayIndex
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_nEnvCubeMapArrayIndex")]
	public ref Int32 EnvCubeMapArrayIndex => ref Schema.GetRef<Int32>(this.Handle, "CEnvCombinedLightProbeVolume", "m_nEnvCubeMapArrayIndex");

	// m_nPriority
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_nPriority")]
	public ref Int32 Priority => ref Schema.GetRef<Int32>(this.Handle, "CEnvCombinedLightProbeVolume", "m_nPriority");

	// m_bStartDisabled
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_bStartDisabled")]
	public ref bool StartDisabled => ref Schema.GetRef<bool>(this.Handle, "CEnvCombinedLightProbeVolume", "m_bStartDisabled");

	// m_flEdgeFadeDist
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_flEdgeFadeDist")]
	public ref float EdgeFadeDist => ref Schema.GetRef<float>(this.Handle, "CEnvCombinedLightProbeVolume", "m_flEdgeFadeDist");

	// m_vEdgeFadeDists
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_vEdgeFadeDists")]
	public Vector EdgeFadeDists => Schema.GetDeclaredClass<Vector>(this.Handle, "CEnvCombinedLightProbeVolume", "m_vEdgeFadeDists");

	// m_nLightProbeSizeX
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_nLightProbeSizeX")]
	public ref Int32 LightProbeSizeX => ref Schema.GetRef<Int32>(this.Handle, "CEnvCombinedLightProbeVolume", "m_nLightProbeSizeX");

	// m_nLightProbeSizeY
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_nLightProbeSizeY")]
	public ref Int32 LightProbeSizeY => ref Schema.GetRef<Int32>(this.Handle, "CEnvCombinedLightProbeVolume", "m_nLightProbeSizeY");

	// m_nLightProbeSizeZ
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_nLightProbeSizeZ")]
	public ref Int32 LightProbeSizeZ => ref Schema.GetRef<Int32>(this.Handle, "CEnvCombinedLightProbeVolume", "m_nLightProbeSizeZ");

	// m_nLightProbeAtlasX
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_nLightProbeAtlasX")]
	public ref Int32 LightProbeAtlasX => ref Schema.GetRef<Int32>(this.Handle, "CEnvCombinedLightProbeVolume", "m_nLightProbeAtlasX");

	// m_nLightProbeAtlasY
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_nLightProbeAtlasY")]
	public ref Int32 LightProbeAtlasY => ref Schema.GetRef<Int32>(this.Handle, "CEnvCombinedLightProbeVolume", "m_nLightProbeAtlasY");

	// m_nLightProbeAtlasZ
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_nLightProbeAtlasZ")]
	public ref Int32 LightProbeAtlasZ => ref Schema.GetRef<Int32>(this.Handle, "CEnvCombinedLightProbeVolume", "m_nLightProbeAtlasZ");

	// m_bEnabled
	[SchemaMember("CEnvCombinedLightProbeVolume", "m_bEnabled")]
	public ref bool Enabled => ref Schema.GetRef<bool>(this.Handle, "CEnvCombinedLightProbeVolume", "m_bEnabled");

}
