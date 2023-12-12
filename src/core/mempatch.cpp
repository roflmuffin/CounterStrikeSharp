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

#include "mempatch.h"
#include "tier0/dbg.h"
#include "tier1/strtools.h"

#include "tier0/memdbgon.h"

#include "utils/plat.h"
#include "core/memory_module.h"
#include "core/log.h"

namespace counterstrikesharp {
using namespace modules;

bool CMemPatch::PerformPatch(CGameConfig* gameConfig)
{
    if (!m_pPatchAddress)
	{
		m_pPatchAddress = gameConfig->ResolveSignature(m_pSignatureName);
	}

    if (!m_pPatchAddress)
    {
        CSSHARP_CORE_ERROR("Failed to find patch address for {}", m_pSignatureName);
        return false;
    }

    const char* patch = gameConfig->GetPatch(m_pszName);

    if (!patch)
    {
        CSSHARP_CORE_ERROR("Failed to find patch for {}", m_pszName);
        return false;
    }

    m_pPatch = gameConfig->HexToByte(patch, m_iPatchLength);

    if (!m_pPatch)
    {
        CSSHARP_CORE_ERROR("Failed convert patch {} ({})", m_pszName, patch);
        return false;
    }

    m_pOriginalBytes = new byte[m_iPatchLength];
    V_memcpy(m_pOriginalBytes, m_pPatchAddress, m_iPatchLength);

    Plat_WriteMemory(m_pPatchAddress, (byte*)m_pPatch, m_iPatchLength);

    CSSHARP_CORE_INFO("Patched {} at {}", m_pszName, m_pPatchAddress);
    return true;
}

void CMemPatch::UndoPatch()
{
    if (!m_pPatchAddress)
        return;

    CSSHARP_CORE_INFO("Undoing patch {} at {}", m_pszName, m_pPatchAddress);

    Plat_WriteMemory(m_pPatchAddress, m_pOriginalBytes, m_iPatchLength);

    delete[] m_pOriginalBytes;
}
} // namespace counterstrikesharp