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

#include "core/managers/chat_manager.h"
#include "core/managers/con_command_manager.h"
#include "scripting/callback_manager.h"
#include "characterset.h"

#include <igameevents.h>
#include <baseentity.h>
#include <public/eiface.h>
#include "core/memory.h"
#include "core/log.h"
#include "core/coreconfig.h"
#include "core/gameconfig.h"

#include <funchook.h>

#include "core/memory_module.h"

namespace counterstrikesharp {

ChatManager::ChatManager() {}

ChatManager::~ChatManager() {}

void ChatManager::OnAllInitialized()
{
    m_pHostSay = reinterpret_cast<HostSay>(
        modules::server->FindSignature(globals::gameConfig->GetSignature("Host_Say")));

    if (m_pHostSay == nullptr) {
        CSSHARP_CORE_ERROR("Failed to find signature for \'Host_Say\'");
        return;
    }

    auto m_hook = funchook_create();
    funchook_prepare(m_hook, (void**)&m_pHostSay, (void*)&DetourHostSay);
    funchook_install(m_hook, 0);
}

void ChatManager::OnShutdown() {}

void DetourHostSay(CBaseEntity* pController, CCommand& args, bool teamonly, int unk1,
                   const char* unk2)
{
    if (pController) {
        auto pEvent = globals::gameEventManager->CreateEvent("player_chat", true);
        if (pEvent) {
            pEvent->SetBool("teamonly", teamonly);
            pEvent->SetInt("userid", pController->GetEntityIndex().Get() - 1);
            pEvent->SetString("text", args[1]);

            globals::gameEventManager->FireEvent(pEvent, true);
        }
    }

    std::string prefix;
    bool bSilent = globals::coreConfig->IsSilentChatTrigger(args[1], prefix);
    bool bCommand = globals::coreConfig->IsPublicChatTrigger(args[1], prefix) || bSilent;

    if (!bSilent) {
        m_pHostSay(pController, args, teamonly, unk1, unk2);
    }

    if (bCommand)
    {
        char *pszMessage = (char *)(args.ArgS() + prefix.length() + 1);

        // Trailing slashes are only removed if Host_Say has been called.
        if (bSilent)
            pszMessage[V_strlen(pszMessage) - 1] = 0;

        CCommand args;
        args.Tokenize(pszMessage);

        auto prefixedPhrase = std::string("css_") + args.Arg(0);
        auto bValidWithPrefix = globals::conCommandManager.IsValidValveCommand(prefixedPhrase.c_str());

        if (bValidWithPrefix) {
            // Re-tokenize with a `css_` prefix if we have found that its a valid command.
            args.Tokenize(("css_" + std::string(pszMessage)).c_str());
        }

        globals::chatManager.OnSayCommandPost(pController, args);
    }
}

bool ChatManager::OnSayCommandPre(CBaseEntity* pController, CCommand& command) { return false; }

void ChatManager::OnSayCommandPost(CBaseEntity* pController, CCommand& command)
{
    auto commandStr = command.Arg(0);

    return InternalDispatch(pController, commandStr, command);
}

void ChatManager::InternalDispatch(CBaseEntity* pPlayerController, const char* szTriggerPhase,
                                   CCommand& fullCommand)
{
    if (pPlayerController == nullptr) {
        globals::conCommandManager.ExecuteCommandCallbacks(
            fullCommand.Arg(0), CCommandContext(CommandTarget_t::CT_NO_TARGET, CPlayerSlot(-1)),
            fullCommand, HookMode::Pre, CommandCallingContext::Chat);
        return;
    }

    auto index = pPlayerController->GetEntityIndex().Get();
    auto slot = CPlayerSlot(index - 1);

    globals::conCommandManager.ExecuteCommandCallbacks(
        fullCommand.Arg(0), CCommandContext(CommandTarget_t::CT_NO_TARGET, slot), fullCommand,
        HookMode::Pre, CommandCallingContext::Chat);
}
} // namespace counterstrikesharp