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

#include "utlstring.h"
#include "entity2/entitysystem.h"

#include "tier0/memdbgon.h"

CBaseEntity* CEntitySystem::GetBaseEntity(CEntityIndex entnum)
{
	if (entnum.Get() <= -1 || entnum.Get() >= (MAX_TOTAL_ENTITIES - 1))
		return nullptr;

	CEntityIdentity* pChunkToUse = m_EntityList.m_pIdentityChunks[entnum.Get() / MAX_ENTITIES_IN_LIST];
	if (!pChunkToUse)
		return nullptr;

	CEntityIdentity* pIdentity = &pChunkToUse[entnum.Get() % MAX_ENTITIES_IN_LIST];
	if (!pIdentity)
		return nullptr;

	if (pIdentity->m_EHandle.GetEntryIndex() != entnum.Get())
		return nullptr;

	return dynamic_cast<CBaseEntity*>(pIdentity->m_pInstance);
}

CBaseEntity* CEntitySystem::GetBaseEntity(const CEntityHandle& hEnt)
{
	if (!hEnt.IsValid())
		return nullptr;

	CEntityIdentity* pChunkToUse = m_EntityList.m_pIdentityChunks[hEnt.GetEntryIndex() / MAX_ENTITIES_IN_LIST];
	if (!pChunkToUse)
		return nullptr;

	CEntityIdentity* pIdentity = &pChunkToUse[hEnt.GetEntryIndex() % MAX_ENTITIES_IN_LIST];
	if (!pIdentity)
		return nullptr;

	if (pIdentity->m_EHandle != hEnt)
		return nullptr;

	return dynamic_cast<CBaseEntity*>(pIdentity->m_pInstance);
}