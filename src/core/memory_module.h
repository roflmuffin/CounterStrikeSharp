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
#include <cstdio>

#include "interface.h"
#include "strtools.h"
#include "metamod_oslink.h"

#include <vector>
#undef snprintf

namespace counterstrikesharp::modules {

class CModule
{
  public:
    CModule(const char* path, const char* module);

    void* FindSignature(const char* signature);

    void* FindSignature(const std::vector<int16_t>& sigBytes);

    void* FindInterface(const char* name);

    const char* m_pszModule;
    const char* m_pszPath;
    HINSTANCE m_hModule;
    void* m_base;
    size_t m_size;
};

} // namespace counterstrikesharp::modules