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
#include "core/core_bindings.h"
#include "core/globals.h"
#include "core/log.h"
#include "core/managers/server_manager.h"
#include "scripting/autonative.h"
#include "scripting/engine_feature_natives.h"

namespace counterstrikesharp {

// Each direct call on ISource2Server is its own Engine Feature: disabled, the native throws instead of calling.
static void* GetEconItemSystem(ScriptContext& scriptContext)
{
    if (!RequireEngineFeature(scriptContext, bindings::features::EconItemSystem)) return nullptr;
    return globals::serverManager.GetEconItemSystem();
}

static bool IsServerPaused(ScriptContext& scriptContext)
{
    if (!RequireEngineFeature(scriptContext, bindings::features::IsPaused)) return false;
    return globals::serverManager.IsPaused();
}

REGISTER_NATIVES(server, {
    ScriptEngine::RegisterNativeHandler("GET_ECON_ITEM_SYSTEM", GetEconItemSystem);
    ScriptEngine::RegisterNativeHandler("IS_SERVER_PAUSED", IsServerPaused);
})

} // namespace counterstrikesharp
