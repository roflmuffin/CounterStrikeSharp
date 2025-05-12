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

#define CALL_VIRTUAL(retType, idx, ...) vmt::CallVirtual<retType>(idx, __VA_ARGS__)

namespace vmt {
template <typename T = void*> inline T GetVMethod(uint32 uIndex, void* pClass)
{
    if (!pClass)
    {
        return T();
    }

    void** pVTable = *static_cast<void***>(pClass);
    if (!pVTable)
    {
        return T();
    }

    return reinterpret_cast<T>(pVTable[uIndex]);
}

template <typename T, typename... Args> inline T CallVirtual(uint32 uIndex, void* pClass, Args... args)
{
#ifdef _WIN32
    auto pFunc = GetVMethod<T(__thiscall*)(void*, Args...)>(uIndex, pClass);
#else
    auto pFunc = GetVMethod<T (*)(void*, Args...)>(uIndex, pClass);
#endif
    if (!pFunc)
    {
        return T();
    }

    return pFunc(pClass, args...);
}
} // namespace vmt
