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

#include <funchook.h>
#include <igameevents.h>
#include <public/eiface.h>

#include "characterset.h"
#include "core/coreconfig.h"
#include "core/gameconfig.h"
#include "core/log.h"
#include "core/managers/con_command_manager.h"
#include "core/memory.h"
#include "core/memory_module.h"
#include "scripting/callback_manager.h"

namespace counterstrikesharp {

ChatManager::ChatManager() {}

ChatManager::~ChatManager() {}

void ChatManager::OnAllInitialized() {}

void ChatManager::OnShutdown() {}

bool ChatManager::OnSayCommand(CEntityInstance* pController, const CCommand& args, bool teamonly)
{
    if (pController)
    {
        auto pEvent = globals::gameEventManager->CreateEvent("player_chat", true);
        if (pEvent)
        {
            pEvent->SetBool("teamonly", teamonly);
            pEvent->SetInt("userid", pController->GetEntityIndex().Get() - 1);
            pEvent->SetString("text", args[1]);

            globals::gameEventManager->FireEvent(pEvent, true);
        }
    }

    std::string prefix;
    bool bSilent = globals::coreConfig->IsSilentChatTrigger(args[1], prefix);
    bool bCommand = globals::coreConfig->IsPublicChatTrigger(args[1], prefix) || bSilent;

    if (bCommand)
    {
        auto message = std::string(args.ArgS());

        // trim quotes off message if they appear, then trim the prefix
        // "!foobar" -> foobar
        // !foobar -> foobar
        if (message.size() >= 2 && message.front() == '"' && message.back() == '"')
        {
            message = message.substr(1, message.size() - 2);
        }
        message = message.substr(prefix.size());

        CCommand newArgs;
        newArgs.Tokenize(message.c_str());

        auto prefixedPhrase = std::string("css_") + newArgs.Arg(0);
        auto bValidWithPrefix = globals::conCommandManager.IsValidValveCommand(prefixedPhrase.c_str());

        if (bValidWithPrefix)
        {
            // Re-tokenize with a `css_` prefix if we have found that its a valid command.
            newArgs.Tokenize(("css_" + std::string(message)).c_str());
        }

        globals::chatManager.OnSayCommandPost(pController, newArgs);
    }

    return bSilent;
}

bool ChatManager::OnSayCommandPre(CEntityInstance* pController, CCommand& command) { return false; }

void ChatManager::OnSayCommandPost(CEntityInstance* pController, CCommand& command)
{
    auto commandStr = command.Arg(0);

    return InternalDispatch(pController, commandStr, command);
}

void ChatManager::InternalDispatch(CEntityInstance* pPlayerController, const char* szTriggerPhase, CCommand& fullCommand)
{
    if (pPlayerController == nullptr)
    {
        globals::conCommandManager.ExecuteCommandCallbacks(fullCommand.Arg(0),
                                                           CCommandContext(CommandTarget_t::CT_NO_TARGET, CPlayerSlot(-1)), fullCommand,
                                                           HookMode::Pre, CommandCallingContext::Chat);
        return;
    }

    auto index = pPlayerController->GetEntityIndex().Get();
    auto slot = CPlayerSlot(index - 1);

    globals::conCommandManager.ExecuteCommandCallbacks(fullCommand.Arg(0), CCommandContext(CommandTarget_t::CT_NO_TARGET, slot),
                                                       fullCommand, HookMode::Pre, CommandCallingContext::Chat);
}
} // namespace counterstrikesharp
