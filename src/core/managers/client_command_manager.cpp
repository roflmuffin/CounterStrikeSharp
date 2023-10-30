#include "core/managers/client_command_manager.h"

#include <public/eiface.h>

#include <algorithm>

#include "scripting/callback_manager.h"
#include "core/log.h"

namespace counterstrikesharp {

ClientCommandManager::ClientCommandManager() {}

ClientCommandManager::~ClientCommandManager() {}

void ClientCommandManager::OnAllInitialized()
{
    m_global_cmd.callback_pre = globals::callbackManager.CreateCallback("OnClientCommandGlobalPre");
    m_global_cmd.callback_post =
        globals::callbackManager.CreateCallback("OnClientCommandGlobalPost");
}

void ClientCommandManager::OnShutdown() {}

bool ClientCommandManager::DispatchClientCommand(CPlayerSlot slot, const char* cmd,
                                                 const CCommand* args)
{
    CSSHARP_CORE_TRACE("Dispatch client command {}", cmd);
    auto* p_info = m_cmd_lookup[cmd];

    bool result = false;

    if (m_global_cmd.callback_pre->GetFunctionCount() > 0) {
        m_global_cmd.callback_pre->ScriptContext().Reset();
        m_global_cmd.callback_pre->ScriptContext().Push(slot.Get());
        m_global_cmd.callback_pre->ScriptContext().Push(args);

        for (auto fnMethodToCall : m_global_cmd.callback_pre->GetFunctions()) {
            if (!fnMethodToCall)
                continue;
            fnMethodToCall(&m_global_cmd.callback_pre->ScriptContextStruct());

            auto hookResult = m_global_cmd.callback_pre->ScriptContext().GetResult<HookResult>();
            CSSHARP_CORE_TRACE("Received hook result from command callback {}:{}", cmd, hookResult);

            if (hookResult >= HookResult::Stop) {
                return true;
            } else if (hookResult >= HookResult::Handled) {
                result = true;
            }
        }
    }

    if (p_info && p_info->callback_pre) {
        p_info->callback_pre->ScriptContext().Reset();
        p_info->callback_pre->ScriptContext().Push(slot.Get());
        p_info->callback_pre->ScriptContext().Push(args);

        for (auto fnMethodToCall : p_info->callback_pre->GetFunctions()) {
            if (!fnMethodToCall)
                continue;
            fnMethodToCall(&p_info->callback_pre->ScriptContextStruct());

            auto hookResult = p_info->callback_pre->ScriptContext().GetResult<HookResult>();
            CSSHARP_CORE_TRACE("Received hook result from command callback {}:{}", cmd, hookResult);

            if (hookResult >= HookResult::Stop) {
                return true;
            } else if (hookResult >= HookResult::Handled) {
                result = true;
            }
        }
    }

    if (m_global_cmd.callback_post->GetFunctionCount() > 0) {
        m_global_cmd.callback_post->ScriptContext().Reset();
        m_global_cmd.callback_post->ScriptContext().Push(slot.Get());
        m_global_cmd.callback_post->ScriptContext().Push(args);
        m_global_cmd.callback_post->Execute();
    }

    if (result && p_info && p_info->callback_post) {
        p_info->callback_post->ScriptContext().Reset();
        p_info->callback_post->ScriptContext().Push(slot.Get());
        p_info->callback_post->ScriptContext().Push(args);
        p_info->callback_post->Execute();
    }

    return result;
}
void ClientCommandManager::AddCommandListener(const char* cmd, CallbackT callback, bool bPost)
{
    // Handle global command listeners that listen for every ClientCommand.
    if (cmd == nullptr) {
        if (bPost) {
            m_global_cmd.callback_post->AddListener(callback);
            return;
        }

        m_global_cmd.callback_pre->AddListener(callback);
        return;
    }

    auto* p_info = m_cmd_lookup[std::string(cmd)];

    if (!p_info) {
        p_info = new ClientCommandInfo();
        p_info->command = cmd;

        p_info->callback_pre = globals::callbackManager.CreateCallback(cmd);
        p_info->callback_post = globals::callbackManager.CreateCallback(cmd);

        m_cmd_list.push_back(p_info);
        m_cmd_lookup[cmd] = p_info;
    }

    if (bPost) {
        p_info->callback_post->AddListener(callback);
    } else {
        p_info->callback_pre->AddListener(callback);
    }
}
void ClientCommandManager::RemoveCommandListener(const char* cmd, CallbackT callback, bool bPost)
{
    if (cmd == nullptr) {
        if (bPost) {
            m_global_cmd.callback_post->RemoveListener(callback);
            return;
        }

        m_global_cmd.callback_pre->RemoveListener(callback);
        return;
    }

    auto* p_info = m_cmd_lookup[std::string(cmd)];

    if (!p_info) {
        return;
    }

    if (bPost) {
        p_info->callback_post->RemoveListener(callback);
    } else {
        p_info->callback_pre->RemoveListener(callback);
    }
}
} // namespace counterstrikesharp