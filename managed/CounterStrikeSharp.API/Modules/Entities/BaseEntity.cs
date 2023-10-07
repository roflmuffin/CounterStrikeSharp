/*
 *  This file is part of CounterStrikeSharp.
 *  CounterStrikeSharp is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  CounterStrikeSharp is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
 */

using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using Vector = CounterStrikeSharp.API.Modules.Utils.Vector;

namespace CounterStrikeSharp.API.Modules.Entities
{
    public class BaseEntity : NativeObject
    {
        public BaseEntity(int index) : base(NativeAPI.BaseentityFromIndex(index))
        {
            _index = index;
        }

        public BaseEntity(IntPtr pointer) : base(pointer)
        {
            _index = NativeAPI.IndexFromBaseentity(pointer);
        }

        public override string ToString()
        {
            return string.Format("{0}", Index);
        }

        #region Props
        public int GetProp(PropType type, string name) => NativeAPI.EntityGetPropInt(Index, (int) type, name);

        public T GetProp<T>(PropType type, string name) => (T) (NativeAPI.EntityGetPropInt(Index, (int) type, name) as object);

        public void SetProp(PropType type, string name, int value) => NativeAPI.EntitySetPropInt(Index, (int)type, name, value);

        public void SetProp<T>(PropType type, string name, T value) => NativeAPI.EntitySetPropInt(Index, (int)type, name, (int)(object)value);

        public bool GetPropBool(PropType type, string name) => GetProp(type, name) == 1;

        public void SetPropBool(PropType type, string name, bool value) => SetProp(type, name, value ? 1 : 0);

        public float GetPropFloat(PropType type, string name) => NativeAPI.EntityGetPropFloat(Index, (int) type, name);

        public void SetPropFloat(PropType type, string name, float value) => NativeAPI.EntitySetPropFloat(Index, (int)type, name, value);

        public Vector GetPropVector(PropType type, string name) => new(NativeAPI.EntityGetPropVector(Index, (int) type, name));

        public void SetPropVector(PropType type, string name, Vector vector) => NativeAPI.EntitySetPropVector(Index, (int) type, name, vector.Handle);

        public string GetPropString(PropType type, string name) => NativeAPI.EntityGetPropString(Index, (int)type, name);

        public void SetPropString(PropType type, string name, string value) => NativeAPI.EntitySetPropString(Index, (int)type, name, value);

        public BaseEntity GetPropEnt(PropType type, string name)
        {
            var returnVal = NativeAPI.EntityGetPropEnt(Index, (int) type, name);
            if (returnVal < 0) return null;
            return BaseEntity.FromIndex(returnVal);
        }

        public BaseEntity GetPropEntByOffset(int offset)
        {
            var returnVal = NativeAPI.EntityGetPropEntByOffset(Index, offset);
            if (returnVal < 0) return null;
            return BaseEntity.FromIndex(returnVal);
        }

        public void SetPropEnt(PropType type, string name, int index) => NativeAPI.EntitySetPropEnt(Index, (int) type, name, index);

        #endregion

        #region KeyValues
        public string GetKeyValue(string name) => NativeAPI.EntityGetKeyvalue(Index, name);

        public Vector GetKeyValueVector(string name)
        {
            var values = GetKeyValue(name).Split(new []{" "}, StringSplitOptions.None).Select(float.Parse).ToArray();
            return new Vector(values[0], values[1], values[2]);
        }

        public float GetKeyValueFloat(string name)
        {
            return Convert.ToSingle(GetKeyValue(name));
        }

        public void SetKeyValue(string name, string value) => NativeAPI.EntitySetKeyvalue(Index, name, value);

        public void SetKeyValueFloat(string name, float value) => SetKeyValue(name, value.ToString());

        public void SetKeyValueVector(string name, Vector vec)
        {
            string strValue = $"{vec.X} {vec.Y} {vec.Z}";
            NativeAPI.EntitySetKeyvalue(Index, name, strValue);
        }

        #endregion

        public bool IsPlayer => NativeAPI.EntityIsPlayer(Index);
        public bool IsWeapon => NativeAPI.EntityIsWeapon(Index);

        public bool IsNetworked => NativeAPI.EntityIsNetworked(Index);
        public bool IsValid => NativeAPI.EntityIsValid(Index);

        /*public bool IsPlayer => NativePINVOKE.CBaseEntityWrapper_IsPlayer(ptr);
        public bool IsWeapon => NativePINVOKE.CBaseEntityWrapper_IsWeapon(ptr);
        public bool IsMoving => NativePINVOKE.CBaseEntityWrapper_IsMoving(ptr);*/


