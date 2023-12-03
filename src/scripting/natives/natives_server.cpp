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
#include "core/globals.h"
#include "core/log.h"
#include "core/managers/server_manager.h"
#include "scripting/autonative.h"

namespace counterstrikesharp {

static void *GetEconItemSystem(ScriptContext& scriptContext) {
    return globals::serverManager.GetEconItemSystem();
}

static bool IsServerPaused(ScriptContext& scriptContext)
{
    return globals::serverManager.IsPaused();
}

REGISTER_NATIVES(server, {
    ScriptEngine::RegisterNativeHandler("GET_ECON_ITEM_SYSTEM", GetEconItemSystem);
    ScriptEngine::RegisterNativeHandler("IS_SERVER_PAUSED", IsServerPaused);
})

}