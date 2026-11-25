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

#include "core/managers/voice_manager.h"

#include <entity2/entitysystem.h>
#include <engine2/ivengineserver2.h>
#include <schema.h>

#include "core/core_bindings.h"
#include "core/managers/player_manager.h"
#include "core/slot_hooks.h"
#include "scripting/callback_manager.h"

namespace counterstrikesharp {

VoiceManager::VoiceManager() {}

VoiceManager::~VoiceManager() {}

void VoiceManager::OnAllInitialized()
{
    // By the slot index the boot Vtable Check verified. Disabled set-listening: voice flags and listen overrides
    // cannot be set (their natives throw); the engine decides who hears whom.
    if (bindings::ShouldRegisterHooks(bindings::features::PlayerSetListening, "SetClientListening hook"))
        HookSlot(m_hooks, "IVEngineServer2::SetClientListening", &IVEngineServer2::SetClientListening, globals::engine, this,
                 &VoiceManager::SetClientListening, nullptr);
}

void VoiceManager::OnShutdown() { m_hooks.Clear(); }

KHook::Return<bool> VoiceManager::SetClientListening(IVEngineServer2* hookThis, CPlayerSlot iReceiver, CPlayerSlot iSender, bool bListen)
{
    auto pReceiver = globals::playerManager.GetPlayerBySlot(iReceiver.Get());
    auto pSender = globals::playerManager.GetPlayerBySlot(iSender.Get());

    if (pReceiver && pSender)
    {
        auto listenOverride = pReceiver->GetListen(iSender);
        auto senderFlags = pSender->GetVoiceFlags();
        auto receiverFlags = pReceiver->GetVoiceFlags();

        if (pReceiver->m_selfMutes->Get(iSender.Get()))
        {
            return KHook::Recall(&IVEngineServer2::SetClientListening, KHook::Return<bool>{ KHook::Action::Ignore, bListen }, hookThis,
                                 iReceiver, iSender, false);
        }

        if (senderFlags & Speak_Muted)
        {
            return KHook::Recall(&IVEngineServer2::SetClientListening, KHook::Return<bool>{ KHook::Action::Ignore, bListen }, hookThis,
                                 iReceiver, iSender, false);
        }

        if (listenOverride == Listen_Mute)
        {
            return KHook::Recall(&IVEngineServer2::SetClientListening, KHook::Return<bool>{ KHook::Action::Ignore, bListen }, hookThis,
                                 iReceiver, iSender, false);
        }
        else if (listenOverride == Listen_Hear)
        {
            return KHook::Recall(&IVEngineServer2::SetClientListening, KHook::Return<bool>{ KHook::Action::Ignore, bListen }, hookThis,
                                 iReceiver, iSender, true);
        }

        if ((senderFlags & Speak_All) || (receiverFlags & Speak_ListenAll))
        {
            return KHook::Recall(&IVEngineServer2::SetClientListening, KHook::Return<bool>{ KHook::Action::Ignore, bListen }, hookThis,
                                 iReceiver, iSender, true);
        }

        if ((senderFlags & Speak_Team) || (receiverFlags & Speak_ListenTeam))
        {
            // Missing from the schema (logged once): team voice flags are not applied.
            static const auto teamNum = schema::FindField("CBaseEntity", "m_iTeamNum");

            auto receiverController = globals::entitySystem->GetEntityInstance(CEntityIndex(iReceiver.Get() + 1));
            auto senderController = globals::entitySystem->GetEntityInstance(CEntityIndex(iSender.Get() + 1));

            if (teamNum && receiverController && senderController)
            {
                auto receiverTeam = *reinterpret_cast<std::add_pointer_t<unsigned int>>((uintptr_t)(receiverController) + teamNum->offset);

                auto senderTeam = *reinterpret_cast<std::add_pointer_t<unsigned int>>((uintptr_t)(senderController) + teamNum->offset);

                return KHook::Recall(&IVEngineServer2::SetClientListening, KHook::Return<bool>{ KHook::Action::Ignore, bListen }, hookThis,
                                     iReceiver, iSender, receiverTeam == senderTeam);
            }
        }
    }

    return { KHook::Action::Ignore, bListen };
}

void VoiceManager::OnClientCommand(CPlayerSlot slot, const CCommand& args)
{
    auto pPlayer = globals::playerManager.GetPlayerBySlot(slot.Get());

    if (!pPlayer) return;

    if (args.ArgC() > 1 && V_stricmp(args.Arg(0), "vban") == 0)
    {
        // clients just refuse to send vban for indexes over 32 and all 4 fields are just the same number, so we only get the first one
        // for (int i = 1; (i < args.ArgC()) && (i < 3); i++) {
        unsigned int mask = 0;
        sscanf(args.Arg(1), "%x", &mask);

        pPlayer->m_selfMutes->SetDWord(0, mask);
        //}
    }
}

} // namespace counterstrikesharp
