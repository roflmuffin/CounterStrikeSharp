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
 *
 * This file has been modified for use in CounterStrikeSharp.
 */

#include "gameconfig.h"
#include "memory_module.h"

void* FindSignature(const char* moduleName, const char* bytesStr) {
    auto module = counterstrikesharp::modules::GetModuleByName(moduleName);
    if (module == nullptr) {
        return nullptr;
    }

    return module->FindSignature(bytesStr);
}
