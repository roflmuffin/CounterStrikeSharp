/**
 * =============================================================================
 * SourceMod
 * Copyright (C) 2004-2016 AlliedModders LLC.  All rights reserved.
 * =============================================================================
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License, version 3.0, as published by the
 * Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.  See the GNU General Public License for more
 * details.
 *
 * You should have received a copy of the GNU General Public License along with
 * this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 * As a special exception, AlliedModders LLC gives you permission to link the
 * code of this program (as well as its derivative works) to "Half-Life 2," the
 * "Source Engine," the "SourcePawn JIT," and any Game MODs that run on software
 * by the Valve Corporation.  You must obey the GNU General Public License in
 * all respects for all other code used.  Additionally, AlliedModders LLC grants
 * this exception to all derivative works.  AlliedModders LLC defines further
 * exceptions, found in LICENSE.txt (as of this writing, version JULY-31-2007),
 * or <http://www.sourcemod.net/license.php>.
 *
 * This file has been modified from its original form, under the GNU General
 * Public License, version 3.0.
 */

#include "core/managers/con_command_manager.h"

#include <public/eiface.h>
#include <sourcehook/sourcehook.h>

#include <algorithm>

#include "scripting/callback_manager.h"
#include "core/log.h"
#include "core/cs2_sdk/interfaces/cschemasystem.h"
#include "core/utils.h"
#include "core/memory.h"
#include "interfaces/cs2_interfaces.h"
#include <nlohmann/json.hpp>

#include "metamod_oslink.h"
using json = nlohmann::json;

