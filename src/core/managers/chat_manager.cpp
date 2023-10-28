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

#include <funchook.h>

namespace counterstrikesharp {

ChatManager::ChatManager() {}

ChatManager::~ChatManager() {}

void ChatManager::OnAllInitialized()
{
    // TODO: Allow reading of the shared game data json from the C++ side too so this isn't
    // being hardcoded.
    m_pHostSay = (HostSay)FindSignature(
        MODULE_PREFIX "server" MODULE_EXT,
        R"(\x55\x48\x89\xE5\x41\x57\x49\x89\xFF\x41\x56\x41\x55\x41\x54\x4D\x89\xC4)");

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

    if (*args[1] == '/' || *args[1] == '!') {
        globals::chatManager.OnSayCommandPost(pController, newArgs);
        return;
    }

    m_pHostSay(pController, args, teamonly, unk1, unk2);

    if (pController) {
        auto pEvent = globals::gameEventManager->CreateEvent("player_chat", true);
        if (pEvent) {
            pEvent->SetBool("teamonly", teamonly);
            pEvent->SetInt("userid", pController->GetEntityIndex().Get());
            pEvent->SetString("text", args[1]);

            globals::gameEventManager->FireEvent(pEvent, true);
        }
    }
}

bool ChatManager::OnSayCommandPre(CBaseEntity* pController, CCommand& command) {
    return false;
}

bool ChatManager::OnSayCommandPost(CBaseEntity* pController, CCommand& command)
{
    const char* args = command.ArgS();
    auto commandStr = command.Arg(0);

    return InternalDispatch(pController, commandStr + 1, command);
}

bool ChatManager::InternalDispatch(CBaseEntity* pPlayerController, const char* szTriggerPhase,
                                   CCommand& fullCommand)
{
    auto ppArgV = new const char*[fullCommand.ArgC()];
    ppArgV[0] = strdup(szTriggerPhase);
    for (int i = 1; i < fullCommand.ArgC(); i++) {
        ppArgV[i] = fullCommand.Arg(i);
    }


    auto command = globals::conCommandManager.FindCommand((std::string("css_") + szTriggerPhase).c_str());

    if (command) {
        ppArgV[0] = (std::string("css_") + szTriggerPhase).c_str();
    }

    CCommand commandCopy(fullCommand.ArgC(), ppArgV);

    if (pPlayerController == nullptr) {
        auto result = globals::conCommandManager.InternalDispatch(CPlayerSlot(-1), &commandCopy);
        delete[] ppArgV;
        return result;
    }

    auto index = pPlayerController->GetEntityIndex().Get();

    auto slot = CPlayerSlot(index - 1);

    auto result = globals::conCommandManager.InternalDispatch(slot, &commandCopy);
    delete[] ppArgV;
    return result;
}
} // namespace counterstrikesharp