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

#include "stdint.h"

#ifdef _WIN32
    #pragma warning(push)
    #pragma warning(disable : 4005)
#endif

#include <type_traits>

#ifdef _WIN32
    #pragma warning(pop)
#endif

#include "tier0/dbg.h"
#include "const.h"
#include "../../utils/virtual.h"
#include "stdint.h"
#undef schema

struct SchemaKey {
    int16_t offset;
    bool networked;
};

class Z_CBaseEntity;
void SetStateChanged(Z_CBaseEntity *pEntity, int offset);

inline uint32_t val_32_const = 0x811c9dc5;
inline uint32_t prime_32_const = 0x1000193;
inline uint64_t val_64_const = 0xcbf29ce484222325;
inline uint64_t prime_64_const = 0x100000001b3;

inline uint32_t hash_32_fnv1a_const(const char *str, const uint32_t value = val_32_const) noexcept {
    return (str[0] == '\0')
               ? value
               : hash_32_fnv1a_const(&str[1], (value ^ uint32_t(str[0])) * prime_32_const);
}

inline uint64_t hash_64_fnv1a_const(const char *str, const uint64_t value = val_64_const) noexcept {
    return (str[0] == '\0')
               ? value
               : hash_64_fnv1a_const(&str[1], (value ^ uint64_t(str[0])) * prime_64_const);
}

namespace schema {
int16_t FindChainOffset(const char *className);
SchemaKey GetOffset(const char *className, uint32_t classKey, const char *memberName, uint32_t memberKey);
}  // namespace schema
