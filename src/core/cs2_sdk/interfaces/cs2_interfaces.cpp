/**
 * =============================================================================
 * CS2Fixes
 * Copyright (C) 2023 Source2ZE
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

#include "cs2_interfaces.h"
#include "core/memory_module.h"
#include "core/globals.h"
#include "interfaces/interfaces.h"

// memdbgon must be the last include file in a .cpp file!!!
#include "tier0/memdbgon.h"

namespace counterstrikesharp {
void interfaces::Initialize() {
    pGameResourceServiceServer = (CGameResourceService*)modules::engine->FindInterface(
        GAMERESOURCESERVICESERVER_INTERFACE_VERSION);
    g_pCVar = (ICvar*)modules::tier0->FindInterface(CVAR_INTERFACE_VERSION);
    g_pSource2GameEntities = (ISource2GameEntities*)modules::server->FindInterface(
        SOURCE2GAMEENTITIES_INTERFACE_VERSION);
}
}  // namespace counterstrikesharp
