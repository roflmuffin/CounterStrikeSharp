/*
 *  This file is part of VSP.NET.
 *  VSP.NET is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  VSP.NET is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with VSP.NET.  If not, see <https://www.gnu.org/licenses/>. *
 */

#include <eiface.h>

#include "core/managers/client_command_manager.h"
#include "scripting/autonative.h"
#include "scripting/callback_manager.h"
#include "core/managers/con_command_manager.h"
#include "core/managers/player_manager.h"
#include "scripting/script_engine.h"
#include "core/log.h"

namespace counterstrikesharp {

static ConCommandInfo* AddCommand(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);
    auto description = script_context.GetArgument<const char*>(1);
    auto server_only = script_context.GetArgument<bool>(2);
    auto flags = script_context.GetArgument<int>(3);
    auto callback = script_context.GetArgument<CallbackT>(4);

    CSSHARP_CORE_TRACE("Adding command {}, {}, {}, {}, {}", name, description, server_only, flags,
                       (void*)callback);

    if (globals::conCommandManager.FindCommand(name)) {
        script_context.ThrowNativeError("Failed to add command \"%s\", command already exists.", name);
        return nullptr;
    }

    return globals::conCommandManager.AddCommand(name, description, server_only, flags, callback);
}

static void RemoveCommand(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);

    globals::conCommandManager.RemoveCommand(name, callback);
}

static void AddCommandListener(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);
    auto post = script_context.GetArgument<bool>(2);

    globals::clientCommandManager.AddCommandListener(name, callback, post);
}

static void RemoveCommandListener(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);
    auto post = script_context.GetArgument<bool>(2);

    globals::clientCommandManager.RemoveCommandListener(name, callback, post);
}

static int CommandGetArgCount(ScriptContext& script_context)
{
    auto command = script_context.GetArgument<CCommand*>(0);

    if (!command) {
        script_context.ThrowNativeError("Invalid command.");
        return -1;
    }

    return command->ArgC();
}

static const char* CommandGetArgString(ScriptContext& script_context)
{
    auto command = script_context.GetArgument<CCommand*>(0);

    if (!command) {
        script_context.ThrowNativeError("Invalid command.");
        return nullptr;
    }

    return command->ArgS();
}

static const char* CommandGetCommandString(ScriptContext& script_context)
{
    auto* command = script_context.GetArgument<CCommand*>(0);

    if (!command) {
        script_context.ThrowNativeError("Invalid command.");
        return nullptr;
    }

    return command->GetCommandString();
}

static const char* CommandGetArgByIndex(ScriptContext& script_context)
{
    auto* command = script_context.GetArgument<CCommand*>(0);
    auto index = script_context.GetArgument<int>(1);

    if (!command) {
        script_context.ThrowNativeError("Invalid command.");
        return nullptr;
    }

    return command->Arg(index);
}

static void IssueClientCommand(ScriptContext& script_context)
{
    auto entity_index = script_context.GetArgument<int>(0);
    auto command = script_context.GetArgument<const char*>(1);

    globals::engine->ClientCommand(CPlayerSlot(entity_index), command);
}

ConVar* FindConVar(ScriptContext& script_context)
{
    auto name = script_context.GetArgument<const char*>(0);
    auto hCvarHandle = globals::cvars->FindConVar(name, true);

    if (!hCvarHandle.IsValid()) {
        return nullptr;
    }

    return globals::cvars->GetConVar(hCvarHandle);
}

void SetConVarStringValue(ScriptContext& script_context)
{
    auto pCvar = script_context.GetArgument<ConVar*>(0);
    auto value = script_context.GetArgument<const char*>(1);

    if (!pCvar) {
        script_context.ThrowNativeError("Invalid cvar.");
    }

    pCvar->values = reinterpret_cast<CVValue_t**>((char*)value);
}

REGISTER_NATIVES(commands, {
    ScriptEngine::RegisterNativeHandler("ADD_COMMAND", AddCommand);
    ScriptEngine::RegisterNativeHandler("REMOVE_COMMAND", RemoveCommand);
    ScriptEngine::RegisterNativeHandler("ADD_COMMAND_LISTENER", AddCommandListener);
    ScriptEngine::RegisterNativeHandler("REMOVE_COMMAND_LISTENER", RemoveCommandListener);

    ScriptEngine::RegisterNativeHandler("COMMAND_GET_ARG_COUNT", CommandGetArgCount);
    ScriptEngine::RegisterNativeHandler("COMMAND_GET_ARG_STRING", CommandGetArgString);
    ScriptEngine::RegisterNativeHandler("COMMAND_GET_COMMAND_STRING", CommandGetCommandString);
    ScriptEngine::RegisterNativeHandler("COMMAND_GET_ARG_BY_INDEX", CommandGetArgByIndex);

    ScriptEngine::RegisterNativeHandler("FIND_CONVAR", FindConVar);
    ScriptEngine::RegisterNativeHandler("SET_CONVAR_STRING_VALUE", SetConVarStringValue);

    ScriptEngine::RegisterNativeHandler("ISSUE_CLIENT_COMMAND", IssueClientCommand);
})
} // namespace counterstrikesharp
