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

#pragma once

#include <public/engine/IEngineTrace.h>

#include "scripting/callback_manager.h"

namespace counterstrikesharp {
class TraceFilterProxy : public ITraceFilter {
public:
    TraceFilterProxy() {}
    bool ShouldHitEntity(IHandleEntity *pServerEntity, int contentsMask);
    TraceType_t GetTraceType() const;

    void SetShouldHitEntityCallback(CallbackT cb);
    void SetGetTraceTypeCallback(CallbackT cb);

private:
    CallbackT m_cb_should_hit_entity;
    CallbackT m_cb_get_trace_type;
};

class CSimpleTraceFilter : public ITraceFilter {
public:
    CSimpleTraceFilter(int index)
        : m_index_to_exclude(index) {}
    bool ShouldHitEntity(IHandleEntity *pServerEntity, int contentsMask);

    TraceType_t GetTraceType() const { return TRACE_EVERYTHING; }

private:
    int m_index_to_exclude = -1;
};

enum RayType { RayType_EndPoint, RayType_Infinite };

}  // namespace counterstrikesharp