namespace counterstrikesharp {

json WriteTypeJson(json obj, CSchemaType* current_type)
{
    obj["name"] = current_type->m_name_;
    obj["category"] = current_type->type_category;

    if (current_type->type_category == Schema_Atomic) {
        obj["atomic"] = current_type->atomic_category;

        if (current_type->atomic_category == Atomic_T &&
            current_type->m_atomic_t_.generic_type != nullptr) {
            obj["outer"] = current_type->m_atomic_t_.generic_type->m_name_;
        }

        if (current_type->atomic_category == Atomic_T ||
            current_type->atomic_category == Atomic_CollectionOfT) {
            obj["inner"] =
                WriteTypeJson(json::object(), current_type->m_atomic_t_.template_typename);
        }
    } else if (current_type->type_category == Schema_FixedArray) {
        obj["inner"] = WriteTypeJson(json::object(), current_type->m_array_.element_type_);
    } else if (current_type->type_category == Schema_Ptr) {
        obj["inner"] = WriteTypeJson(json::object(), current_type->m_schema_type_);
    }

    return obj;
}

CON_COMMAND(dump_schema, "dump schema symbols")
{
    std::vector<std::string> classNames;
    std::vector<std::string> enumNames;
    // Reading these from a static file since I cannot seem to get the
    // CSchemaSystemTypeScope->GetClasses() to return anything on linux.
    std::ifstream inputClasses(utils::GamedataDirectory() + "/schema_classes.txt");
    std::ifstream inputEnums(utils::GamedataDirectory() + "/schema_enums.txt");
    std::ofstream output(utils::GamedataDirectory() + "/schema.json");
    std::string line;

    while (std::getline(inputClasses, line)) {
        if (!line.empty() && line.back() == '\r') {
            line.pop_back();
        }
        classNames.push_back(line);
    }

    while (std::getline(inputEnums, line)) {
        if (!line.empty() && line.back() == '\r') {
            line.pop_back();
        }
        enumNames.push_back(line);
    }

    CSchemaSystemTypeScope* pType =
        interfaces::pSchemaSystem->FindTypeScopeForModule(MODULE_PREFIX "server" MODULE_EXT);

    json j;
    j["classes"] = json::object();
    j["enums"] = json::object();

    for (const auto& line : classNames) {
        SchemaClassInfoData_t* pClassInfo = pType->FindDeclaredClass(line.c_str());
        if (!pClassInfo)
            continue;

        short fieldsSize = pClassInfo->m_align;
        SchemaClassFieldData_t* pFields = pClassInfo->m_fields;

        j["classes"][pClassInfo->m_name] = json::object();
        if (pClassInfo->m_schema_parent) {
            j["classes"][pClassInfo->m_name]["parent"] =
                pClassInfo->m_schema_parent->m_class->m_name;
        }

        j["classes"][pClassInfo->m_name]["fields"] = json::array();

        for (int i = 0; i < fieldsSize; ++i) {
            SchemaClassFieldData_t& field = pFields[i];

            j["classes"][pClassInfo->m_name]["fields"].push_back({
                {"name", field.m_name},
                {"type", WriteTypeJson(json::object(), field.m_type)},
            });
        }
    }

    for (const auto& line : enumNames) {
        auto* pEnumInfo = pType->FindDeclaredEnum(line.c_str());
        if (!pEnumInfo)
            continue;

        j["enums"][pEnumInfo->m_binding_name_] = json::object();
        j["enums"][pEnumInfo->m_binding_name_]["align"] = pEnumInfo->m_align_;
        j["enums"][pEnumInfo->m_binding_name_]["items"] = json::array();

        for (int i = 0; i < pEnumInfo->m_size_; ++i) {
            auto& field = pEnumInfo->m_enum_info_[i];

            j["enums"][pEnumInfo->m_binding_name_]["items"].push_back({
                {"name", field.m_name},
                {"value", field.m_value},
            });
        }
    }

    Msg("Schema dumped to %s\n", (utils::GamedataDirectory() + "/schema.json").c_str());
    output << std::setw(2) << j << std::endl;
}

SH_DECL_HOOK3_void(ICvar, DispatchConCommand, SH_NOATTRIB, 0, ConCommandHandle,
                   const CCommandContext&, const CCommand&);

ConCommandInfo::ConCommandInfo()
{
    callback_pre = globals::callbackManager.CreateCallback("");
    callback_post = globals::callbackManager.CreateCallback("");
}
ConCommandInfo::~ConCommandInfo()
{
    globals::callbackManager.ReleaseCallback(callback_pre);
    globals::callbackManager.ReleaseCallback(callback_post);
}
ConCommandInfo::ConCommandInfo(bool bNoCallbacks) {

}

ConCommandManager::ConCommandManager() {}

ConCommandManager::~ConCommandManager() {}

void ConCommandManager::OnAllInitialized()
{
    SH_ADD_HOOK_MEMFUNC(ICvar, DispatchConCommand, globals::cvars, this,
                        &ConCommandManager::Hook_DispatchConCommand, false);
    SH_ADD_HOOK_MEMFUNC(ICvar, DispatchConCommand, globals::cvars, this,
                        &ConCommandManager::Hook_DispatchConCommand_Post, true);

    m_global_cmd.callback_pre = globals::callbackManager.CreateCallback("OnClientCommandGlobalPre");
    m_global_cmd.callback_post =
        globals::callbackManager.CreateCallback("OnClientCommandGlobalPost");
}

void ConCommandManager::OnShutdown()
{
    SH_REMOVE_HOOK_MEMFUNC(ICvar, DispatchConCommand, globals::cvars, this,
                           &ConCommandManager::Hook_DispatchConCommand, false);
    SH_REMOVE_HOOK_MEMFUNC(ICvar, DispatchConCommand, globals::cvars, this,
                           &ConCommandManager::Hook_DispatchConCommand_Post, true);

    globals::callbackManager.ReleaseCallback(m_global_cmd.callback_pre);
    globals::callbackManager.ReleaseCallback(m_global_cmd.callback_post);
}

void CommandCallback(const CCommandContext& context, const CCommand& command)
{
    // This is handled by the global hook
    RETURN_META(MRES_SUPERCEDE);
}

void ConCommandManager::AddCommandListener(const char* name, CallbackT callback, HookMode mode)
{
    if (name == nullptr) {
        if (mode == HookMode::Pre) {
            m_global_cmd.callback_pre->AddListener(callback);
        } else {
            m_global_cmd.callback_post->AddListener(callback);
        }
        return;
    }

    auto strName = std::string(name);
    ConCommandInfo* pInfo = m_cmd_lookup[strName];

    if (!pInfo) {
        pInfo = new ConCommandInfo();
        m_cmd_lookup[strName] = pInfo;

        ConCommandHandle hExistingCommand = globals::cvars->FindCommand(name);
        if (hExistingCommand.IsValid()) {
            pInfo->command = globals::cvars->GetCommand(hExistingCommand);
        }
    }

    if (mode == HookMode::Pre) {
        pInfo->callback_pre->AddListener(callback);
    } else {
        pInfo->callback_post->AddListener(callback);
    }
}

void ConCommandManager::RemoveCommandListener(const char* name, CallbackT callback, HookMode mode)
{
    if (name == nullptr) {
        if (mode == HookMode::Pre) {
            m_global_cmd.callback_pre->RemoveListener(callback);
        } else {
            m_global_cmd.callback_post->RemoveListener(callback);
        }
        return;
    }

    auto strName = std::string(name);
    ConCommandInfo* pInfo = m_cmd_lookup[strName];

    if (!pInfo) {
        return;
    }

    if (mode == HookMode::Pre) {
        pInfo->callback_pre->RemoveListener(callback);
    } else {
        pInfo->callback_post->RemoveListener(callback);
    }
}

bool ConCommandManager::AddValveCommand(const char* name, const char* description, bool server_only,
                                        int flags)
{
    ConCommandHandle hExistingCommand = globals::cvars->FindCommand(name);
    if (hExistingCommand.IsValid())
        return false;

    ConCommandRefAbstract conCommandRefAbstract;
    auto conCommand =
        new ConCommand(&conCommandRefAbstract, strdup(name), CommandCallback, strdup(description), flags);

    ConCommandInfo* pInfo = m_cmd_lookup[std::string(name)];

    if (!pInfo) {
        pInfo = new ConCommandInfo();
        m_cmd_lookup[std::string(name)] = pInfo;
    }

    pInfo->p_cmd = conCommandRefAbstract;
    pInfo->command = conCommand;
    pInfo->server_only = server_only;

    return true;
}

bool ConCommandManager::RemoveValveCommand(const char* name)
{
    auto hFoundCommand = globals::cvars->FindCommand(name);

    if (!hFoundCommand.IsValid()) {
        return false;
    }

    globals::cvars->UnregisterConCommand(hFoundCommand);

    auto pInfo = m_cmd_lookup[std::string(name)];
    if (!pInfo) {
        return true;
    }

    pInfo->command = nullptr;

    return true;
}

HookResult ConCommandManager::ExecuteCommandCallbacks(const char* name, const CCommandContext& ctx,
                                                      const CCommand& args, HookMode mode)
{
    CSSHARP_CORE_TRACE("[ConCommandManager::ExecuteCommandCallbacks][{}]: {}",
                       mode == Pre ? "Pre" : "Post", name);
    ConCommandInfo* pInfo = m_cmd_lookup[std::string(name)];

    HookResult result = HookResult::Continue;

    auto globalCallback = mode == HookMode::Pre ? m_global_cmd.callback_pre : m_global_cmd.callback_post;

    if (globalCallback->GetFunctionCount() > 0) {
        globalCallback->ScriptContext().Reset();
        globalCallback->ScriptContext().Push(ctx.GetPlayerSlot().Get());
        globalCallback->ScriptContext().Push(&args);

        for (auto fnMethodToCall : globalCallback->GetFunctions()) {
            if (!fnMethodToCall)
                continue;
            fnMethodToCall(&globalCallback->ScriptContextStruct());

            auto hookResult = globalCallback->ScriptContext().GetResult<HookResult>();

            if (hookResult >= HookResult::Stop) {
                if (mode == HookMode::Pre) {
                    return HookResult::Stop;
                }

                result = hookResult;
                break;
            }

            if (hookResult >= HookResult::Handled) {
                result = hookResult;
            }
        }
    }

    if (!pInfo) {
        return result;
    }

    auto pCallback = mode == HookMode::Pre ? pInfo->callback_pre : pInfo->callback_post;

    pCallback->Reset();
    pCallback->ScriptContext().Push(ctx.GetPlayerSlot().Get());
    pCallback->ScriptContext().Push(&args);

    for (auto fnMethodToCall : pCallback->GetFunctions()) {
        if (!fnMethodToCall)
            continue;
        fnMethodToCall(&pCallback->ScriptContextStruct());

        auto thisResult = pCallback->ScriptContext().GetResult<HookResult>();

        if (thisResult >= HookResult::Handled) {
            return thisResult;
        } else if (thisResult > result) {
            result = thisResult;
        }
    }

    return result;
}

void ConCommandManager::Hook_DispatchConCommand(ConCommandHandle cmd, const CCommandContext& ctx,
                                                const CCommand& args)
{
    const char* name = args.Arg(0);

    CSSHARP_CORE_TRACE("[ConCommandManager::Hook_DispatchConCommand]: {}", name);

    auto result = ExecuteCommandCallbacks(name, ctx, args, HookMode::Pre);
    if (result >= HookResult::Handled) {
        RETURN_META(MRES_SUPERCEDE);
    }
}
void ConCommandManager::Hook_DispatchConCommand_Post(ConCommandHandle cmd,
                                                     const CCommandContext& ctx,
                                                     const CCommand& args)
{
    const char* name = args.Arg(0);

    auto result = ExecuteCommandCallbacks(name, ctx, args, HookMode::Post);
    if (result >= HookResult::Handled) {
        RETURN_META(MRES_SUPERCEDE);
    }
}
bool ConCommandManager::IsValidValveCommand(const char* name) {
    ConCommandHandle pCmd = globals::cvars->FindCommand(name);
    return pCmd.IsValid();
}

} // namespace counterstrikesharp