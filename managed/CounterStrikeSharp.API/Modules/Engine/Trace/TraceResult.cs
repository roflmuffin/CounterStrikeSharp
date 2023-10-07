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
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Engine.Trace
{
    public class TraceResult : NativeObject
    {
        public TraceResult(IntPtr cPtr) : base(cPtr)
        {
        }

        public TraceResult() : base(NativeAPI.NewTraceResult())
        {

        }

        public bool DidHit()
        {
            return NativeAPI.TraceDidHit(Handle);
        }

        public BaseEntity Entity
        {
            get
            {
                var entity = new BaseEntity(NativeAPI.TraceResultEntity(Handle));
                if (entity?.IsNetworked == true)
                {
                    return entity;
                }

                return null;
            }
        }

        /*public TraceSurface Surface => NativePINVOKE.CGameTrace_surface_get(ptr).ToObject<TraceSurface>();
        public int Hitbox => NativePINVOKE.CGameTrace_hitbox_get(ptr);
        public int Hitgroup => NativePINVOKE.CGameTrace_hitgroup_get(ptr);
        public float FractionLeftSolid => NativePINVOKE.CGameTrace_fractionleftsolid_get(ptr);
        public int PhysicsBone => NativePINVOKE.CGameTrace_physicsbone_get(ptr);*/

        /*public Vector StartPosition => NativePINVOKE.CBaseTrace_startpos_get(ptr).ToObject<Vector>();
        public Vector EndPosition => NativePINVOKE.CBaseTrace_endpos_get(ptr).ToObject<Vector>();*/
    }
}