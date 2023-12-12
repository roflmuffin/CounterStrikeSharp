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

#pragma once

#include "core/globals.h"
#include "core/global_listener.h"
#include "core/mempatch.h"

#include <vector>

namespace counterstrikesharp {
class MemoryManager : public GlobalClass {
public:
    MemoryManager();
    ~MemoryManager();

    void OnAllInitialized() override;
    void OnShutdown() override;

    bool CreatePatch(const char* pszSignatureName, const char* pszName);
    void UndoPatch(const char* pszSignatureName);
    bool DoesPatchExists(const char* pszSignatureName);
    CMemPatch* GetPatchByName(const char* pszSignatureName);

    void* GetPatchAddress(const char* pszSignatureName);

private:
    std::unordered_map<std::string, CMemPatch*> m_memoryPatches;
};

}  // namespace counterstrikesharp