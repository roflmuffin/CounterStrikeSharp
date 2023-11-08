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

SH_DECL_HOOK2_void(
    ConCommandHandle, Dispatch, SH_NOATTRIB, false, const CCommandContext&, const CCommand&);

void ConCommandInfo::HookChange(CallbackT cb, bool post) {
    if (post) {
        this->callback_post->AddListener(cb);
    } else {
        this->callback_pre->AddListener(cb);
    }
}

void ConCommandInfo::UnhookChange(CallbackT cb, bool post) {
    if (post) {
        if (this->callback_post && this->callback_post->GetFunctionCount()) {
            callback_post->RemoveListener(cb);
        }
    } else {
        if (this->callback_pre && this->callback_pre->GetFunctionCount()) {
            callback_pre->RemoveListener(cb);
        }
    }
}

ConCommandManager::ConCommandManager()
    : last_command_client(-1) {}

ConCommandManager::~ConCommandManager() {}

void ConCommandManager::OnAllInitialized() {}

void ConCommandManager::OnShutdown() {}

void CommandCallback(const CCommandContext& context, const CCommand& command) {
    bool rval = globals::conCommandManager.InternalDispatch(
        context.GetPlayerSlot(), &command);

    if (rval) {
        RETURN_META(MRES_SUPERCEDE);
    }
}

void CommandCallback_Post(const CCommandContext& context, const CCommand& command) {
    bool rval = globals::conCommandManager.InternalDispatch_Post(context.GetPlayerSlot(), &command);

    if (rval) {
        RETURN_META(MRES_SUPERCEDE);
    }
}

ConCommandInfo* ConCommandManager::AddOrFindCommand(const char* name,
                                                    const char* description,
                                                    bool server_only,
                                                    int flags) {
    ConCommandInfo* p_info = m_cmd_lookup[std::string(name)];

    if (!p_info) {
        CSSHARP_CORE_TRACE("[ConCommandManager] Could not find command in existing lookup {}", name);
        //        auto found = std::find_if(m_cmd_list.begin(), m_cmd_list.end(),
        //        [&](ConCommandInfo* info) {
        //            return V_strcasecmp(info->command->GetName(), name) == 0;
        //        });
        //        if (found != m_cmd_list.end()) {
        //            return *found;
        //        }
        p_info = new ConCommandInfo();
        ConCommandHandle existingCommand = globals::cvars->FindCommand(name);
        ConCommandRefAbstract pointerConCommand;
        p_info->p_cmd = pointerConCommand;

        if (!existingCommand.IsValid()) {
            if (!description) {
                description = "";
            }

            CSSHARP_CORE_TRACE("[ConCommandManager] Creating new command {}", name);

            char* new_name = strdup(name);
            char* new_desc = strdup(description);

            CSSHARP_CORE_TRACE("[ConCommandManager] Creating new command {}, {}, {}, {}, {}", (void*)&pointerConCommand, new_name, (void*)CommandCallback, new_desc, flags);

            auto conCommand =
                new ConCommand(&pointerConCommand, new_name, CommandCallback, new_desc, flags);
            
            CSSHARP_CORE_TRACE("[ConCommandManager] Creating callbacks for command {}", name);

            p_info->command = conCommand;
            p_info->callback_pre = globals::callbackManager.CreateCallback(name);
            p_info->callback_post = globals::callbackManager.CreateCallback(name);
            p_info->server_only = server_only;

            CSSHARP_CORE_TRACE("[ConCommandManager] Adding hooks for command callback for command {}", name);

            SH_ADD_HOOK(ConCommandHandle, Dispatch, &pointerConCommand.handle, SH_STATIC(CommandCallback), false);
            SH_ADD_HOOK(ConCommandHandle, Dispatch, &pointerConCommand.handle, SH_STATIC(CommandCallback_Post), true);

            CSSHARP_CORE_TRACE("[ConCommandManager] Adding command to internal lookup {}", name);

            m_cmd_list.push_back(p_info);
            m_cmd_lookup[name] = p_info;
        } else {
            //            p_info->callback_pre = globals::callbackManager.CreateCallback(name);
            //            p_info->callback_post = globals::callbackManager.CreateCallback(name);
            //            p_info->server_only = server_only;
            //
            //            SH_ADD_HOOK(ConCommandHandle, Dispatch, pointerConCommand->handle,
            //            SH_STATIC(CommandCallback), false); SH_ADD_HOOK(ConCommandHandle,
            //            Dispatch, pointerConCommand->handle, SH_STATIC(CommandCallback_Post),
            //            true);
        }

        return p_info;
    }

    return p_info;
}

ConCommandInfo* ConCommandManager::AddCommand(
    const char* name, const char* description, bool server_only, int flags, CallbackT callback) {
    ConCommandInfo* p_info = AddOrFindCommand(name, description, server_only, flags);
    if (!p_info || !p_info->callback_pre) {
        return nullptr;
    }

    p_info->callback_pre->AddListener(callback);

    return p_info;
}

