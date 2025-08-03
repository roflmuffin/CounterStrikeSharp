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

#include "chat_manager.h"
#include "entitysystem.h"

#include <nlohmann/json.hpp>
#include <public/eiface.h>
#include <schemasystem.h>
#include <schematypes.h>
#include <sourcehook/sourcehook.h>

#include <algorithm>

#include "core/coreconfig.h"
#include "core/log.h"
#include "core/memory.h"
#include "core/utils.h"
#include "interfaces/cs2_interfaces.h"
#include "metamod_oslink.h"
#include "scripting/callback_manager.h"
using json = nlohmann::json;

namespace counterstrikesharp {

json WriteTypeJson(json obj, CSchemaType* current_type)
{
    obj["name"] = current_type->m_sTypeName.Get();
    obj["category"] = current_type->m_eTypeCategory;

    if (current_type->m_eTypeCategory == SCHEMA_TYPE_ATOMIC)
    {
        obj["atomic"] = current_type->m_eAtomicCategory;

        if (current_type->m_eAtomicCategory == SCHEMA_ATOMIC_T)
        {
            auto atomicTType = static_cast<CSchemaType_Atomic_T*>(current_type);

            if (atomicTType->m_pAtomicInfo != nullptr)
            {
                obj["outer"] = atomicTType->m_pAtomicInfo->m_pszName;
            }
        }

        if (current_type->m_eAtomicCategory == SCHEMA_ATOMIC_T || current_type->m_eAtomicCategory == SCHEMA_ATOMIC_COLLECTION_OF_T)
        {
            auto atomicType = static_cast<CSchemaType_Atomic_T*>(current_type);

            if (atomicType->GetInnerType().Get() != nullptr)
            {
                obj["inner"] = WriteTypeJson(json::object(), atomicType->GetInnerType().Get());
            }
        }
    }
    else if (current_type->m_eTypeCategory == SCHEMA_TYPE_FIXED_ARRAY)
    {
        auto fixedArrayType = static_cast<CSchemaType_FixedArray*>(current_type);
        obj["inner"] = WriteTypeJson(json::object(), fixedArrayType->m_pElementType);
    }
    else if (current_type->m_eTypeCategory == SCHEMA_TYPE_POINTER)
    {
        auto ptrType = static_cast<CSchemaType_Ptr*>(current_type);
        obj["inner"] = WriteTypeJson(json::object(), ptrType->m_pObjectType);
    }

    return obj;
}

CON_COMMAND(css_dump_schema, "dump schema symbols")
{
    std::ofstream output(utils::GamedataDirectory() + "/schema.json");

    CSchemaSystemTypeScope* pType = globals::schemaSystem->FindTypeScopeForModule(MODULE_PREFIX "server" MODULE_EXT);

    auto index = pType->m_DeclaredClasses.m_Map.FirstInorder();
    std::vector<CSchemaClassInfo*> classes;
    do
    {
        classes.push_back(pType->m_DeclaredClasses.m_Map.Element(index)->m_pClassInfo);
        index = pType->m_DeclaredClasses.m_Map.NextInorder(index);
    } while (index != pType->m_DeclaredClasses.m_Map.InvalidIndex());

    index = pType->m_DeclaredEnums.m_Map.FirstInorder();
    std::vector<CSchemaEnumInfo*> enums;
    do
    {
        enums.push_back(pType->m_DeclaredEnums.m_Map.Element(index)->m_pEnumInfo);
        index = pType->m_DeclaredEnums.m_Map.NextInorder(index);
    } while (index != pType->m_DeclaredEnums.m_Map.InvalidIndex());

    pType = globals::schemaSystem->GlobalTypeScope();
    index = pType->m_DeclaredClasses.m_Map.FirstInorder();
    do
    {
        classes.push_back(pType->m_DeclaredClasses.m_Map.Element(index)->m_pClassInfo);
        index = pType->m_DeclaredClasses.m_Map.NextInorder(index);
    } while (index != pType->m_DeclaredClasses.m_Map.InvalidIndex());

    index = pType->m_DeclaredEnums.m_Map.FirstInorder();
    do
    {
        enums.push_back(pType->m_DeclaredEnums.m_Map.Element(index)->m_pEnumInfo);
        index = pType->m_DeclaredEnums.m_Map.NextInorder(index);
    } while (index != pType->m_DeclaredEnums.m_Map.InvalidIndex());

    json j;
    j["classes"] = json::object();
    j["enums"] = json::object();

    for (const auto& pClassInfo : classes)
    {
        short fieldsSize = pClassInfo->m_nFieldCount;
        SchemaClassFieldData_t* pFields = pClassInfo->m_pFields;

        j["classes"][pClassInfo->m_pszName] = json::object();
        if (pClassInfo->m_pBaseClasses)
        {
            j["classes"][pClassInfo->m_pszName]["parent"] = pClassInfo->m_pBaseClasses->m_pClass->m_pszName;
        }

        j["classes"][pClassInfo->m_pszName]["fields"] = json::array();

        for (int i = 0; i < fieldsSize; ++i)
        {
            SchemaClassFieldData_t& field = pFields[i];

            j["classes"][pClassInfo->m_pszName]["fields"].push_back({
                { "name", field.m_pszName },
                { "type", WriteTypeJson(json::object(), field.m_pType) },
            });
        }
    }

    for (const auto& pEnumInfo : enums)
    {
        j["enums"][pEnumInfo->m_pszName] = json::object();
        j["enums"][pEnumInfo->m_pszName]["align"] = pEnumInfo->m_nSize;
        j["enums"][pEnumInfo->m_pszName]["items"] = json::array();

        for (int i = 0; i < pEnumInfo->m_nEnumeratorCount; ++i)
        {
            auto& field = pEnumInfo->m_pEnumerators[i];

            j["enums"][pEnumInfo->m_pszName]["items"].push_back({
                { "name", field.m_pszName },
                { "value", field.m_nValue },
            });
        }
    }

    Msg("Schema dumped to %s\n", (utils::GamedataDirectory() + "/schema.json").c_str());
    output << std::setw(2) << j << std::endl;
}

SH_DECL_HOOK3_void(ICvar, DispatchConCommand, SH_NOATTRIB, 0, ConCommandRef, const CCommandContext&, const CCommand&);

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
ConCommandInfo::ConCommandInfo(bool bNoCallbacks) {}

ConCommandManager::ConCommandManager() {}

ConCommandManager::~ConCommandManager() {}

void ConCommandManager::OnAllInitialized()
{
    SH_ADD_HOOK_MEMFUNC(ICvar, DispatchConCommand, globals::cvars, this, &ConCommandManager::Hook_DispatchConCommand, false);
    SH_ADD_HOOK_MEMFUNC(ICvar, DispatchConCommand, globals::cvars, this, &ConCommandManager::Hook_DispatchConCommand_Post, true);

    m_global_cmd.callback_pre = globals::callbackManager.CreateCallback("OnClientCommandGlobalPre");
    m_global_cmd.callback_post = globals::callbackManager.CreateCallback("OnClientCommandGlobalPost");

    if (globals::coreConfig->UnlockConCommands)
    {
        UnlockConCommands();
    }

    if (globals::coreConfig->UnlockConVars)
    {
        UnlockConVars();
    }
}

static uint64 flagsToRemove = (FCVAR_HIDDEN | FCVAR_DEVELOPMENTONLY);

void UnlockConVars()
{
    int unhiddenConVars = 0;

    for (ConVarRefAbstract ref(ConVarRef((uint16)0)); ref.IsValidRef(); ref = ConVarRefAbstract(ConVarRef(ref.GetAccessIndex() + 1)))
    {
        if (!ref.IsFlagSet(flagsToRemove)) continue;

        ref.RemoveFlags(flagsToRemove);
        unhiddenConVars++;
    }
}

void UnlockConCommands()
{
    int unhiddenConCommands = 0;

    ConCommandData* currentConCommand = nullptr;
    ConCommandData* invalidConCommand = globals::cvars->GetConCommandData(ConCommandRef());
    ConCommandRef ref = ConCommandRef((uint16)0);

    ConCommandData* data = g_pCVar->GetConCommandData(ConCommandRef());
    for (ConCommandRef ref = ConCommandRef((uint16)0); ref.GetRawData() != data; ref = ConCommandRef(ref.GetAccessIndex() + 1))
    {
        if (!ref.IsFlagSet(flagsToRemove)) continue;

        ref.RemoveFlags(flagsToRemove);
        unhiddenConCommands++;
    }
}

void ConCommandManager::OnShutdown()
{
    SH_REMOVE_HOOK_MEMFUNC(ICvar, DispatchConCommand, globals::cvars, this, &ConCommandManager::Hook_DispatchConCommand, false);
    SH_REMOVE_HOOK_MEMFUNC(ICvar, DispatchConCommand, globals::cvars, this, &ConCommandManager::Hook_DispatchConCommand_Post, true);

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
    if (name == nullptr)
    {
        if (mode == HookMode::Pre)
        {
            m_global_cmd.callback_pre->AddListener(callback);
        }
        else
        {
            m_global_cmd.callback_post->AddListener(callback);
        }
        return;
    }

    auto strName = std::string(name);
    ConCommandInfo* pInfo = m_cmd_lookup[strName];

    if (!pInfo)
    {
        pInfo = new ConCommandInfo();
        m_cmd_lookup[strName] = pInfo;

        ConCommandRef hExistingCommand = globals::cvars->FindConCommand(name);
        if (hExistingCommand.IsValidRef())
        {
            pInfo->command = globals::cvars->GetConCommandData(hExistingCommand);
        }
    }

    if (mode == HookMode::Pre)
    {
        pInfo->callback_pre->AddListener(callback);
    }
    else
    {
        pInfo->callback_post->AddListener(callback);
    }
}

void ConCommandManager::RemoveCommandListener(const char* name, CallbackT callback, HookMode mode)
{
    if (name == nullptr)
    {
        if (mode == HookMode::Pre)
        {
            m_global_cmd.callback_pre->RemoveListener(callback);
        }
        else
        {
            m_global_cmd.callback_post->RemoveListener(callback);
        }
        return;
    }

    auto strName = std::string(name);
    ConCommandInfo* pInfo = m_cmd_lookup[strName];

    if (!pInfo)
    {
        return;
    }

    if (mode == HookMode::Pre)
    {
        pInfo->callback_pre->RemoveListener(callback);
    }
    else
    {
        pInfo->callback_post->RemoveListener(callback);
    }
}

bool ConCommandManager::AddValveCommand(const char* name, const char* description, bool server_only, int flags)
{
    ConCommandRef hExistingCommand = globals::cvars->FindConCommand(name);
    if (hExistingCommand.IsValidRef()) return false;

    auto conCommand = new ConCommand(strdup(name), CommandCallback, description ? strdup(description) : "", flags);

    ConCommandInfo* pInfo = m_cmd_lookup[std::string(name)];

    if (!pInfo)
    {
        pInfo = new ConCommandInfo();
        m_cmd_lookup[std::string(name)] = pInfo;
    }

    pInfo->command = conCommand->GetRawData();
    pInfo->server_only = server_only;

    return true;
}

bool ConCommandManager::RemoveValveCommand(const char* name)
{
    auto hFoundCommand = globals::cvars->FindConCommand(name);

    if (!hFoundCommand.IsValidRef())
    {
        return false;
    }

    globals::cvars->UnregisterConCommandCallbacks(hFoundCommand);

    auto pInfo = m_cmd_lookup[std::string(name)];
    if (!pInfo)
    {
        return true;
    }

    pInfo->command = nullptr;

    return true;
}

HookResult ConCommandManager::ExecuteCommandCallbacks(
    const char* name, const CCommandContext& ctx, const CCommand& args, HookMode mode, CommandCallingContext callingContext)
{
    CSSHARP_CORE_TRACE("[ConCommandManager::ExecuteCommandCallbacks][{}]: {}", mode == Pre ? "Pre" : "Post", name);
    ConCommandInfo* pInfo = m_cmd_lookup[std::string(name)];

    HookResult result = HookResult::Continue;

    auto globalCallback = mode == HookMode::Pre ? m_global_cmd.callback_pre : m_global_cmd.callback_post;

    m_cmd_contexts[&args] = callingContext;

    if (globalCallback->GetFunctionCount() > 0)
    {
        globalCallback->ScriptContext().Reset();
        globalCallback->ScriptContext().Push(ctx.GetPlayerSlot().Get());
        globalCallback->ScriptContext().Push(&args);

        for (auto fnMethodToCall : globalCallback->GetFunctions())
        {
            if (!fnMethodToCall) continue;
            fnMethodToCall(&globalCallback->ScriptContextStruct());

            auto hookResult = globalCallback->ScriptContext().GetResult<HookResult>();

            if (hookResult >= HookResult::Stop)
            {
                if (mode == HookMode::Pre)
                {
                    return HookResult::Stop;
                }

                result = hookResult;
                break;
            }

            if (hookResult >= HookResult::Handled)
            {
                result = hookResult;
            }
        }
    }

    if (!pInfo)
    {
        m_cmd_contexts.erase(&args);
        return result;
    }

    auto pCallback = mode == HookMode::Pre ? pInfo->callback_pre : pInfo->callback_post;

    pCallback->Reset();
    pCallback->ScriptContext().Push(ctx.GetPlayerSlot().Get());
    pCallback->ScriptContext().Push(&args);

    for (auto fnMethodToCall : pCallback->GetFunctions())
    {
        if (!fnMethodToCall) continue;
        fnMethodToCall(&pCallback->ScriptContextStruct());

        auto thisResult = pCallback->ScriptContext().GetResult<HookResult>();

        if (thisResult >= HookResult::Handled)
        {
            m_cmd_contexts.erase(&args);
            return thisResult;
        }
        else if (thisResult > result)
        {
            result = thisResult;
        }
    }

    m_cmd_contexts.erase(&args);

    return result;
}

void ConCommandManager::Hook_DispatchConCommand(ConCommandRef cmd, const CCommandContext& ctx, const CCommand& args)
{
    const char* name = args.Arg(0);

    CSSHARP_CORE_TRACE("[ConCommandManager::Hook_DispatchConCommand]: {}", name);

    auto slot = ctx.GetPlayerSlot();
    bool isSay = V_strcmp(name, "say");
    bool isTeamSay = V_strcmp(name, "say_team");

    if (isSay || isTeamSay)
    {
        CEntityInstance* entityInstance = nullptr;
        if (globals::entitySystem && (slot != -1))
        {
            entityInstance = globals::entitySystem->GetEntityInstance(CEntityIndex(slot.Get() + 1));
        }

        if (globals::chatManager.OnSayCommand(entityInstance, args, isTeamSay))
        {
            RETURN_META(MRES_SUPERCEDE);
        }
    }

    auto result = ExecuteCommandCallbacks(name, ctx, args, HookMode::Pre, CommandCallingContext::Console);
    if (result >= HookResult::Handled)
    {
        RETURN_META(MRES_SUPERCEDE);
    }
}
void ConCommandManager::Hook_DispatchConCommand_Post(ConCommandRef cmd, const CCommandContext& ctx, const CCommand& args)
{
    const char* name = args.Arg(0);

    auto result = ExecuteCommandCallbacks(name, ctx, args, HookMode::Post, CommandCallingContext::Console);
    if (result >= HookResult::Handled)
    {
        RETURN_META(MRES_SUPERCEDE);
    }
}
bool ConCommandManager::IsValidValveCommand(const char* name)
{
    ConCommandRef pCmd = globals::cvars->FindConCommand(name);
    return pCmd.IsValidRef();
}

CommandCallingContext ConCommandManager::GetCommandCallingContext(CCommand* args) { return m_cmd_contexts[args]; }

} // namespace counterstrikesharp
