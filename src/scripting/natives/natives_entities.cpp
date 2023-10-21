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

#include <ios>
#include <sstream>

#include "scripting/autonative.h"
#include "scripting/script_engine.h"
#include "core/memory.h"
#include "core/log.h"
#include <public/entity2/entitysystem.h>

namespace counterstrikesharp {

CBaseEntity* GetEntityFromIndex(ScriptContext& script_context) {
    auto entityIndex = script_context.GetArgument<int>(0);

    return globals::entitySystem->GetBaseEntity(CEntityIndex(entityIndex));
}

int GetUserIdFromIndex(ScriptContext& scriptContext) {
    auto entityIndex = scriptContext.GetArgument<int>(0);

    // CPlayerSlot is 1 less than index
    return globals::engine->GetPlayerUserId(CPlayerSlot(entityIndex - 1)).Get();
}

const char* GetDesignerName(ScriptContext& scriptContext) {
    auto entity = scriptContext.GetArgument<CBaseEntity*>(0);
    return entity->GetClassname();
}

void* GetEntityPointerFromHandle(ScriptContext& scriptContext) {
    auto handle = scriptContext.GetArgument<CEntityHandle*>(0);

    if (!handle->IsValid()) {
        return nullptr;
    }

    return globals::entitySystem->GetBaseEntity(*handle);
}

void PrintToConsole(ScriptContext& scriptContext) {
    auto index = scriptContext.GetArgument<int>(0);
    auto message = scriptContext.GetArgument<const char*>(1);

    globals::engine->ClientPrintf(CPlayerSlot{index - 1}, message);
}

CEntityIdentity* GetFirstActiveEntity(ScriptContext& script_context) {
    return globals::entitySystem->m_EntityList.m_pFirstActiveEntity;
}

REGISTER_NATIVES(entities, {
    ScriptEngine::RegisterNativeHandler("GET_ENTITY_FROM_INDEX", GetEntityFromIndex);
    ScriptEngine::RegisterNativeHandler("GET_USERID_FROM_INDEX", GetUserIdFromIndex);
    ScriptEngine::RegisterNativeHandler("GET_DESIGNER_NAME", GetDesignerName);
    ScriptEngine::RegisterNativeHandler("GET_ENTITY_POINTER_FROM_HANDLE",
                                        GetEntityPointerFromHandle);
    ScriptEngine::RegisterNativeHandler("PRINT_TO_CONSOLE", PrintToConsole);
    ScriptEngine::RegisterNativeHandler("GET_FIRST_ACTIVE_ENTITY", GetFirstActiveEntity);
})
}  // namespace counterstrikesharp