        public Vector Origin
        {
            get => GetKeyValueVector("origin");
            set => SetKeyValueVector("origin", value);
        }
        

        public Vector Maxs
        {
            get => GetPropVector(PropType.Send, "m_Collision.m_vecMaxs");
            set => SetPropVector(PropType.Send, "m_Collision.m_vecMaxs", value);
        }

        public Vector Mins
        {
            get => GetPropVector(PropType.Send, "m_Collision.m_vecMins");
            set => SetPropVector(PropType.Send, "m_Collision.m_vecMins", value);
        }

        public int EntityFlags
        {
            get => GetProp(PropType.Data, "m_iEFlags");
            set => SetProp(PropType.Data, "m_iEFlags", value);
        }

        public SolidType SolidType
        {
            get => (SolidType)GetProp(PropType.Send, "m_Collision.m_nSolidType");
            set => SetProp(PropType.Send, "m_Collision.m_nSolidType", (int)value);
        }

        public SolidFlags SolidFlags
        {
            get => GetProp<SolidFlags>(PropType.Send, "m_Collision.m_usSolidFlags");
            set => SetProp(PropType.Send, "m_Collision.m_usSolidFlags", value);
        }

        public CollisionGroup CollisionGroup
        {
            get => GetProp<CollisionGroup>(PropType.Send, "m_CollisionGroup");
            set => SetProp(PropType.Send, "m_CollisionGroup", value);
        }

        // TODO: ENTITY RENDER COLOR
        public Color Color
        {
            get
            {
                int offset = NativeAPI.FindDatamapInfo(Index, "m_clrRender");

                int r = NativeAPI.EntityGetProp(Index, offset + 0, 8);
                int g = NativeAPI.EntityGetProp(Index, offset + 1, 8);
                int b = NativeAPI.EntityGetProp(Index, offset + 2, 8);
                int a = NativeAPI.EntityGetProp(Index, offset + 3, 8);
                return Color.FromArgb(a, r, g, b);
            }
            set
            {
                int offset = NativeAPI.FindDatamapInfo(Index, "m_clrRender");

                NativeAPI.EntitySetProp(Index, offset + 0, 8, value.R);
                NativeAPI.EntitySetProp(Index, offset + 1, 8, value.G);
                NativeAPI.EntitySetProp(Index, offset + 2, 8, value.B);
                NativeAPI.EntitySetProp(Index, offset + 3, 8, value.A);
            }
        }

        public float Elasticity
        {
            get => GetPropFloat(PropType.Send, "m_flElasticity");
            set => SetPropFloat(PropType.Send, "m_flElasticity", value);
        }

        public BaseEntity GroundEntity
        {
            get => GetPropEnt(PropType.Data, "m_hGroundEntity");
            set => SetPropEnt(PropType.Data, "m_hGroundEntity", value.Index);
        }

        public Team Team
        {
            get => GetProp<Team>(PropType.Send, "m_iTeamNum");
            set => SetProp(PropType.Send, "m_iTeamNum", value);
        }

        public RenderFx RenderFx
        {
            get => (RenderFx)GetProp(PropType.Send, "m_nRenderFX");
            set => SetProp(PropType.Send, "m_nRenderFX", (int)value);
        }

        public RenderMode RenderMode
        {
            get => (RenderMode)GetProp(PropType.Send, "m_nRenderMode");
            set => SetProp(PropType.Send, "m_nRenderMode", (int)value);
        }

        public MoveType MoveType
        {
            get => (MoveType)GetProp(PropType.Send, "movetype");
            set => SetProp(PropType.Send, "movetype", (int)value);
        }

        public new IntPtr Handle => NativeAPI.BaseentityFromIndex(Index);

        public int ParentHandle
        {
            get
            {
                try
                {
                    return GetProp(PropType.Data, "m_pParent");
                }
                catch (Exception)
                {
                    return -1;
                }
            }
            set => SetProp(PropType.Data, "m_pParent", value);
        }

        public Vector Angles
        {
            get => GetKeyValueVector("angles");
            set => SetKeyValueVector("angles", value);
        }

        public string TargetName
        {
            get => GetKeyValue("targetname");
            set => SetKeyValue("targetname", value);
        }

        // TODO: Entity Owner Handle

        public Vector AngVelocity
        {
            get => GetPropVector(PropType.Data, "m_vecAngVelocity");
            set => SetPropVector(PropType.Data, "m_vecAngVelocity", value);
        }

        public Vector BaseVelocity
        {
            get => GetPropVector(PropType.Data, "m_vecBaseVelocity");
            set => SetPropVector(PropType.Data, "m_vecBaseVelocity", value);
        }

