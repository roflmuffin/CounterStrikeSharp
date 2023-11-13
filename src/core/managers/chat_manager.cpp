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
    CCommand newArgs;
    newArgs.Tokenize(args.Arg(1));

    if (pController) {
        auto pEvent = globals::gameEventManager->CreateEvent("player_chat", true);
        if (pEvent) {
            pEvent->SetBool("teamonly", teamonly);
            pEvent->SetInt("userid", pController->GetEntityIndex().Get() - 1);
            pEvent->SetString("text", args[1]);

            globals::gameEventManager->FireEvent(pEvent, true);
        }
    }

    if (*args[1] == '/' || *args[1] == '!') {
        globals::chatManager.OnSayCommandPost(pController, newArgs);
        return;
    }

    m_pHostSay(pController, args, teamonly, unk1, unk2);
}

bool ChatManager::OnSayCommandPre(CBaseEntity* pController, CCommand& command) { return false; }

void ChatManager::OnSayCommandPost(CBaseEntity* pController, CCommand& command)
{
    const char* args = command.ArgS();
    auto commandStr = command.Arg(0);

    return InternalDispatch(pController, commandStr + 1, command);
}

void ChatManager::InternalDispatch(CBaseEntity* pPlayerController, const char* szTriggerPhase,
                                   CCommand& fullCommand)
{
    auto ppArgV = new const char*[fullCommand.ArgC()];
    ppArgV[0] = strdup(szTriggerPhase);
    for (int i = 1; i < fullCommand.ArgC(); i++) {
        ppArgV[i] = fullCommand.Arg(i);
    }

    auto prefixedPhrase = std::string("css_") + szTriggerPhase;

    auto bValidWithPrefix = globals::conCommandManager.IsValidValveCommand(prefixedPhrase.c_str());

    if (bValidWithPrefix) {
        ppArgV[0] = prefixedPhrase.c_str();
    }

    CCommand commandCopy(fullCommand.ArgC(), ppArgV);

    if (pPlayerController == nullptr) {
        globals::conCommandManager.ExecuteCommandCallbacks(
            commandCopy.Arg(0), CCommandContext(CommandTarget_t::CT_NO_TARGET, CPlayerSlot(-1)),
            commandCopy, HookMode::Pre);
        delete[] ppArgV;
        return;
    }

    auto index = pPlayerController->GetEntityIndex().Get();
    auto slot = CPlayerSlot(index - 1);

    globals::conCommandManager.ExecuteCommandCallbacks(
        commandCopy.Arg(0), CCommandContext(CommandTarget_t::CT_NO_TARGET, slot), commandCopy,
        HookMode::Pre);
    delete[] ppArgV;
}
} // namespace counterstrikesharp