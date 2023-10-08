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
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Listeners;

namespace CounterStrikeSharp.API.Modules.Engine.Trace
{
    public class SimpleTraceFilter : NativeObject
    {
        public SimpleTraceFilter(IntPtr cPtr) : base(cPtr)
        {
        }

        public SimpleTraceFilter(int indexToIgnore) : base(NativeAPI.NewSimpleTraceFilter(indexToIgnore))
        {

        }
    }

    public class TraceFilterProxy : NativeObject
    {
        private ITraceFilter _filter;
        private FunctionReference.CallbackDelegate getTypeCallback;
        private FunctionReference.CallbackDelegate shouldHitCallback;

        public TraceFilterProxy(IntPtr cPtr) : base(cPtr)
        {
        }

        public TraceFilterProxy(ITraceFilter filter) : base(NativeAPI.NewTraceFilterProxy())
        {
            _filter = filter;

            /*
            getTypeCallback = Utilities.SafeExecute(intPtr =>
            {
                var marshal = new CMarshalObject();
                marshal.PushInt((int) _filter.GetTraceType());
                return marshal.GetPointer();
            });
            */

            /*shouldHitCallback = Utilities.SafeExecute(ptr =>
            {
                var marshal = new CMarshalObject(ptr, true);
                var entity = marshal.GetValue<BaseEntity>();
                var contentMask = marshal.GetInt();

                var isValidEntity = _filter.ShouldHitEntity(entity, contentMask);

                var response = new CMarshalObject();
                response.PushInt(isValidEntity ? 1 : 0);

                return response.GetPointer();
            });*/

            unsafe
            {
                getTypeCallback = (fxScriptContext* context) =>
                {
                    var scriptContext = new ScriptContext(context);

                    scriptContext.Push(_filter.GetTraceType());
                };

                shouldHitCallback = (fxScriptContext* context) =>
                {
                    var scriptContext = new ScriptContext(context);
                    
                    var entity = new BaseEntity(scriptContext.GetArgument<int>(0));
                    var contentMask = scriptContext.GetArgument<int>(1);

                    var isValidEntity = _filter.ShouldHitEntity(entity, contentMask);

                    Console.WriteLine($"Returning {isValidEntity} to `ShouldHitEntity`");

                    scriptContext.SetResult(isValidEntity, context);
                };
            }
            

            NativeAPI.TraceFilterProxySetTraceTypeCallback(Handle, Marshal.GetFunctionPointerForDelegate(getTypeCallback));
            NativeAPI.TraceFilterProxySetShouldHitEntityCallback(Handle, Marshal.GetFunctionPointerForDelegate(shouldHitCallback));
            /*NativeAPI.TraceFilterProxySetTraceTypeCallback(Handle, getTypeCallback);
            NativePINVOKE.TraceFilterProxy_SetGetTraceTypeCallback(ptr, getTypeCallback.ToHandle());
            NativePINVOKE.TraceFilterProxy_SetShouldHitEntityCallback(ptr, shouldHitCallback.ToHandle());*/
        }
    }

    public enum TraceType
    {
        Everything = 0,
        WorldOnly,				// NOTE: This does *not* test static props!!!
        EntitiesOnly,			// NOTE: This version will *not* test static props
        EverythingFilterProps,	// NOTE: This version will pass the IHandleEntity for props through the filter, unlike all other filters
    };

    public class CustomTraceFilter : TraceFilter
    {
        private Func<BaseEntity, bool> _filter;

        public CustomTraceFilter(Func<BaseEntity, bool> filter)
        {
            _filter = filter;
        }
        public override bool ShouldHitEntity(BaseEntity entity, int contentMask)
        {
            return _filter.Invoke(entity);
        }

        public override TraceType GetTraceType()
        {
            return TraceType.Everything;
        }
    }

    public class ExclusionTraceFilter : TraceFilter
    {
        private int _indexToExclude;

        public ExclusionTraceFilter(int indexToExclude)
        {
            this._indexToExclude = indexToExclude;
        }
        public override bool ShouldHitEntity(BaseEntity entity, int contentMask)
        {
            if (entity.Index == _indexToExclude) return false;

            return true;
        }

        public override TraceType GetTraceType()
        {
            return TraceType.Everything;
        }
    }

    public abstract class TraceFilter : ITraceFilter
    {
        public abstract bool ShouldHitEntity(BaseEntity entity, int contentMask);
        public abstract TraceType GetTraceType();
    }

    public interface ITraceFilter
    {
        bool ShouldHitEntity(BaseEntity entity, int contentMask);
        TraceType GetTraceType();
    }
}