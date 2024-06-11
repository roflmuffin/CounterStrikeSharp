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
#include <public/eiface.h>
#include <schema.h>

#include "core/managers/player_manager.h"
#include "scripting/callback_manager.h"

SH_DECL_HOOK3(IVEngineServer2, SetClientListening, SH_NOATTRIB, 0, bool, CPlayerSlot, CPlayerSlot, bool);

namespace counterstrikesharp {

VoiceManager::VoiceManager() {}

VoiceManager::~VoiceManager() {}

void VoiceManager::OnAllInitialized()
{
    SH_ADD_HOOK(IVEngineServer2, SetClientListening, globals::engine, SH_MEMBER(this, &VoiceManager::SetClientListening), false);
}

void VoiceManager::OnShutdown()
{
    SH_REMOVE_HOOK(IVEngineServer2, SetClientListening, globals::engine, SH_MEMBER(this, &VoiceManager::SetClientListening), false);
}

bool VoiceManager::SetClientListening(CPlayerSlot iReceiver, CPlayerSlot iSender, bool bListen)
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
            RETURN_META_VALUE_NEWPARAMS(MRES_IGNORED, bListen, &IVEngineServer2::SetClientListening, (iReceiver, iSender, false));
        }

        if (senderFlags & Speak_Muted)
        {
            RETURN_META_VALUE_NEWPARAMS(MRES_IGNORED, bListen, &IVEngineServer2::SetClientListening, (iReceiver, iSender, false));
        }

        if (listenOverride == Listen_Mute)
        {
            RETURN_META_VALUE_NEWPARAMS(MRES_IGNORED, bListen, &IVEngineServer2::SetClientListening, (iReceiver, iSender, false));
        }
        else if (listenOverride == Listen_Hear)
        {
            RETURN_META_VALUE_NEWPARAMS(MRES_IGNORED, bListen, &IVEngineServer2::SetClientListening, (iReceiver, iSender, true));
        }

        if ((senderFlags & Speak_All) || (receiverFlags & Speak_ListenAll))
        {
            RETURN_META_VALUE_NEWPARAMS(MRES_IGNORED, bListen, &IVEngineServer2::SetClientListening, (iReceiver, iSender, true));
        }

        if ((senderFlags & Speak_Team) || (receiverFlags & Speak_ListenTeam))
        {
            static auto classKey = hash_32_fnv1a_const("CBaseEntity");
            static auto memberKey = hash_32_fnv1a_const("m_iTeamNum");
            const static auto m_key = schema::GetOffset("CBaseEntity", classKey, "m_iTeamNum", memberKey);

            auto receiverController = globals::entitySystem->GetEntityInstance(CEntityIndex(iReceiver.Get() + 1));
            auto senderController = globals::entitySystem->GetEntityInstance(CEntityIndex(iSender.Get() + 1));

            if (receiverController && senderController)
            {
                auto receiverTeam = *reinterpret_cast<std::add_pointer_t<unsigned int>>((uintptr_t)(receiverController) + m_key.offset);

                auto senderTeam = *reinterpret_cast<std::add_pointer_t<unsigned int>>((uintptr_t)(senderController) + m_key.offset);

                RETURN_META_VALUE_NEWPARAMS(MRES_IGNORED, bListen, &IVEngineServer2::SetClientListening,
                                            (iReceiver, iSender, receiverTeam == senderTeam));
            }
        }
    }

    RETURN_META_VALUE(MRES_IGNORED, bListen);
}

void VoiceManager::OnClientCommand(CPlayerSlot slot, const CCommand& args)
{
    auto pPlayer = globals::playerManager.GetPlayerBySlot(slot.Get());

    if (!pPlayer) return;

    if (args.ArgC() > 1 && stricmp(args.Arg(0), "vban") == 0)
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