        public string DamageFilter
        {
            get => GetKeyValue("damagefilter");
            set => SetKeyValue("damagefilter", value);
        }

        public int Effects
        {
            get => GetProp(PropType.Data, "m_fEffects");
            set => SetProp(PropType.Data, "m_fEffects", value);
        }

        public float Friction
        {
            get => GetPropFloat(PropType.Data, "m_flFriction");
            set => SetPropFloat(PropType.Data, "m_flFriction", value);
        }

        public string GlobalName
        {
            get => GetKeyValue("globalname");
            set => SetKeyValue("globalname", value);
        }

        public float Gravity
        {
            get => GetPropFloat(PropType.Data, "m_flGravity");
            set => SetPropFloat(PropType.Data, "m_flGravity", value);
        }

        public int HammerId
        {
            get => GetProp(PropType.Data, "m_iHammerID");
            set => SetProp(PropType.Data, "m_iHammerID", value);
        }
        public int Health
        {
            get => GetProp(PropType.Data, "m_iHealth");
            set => SetProp(PropType.Data, "m_iHealth", value);
        }

        public float LocalTime
        {
            get => GetPropFloat(PropType.Data, "m_flLocalTime");
            set => SetPropFloat(PropType.Data, "m_flLocalTime", value);
        }

        public int MaxHealth
        {
            get => GetProp(PropType.Data, "m_iMaxHealth");
            set => SetProp(PropType.Data, "m_iMaxHealth", value);
        }

        public string ParentName
        {
            get => GetKeyValue("parentname");
            set => SetKeyValue("parentname", value);
        }

        public float ShadowCastDistance
        {
            get => GetPropFloat(PropType.Send, "m_flShadowCastDistance");
            set => SetPropFloat(PropType.Send, "m_flShadowCastDistance", value);
        }
        public int SpawnFlags
        {
            get => GetProp(PropType.Data, "m_spawnflags");
            set => SetProp(PropType.Data, "m_spawnflags", value);
        }

        public float Speed
        {
            get
            {
                try
                {
                    return GetPropFloat(PropType.Data, "m_flLaggedMovementValue");
                }
                catch (Exception)
                {
                    return GetKeyValueFloat("speed");
                }
            }

            set
            {
                try
                {
                    SetPropFloat(PropType.Data, "m_flLaggedMovementValue", value);
                }
                catch (Exception)
                {
                    SetKeyValueFloat("speed", value);
                }
            }
        }

        public string Target
        {
            get => GetKeyValue("target");
            set => SetKeyValue("target", value);
        }

        public Vector Velocity
        {
            get => GetPropVector(PropType.Data, "m_vecVelocity");
            set => SetPropVector(PropType.Data, "m_vecVelocity", value);
        }

        public int WaterLevel
        {
            get => GetProp(PropType.Data, "m_nWaterLevel");
            set => SetProp(PropType.Data, "m_nWaterLevel", value);
        }

        public Vector Rotation
        {
            get => GetPropVector(PropType.Data, "m_angRotation");
            set => SetPropVector(PropType.Data, "m_angRotation", value);
        }

        private int? _index;
        public int Index
        {
            get
            {
                return _index.Value;
            }
        }

        public string ClassName => NativeAPI.EntityGetClassname(Index);

        public Vector AbsVelocity
        {
            get => GetPropVector(PropType.Data, "m_vecAbsVelocity");
            set => SetPropVector(PropType.Data, "m_vecAbsVelocity", value);
        }

        public Vector AbsOrigin
        {
            get => GetPropVector(PropType.Data, "m_vecAbsOrigin");
            set => SetPropVector(PropType.Data, "m_vecAbsOrigin", value);
        }

        public void Spawn() => NativeAPI.EntitySpawn(Index);

        public void AcceptInput(string name) => NativeAPI.AcceptInput(Index, name);

        public static BaseEntity Create(string className)
        {
            var index = NativeAPI.EntityCreateByClassname(className);
            if (index < 0) return null;
            return new BaseEntity(index);
        }

        public static BaseEntity FromIndex(int index)
        {
            if (index < 0) return null;
            var entity = new BaseEntity(index);
            if (!entity.IsValid) return null;
            return entity;
        }

        public static BaseEntity FindByClassname(int startIndex, string className)
        {
            var index = NativeAPI.EntityFindByClassname(startIndex, className);
            if (index < 0) return null;
            return new BaseEntity(index);
        }

        public static BaseEntity FindByNetClass(int startIndex, string className)
        {
            var index = NativeAPI.EntityFindByNetclass(startIndex, className);
            if (index < 0) return null;
            return new BaseEntity(index);
        }
    }
}