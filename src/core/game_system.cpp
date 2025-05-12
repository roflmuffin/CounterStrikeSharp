/**
 * =============================================================================
 * CS2Fixes
 * Copyright (C) 2023-2024 Source2ZE
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
 */

#include "core/log.h"
#include "core/globals.h"
#include "core/gameconfig.h"
#include "core/game_system.h"

#include "core/managers/server_manager.h"

CBaseGameSystemFactory** CBaseGameSystemFactory::sm_pFirst = nullptr;

CGameSystem g_GameSystem;
IGameSystemFactory* CGameSystem::sm_Factory = nullptr;

// This mess is needed to get the pointer to sm_pFirst so we can insert game systems
bool InitGameSystems()
{
    // This signature directly points to the instruction referencing sm_pFirst, and the opcode is 3
    // bytes so we skip those
    uint8* ptr = (uint8*)counterstrikesharp::globals::gameConfig->ResolveSignature("IGameSystem_InitAllSystems_pFirst") + 3;

    if (!ptr)
    {
        CSSHARP_CORE_ERROR("Failed to InitGameSystems, see warnings above.");
        return false;
    }

    // Grab the offset as 4 bytes
    uint32 offset = *(uint32*)ptr;

    // Go to the next instruction, which is the starting point of the relative jump
    ptr += 4;

    // Now grab our pointer
    CBaseGameSystemFactory::sm_pFirst = (CBaseGameSystemFactory**)(ptr + offset);

    // And insert the game system(s)
    CGameSystem::sm_Factory = new CGameSystemStaticFactory<CGameSystem>("CSSharp_GameSystem", &g_GameSystem);

    return true;
}

GS_EVENT_MEMBER(CGameSystem, BuildGameSessionManifest)
{
    IEntityResourceManifest* pResourceManifest = msg->m_pResourceManifest;

    CSSHARP_CORE_INFO("CGameSystem::BuildGameSessionManifest");

    counterstrikesharp::globals::serverManager.OnPrecacheResources(pResourceManifest);
}