bool ConCommandManager::RemoveCommand(const char* name, CallbackT callback) {
    auto strName = std::string(strdup(name));
    ConCommandInfo* p_info = m_cmd_lookup[strName];
    if (!p_info) return false;

    if (p_info->callback_pre && p_info->callback_pre->GetFunctionCount()) {
        p_info->callback_pre->RemoveListener(callback);
    }

    if (p_info->callback_post && p_info->callback_post->GetFunctionCount()) {
        p_info->callback_post->RemoveListener(callback);
    }

    if (!p_info->callback_pre || p_info->callback_pre->GetFunctionCount() == 0) {
        globals::cvars->UnregisterConCommand(p_info->p_cmd.handle);

        bool success;
        auto it = std::remove_if(m_cmd_list.begin(), m_cmd_list.end(),
                                 [p_info](ConCommandInfo* i) { return p_info == i; });

        if ((success = it != m_cmd_list.end())) m_cmd_list.erase(it, m_cmd_list.end());
        // if (success) {
        //     m_cmd_lookup[strName] = nullptr;
        // }

        return success;
    }

    return true;
}

ConCommandInfo* ConCommandManager::FindCommand(const char* name) {
    ConCommandInfo* p_info = m_cmd_lookup[std::string(name)];

    if (p_info == nullptr) {
        auto found = std::find_if(m_cmd_list.begin(), m_cmd_list.end(), [&](ConCommandInfo* info) {
            return V_strcasecmp(info->command->GetName(), name) == 0;
        });
        if (found != m_cmd_list.end()) {
            return *found;
        }

        ConCommandHandle p_cmd = globals::cvars->FindCommand(name);
        if (!p_cmd.IsValid()) return nullptr;

        p_info = new ConCommandInfo();
        p_info->command = globals::cvars->GetCommand(p_cmd);

        p_info->p_cmd = *p_info->command->GetRef();
        p_info->callback_pre = globals::callbackManager.CreateCallback(name);
        p_info->callback_post = globals::callbackManager.CreateCallback(name);
        p_info->server_only = false;

        m_cmd_list.push_back(p_info);
        m_cmd_lookup[name] = p_info;

        return p_info;
    }

    return p_info;
}

int ConCommandManager::GetCommandClient() { return last_command_client; }

void ConCommandManager::SetCommandClient(int client) { last_command_client = client + 1; }

bool ConCommandManager::InternalDispatch(CPlayerSlot slot, const CCommand* args) {
    const char* cmd = args->Arg(0);

    ConCommandInfo* p_info = m_cmd_lookup[cmd];
    if (p_info == nullptr) {
        if (slot.Get() == 0 && !globals::engine->IsDedicatedServer()) return false;

        for (ConCommandInfo* cmdInfo : m_cmd_list) {
            if ((cmdInfo != nullptr) && strcasecmp(cmdInfo->command->GetName(), cmd) == 0) {
                p_info = cmdInfo;
                continue;
            }
        }
    }

    if (!p_info) {
        return false;
    }

    int realClient = slot.Get();

    bool result = false;
    if (p_info->callback_pre) {
        p_info->callback_pre->ScriptContext().Reset();
        p_info->callback_pre->ScriptContext().SetArgument(0, realClient);
        p_info->callback_pre->ScriptContext().SetArgument(1, args);
        p_info->callback_pre->Execute(false);

        result = p_info->callback_pre->ScriptContext().GetResult<bool>();
    }

    return result;
}

bool ConCommandManager::InternalDispatch_Post(CPlayerSlot slot, const CCommand* args) {
    const char* cmd = args->Arg(0);

    ConCommandInfo* p_info = m_cmd_lookup[cmd];
    if (p_info == nullptr) {
        if (slot.Get() == 0 && !globals::engine->IsDedicatedServer()) return false;

        for (ConCommandInfo* cmdInfo : m_cmd_list) {
            if ((cmdInfo != nullptr) && strcasecmp(cmdInfo->command->GetName(), cmd) == 0) {
                p_info = cmdInfo;
                continue;
            }
        }
    }

    int realClient = slot.Get();

    bool result = false;
    if (p_info->callback_post) {
        p_info->callback_post->ScriptContext().Reset();
        p_info->callback_post->ScriptContext().SetArgument(0, realClient);
        p_info->callback_post->ScriptContext().SetArgument(1, args);
        p_info->callback_post->Execute(false);

        result = p_info->callback_post->ScriptContext().GetResult<bool>();
    }

    return result;
}

bool ConCommandManager::DispatchClientCommand(CPlayerSlot slot,
                                              const char* cmd,
                                              const CCommand* args) {
    ConCommandInfo* p_info = m_cmd_lookup[cmd];
    if (p_info == nullptr) {
        auto found =
            std::find_if(m_cmd_list.begin(), m_cmd_list.end(), [&](const ConCommandInfo* info) {
                return V_strcasecmp(info->command->GetName(), cmd) == 0;
            });
        if (found == m_cmd_list.end()) {
            return false;
        }

        p_info = *found;
    }

    if (p_info->server_only) return false;

    bool result = false;
    if (p_info->callback_pre) {
        p_info->callback_pre->ScriptContext().Reset();
        p_info->callback_pre->ScriptContext().Push(slot.Get());
        p_info->callback_pre->ScriptContext().Push(args);
        p_info->callback_pre->Execute();

        result = true;
    }

    if (result) {
        if (p_info->callback_post) {
            p_info->callback_post->ScriptContext().Reset();
            p_info->callback_post->ScriptContext().Push(slot.Get());
            p_info->callback_post->ScriptContext().Push(args);
            p_info->callback_post->Execute();

            result = true;
        }
    }

    return result;
}
}  // namespace counterstrikesharp