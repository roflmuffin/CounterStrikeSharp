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

#include "core/engine_trace.h"

#include "core/log.h"

namespace counterstrikesharp {

CTraceFilterHitAll g_HitAllFilter;

bool CSimpleTraceFilter::ShouldHitEntity(IHandleEntity *pServerEntity, int contentsMask) {
    //    int index = ExcIndexFromBaseHandle(pServerEntity->GetRefEHandle());
    //    if (index == m_index_to_exclude)
    //        return false;

    return true;
}

TraceType_t TraceFilterProxy::GetTraceType() const {
    auto nativeContext = fxNativeContext{};
    auto scriptContext = ScriptContextRaw(nativeContext);

    m_cb_get_trace_type(&nativeContext);

    return scriptContext.GetResult<TraceType_t>();
}

bool TraceFilterProxy::ShouldHitEntity(IHandleEntity *pServerEntity, int contentsMask) {
    return true;
    //    auto entity = ExcIndexFromBaseHandle(pServerEntity->GetRefEHandle());
    //    if (entity < 0)
    //        return false;
    //
    //    auto nativeContext = fxNativeContext{};
    //    auto scriptContext = ScriptContextRaw(nativeContext);
    //
    //    scriptContext.Push(entity);
    //    scriptContext.Push<int>(0);
    //
    //    m_cb_should_hit_entity(&nativeContext);
    //
    //    auto result = scriptContext.GetResult<bool>();
    //
    //    CSSHARP_CORE_INFO("Received result {0} from `ShouldHitEntity`", result);
    //
    //    return result;
    /*return result;
    return true;*/
}

void TraceFilterProxy::SetShouldHitEntityCallback(CallbackT cb) { m_cb_should_hit_entity = cb; }

void TraceFilterProxy::SetGetTraceTypeCallback(CallbackT cb) { m_cb_get_trace_type = cb; }

}  // namespace counterstrikesharp