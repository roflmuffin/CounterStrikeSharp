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

#pragma once

#include "platform.h"
#include "core/memory_module.h"
#include "core/gameconfig.h"

namespace counterstrikesharp {
using namespace modules;

class CMemPatch
{
  public:
    CMemPatch(const char* pSignatureName, const char* pszName)
        : m_pSignatureName(pSignatureName), m_pszName(pszName)
    {
        m_pModule = nullptr;
        m_pPatchAddress = nullptr;
        m_pOriginalBytes = nullptr;
        m_pSignature = nullptr;
        m_pPatch = nullptr;
        m_iPatchLength = 0;
    }

    bool PerformPatch(CGameConfig* gameConfig);
    void UndoPatch();

    void* GetPatchAddress() { return m_pPatchAddress; }

  private:
    CModule** m_pModule;
    const byte* m_pSignature;
    const byte* m_pPatch;
    byte* m_pOriginalBytes;
    const char* m_pSignatureName;
    const char* m_pszName;
    size_t m_iPatchLength;
    void* m_pPatchAddress;
};
} // namespace counterstrikesharp