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

#include "core/managers/memory_manager.h"
#include "core/log.h"

namespace counterstrikesharp {

MemoryManager::MemoryManager() = default;

MemoryManager::~MemoryManager() = default;

void MemoryManager::OnAllInitialized()
{
    const auto& gameConfigPatches = globals::gameConfig->GetPatches();

    for (const auto& gameConfigPatch : gameConfigPatches)
    {
        CreatePatch(gameConfigPatch.first.c_str(), gameConfigPatch.first.c_str());
    }
}

void MemoryManager::OnShutdown()
{
    for (const auto& patch : m_memoryPatches)
    {
        patch.second->UndoPatch();
        delete patch.second;
    }
}

bool MemoryManager::CreatePatch(const char* pszSignatureName, const char* pszName)
{
    if (DoesPatchExists(pszSignatureName))
    {
        CSSHARP_CORE_ERROR("Patch for '{}' already exists ({})", pszSignatureName, pszName);
        return false;
    }

    CMemPatch* patch = new CMemPatch(pszSignatureName, pszName);

    if (patch->PerformPatch(globals::gameConfig))
    {
        m_memoryPatches[pszSignatureName] = patch;
        return true;
    }

    return false;
}

void MemoryManager::UndoPatch(const char* pszSignatureName)
{
    auto it = m_memoryPatches.find(pszSignatureName);
    if (it == m_memoryPatches.end()) {
        return;
    }

    it->second->UndoPatch();
    delete it->second;
    m_memoryPatches.erase(it->first);
}

bool MemoryManager::DoesPatchExists(const char* pszSignatureName)
{
    auto it = m_memoryPatches.find(pszSignatureName);
    return it != m_memoryPatches.end();
}

CMemPatch* MemoryManager::GetPatchByName(const char* pszSignatureName)
{
    auto it = m_memoryPatches.find(pszSignatureName);
    if (it == m_memoryPatches.end()) {
        return nullptr;
    }

    return it->second;
}

void* MemoryManager::GetPatchAddress(const char* pszSignatureName)
{
    auto it = m_memoryPatches.find(pszSignatureName);
    if (it == m_memoryPatches.end()) {
        return nullptr;
    }

     return it->second->GetPatchAddress();
}

}  // namespace counterstrikesharp