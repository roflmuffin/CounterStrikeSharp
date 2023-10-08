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
using CounterStrikeSharp.API.Modules.Utils;

namespace CounterStrikeSharp.API.Modules.Engine.Trace
{
    public class TraceEngine
    {
        public static IntPtr CreateRay(RayType rayType, Vector vec1, Vector vec2)
        {
            return NativeAPI.CreateRay1((int) rayType, vec1.Handle, vec2.Handle);
        }

        public static IntPtr CreateRay(Vector vec1, Vector vec2, Vector vec3, Vector vec4)
        {
            return NativeAPI.CreateRay2(vec1.Handle, vec2.Handle, vec3.Handle, vec4.Handle);
        }

        public static TraceResult TraceRay(IntPtr ray, uint mask, ITraceFilter filter)
        {
            var tr = new TraceResult();
            var proxy = new TraceFilterProxy(filter);
            NativeAPI.TraceRay(ray, tr.Handle, proxy.Handle, mask);
            return tr;
        }
    }
}