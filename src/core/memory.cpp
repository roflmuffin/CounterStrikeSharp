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
#include "log.h"
#include "memory_module.h"

using namespace counterstrikesharp::modules;

void* FindSignature(const char* moduleName, const char* bytesStr)
{
    auto module = GetModuleByName(moduleName);
    if (module == nullptr)
    {
        return nullptr;
    }

    return module->FindSignature(bytesStr);
}

#ifdef _WIN32
void* GetVirtualTable(CModule* module, const std::string& name)
{
    auto runTimeData = module->GetSection(".data");
    auto readOnlyData = module->GetSection(".rdata");

    if (!runTimeData || !readOnlyData)
    {
        Warning("Failed to find .data or .rdata section\n");
        return nullptr;
    }

    // Windows RTTI format: .?AVClassName@@
    std::string decoratedTableName = ".?AV" + name + "@@";

    SignatureIterator sigIt(runTimeData->m_pBase, runTimeData->m_iSize,
                            (const byte*)decoratedTableName.c_str(),
                            decoratedTableName.size() + 1);

    void* typeDescriptor = sigIt.FindNext(false);
    if (!typeDescriptor)
    {
        Warning("Failed to find type descriptor for %s\n", name.c_str());
        return nullptr;
    }

    typeDescriptor = (void*)((uintptr_t)typeDescriptor - 0x10);
    const uint32_t rttiTDRva = (uintptr_t)typeDescriptor - (uintptr_t)module->m_base;

    SignatureIterator sigIt2(readOnlyData->m_pBase, readOnlyData->m_iSize,
                             (const byte*)&rttiTDRva, sizeof(uint32_t));

    while (void* completeObjectLocator = sigIt2.FindNext(false))
    {
        auto completeObjectLocatorHeader = (uintptr_t)completeObjectLocator - 0xC;

        // Verify RTTI Complete Object Locator header (always 0x1)
        if (*(int32_t*)(completeObjectLocatorHeader) != 1)
            continue;

        // Verify RTTI vtable offset (always 0)
        if (*(int32_t*)((uintptr_t)completeObjectLocator - 0x8) != 0)
            continue;

        // Find reference to Complete Object Locator inside .rdata
        SignatureIterator sigIt3(readOnlyData->m_pBase, readOnlyData->m_iSize,
                                 (const byte*)&completeObjectLocatorHeader, sizeof(void*));

        void* vtable = sigIt3.FindNext(false);
        if (!vtable)
        {
            Warning("Failed to find vtable for %s\n", name.c_str());
            return nullptr;
        }

        // Return pointer after Complete Object Locator
        // (vtable + 0x8) → start of first virtual function
        return (void*)((uintptr_t)vtable + 0x8);
    }

    Warning("Failed to find RTTI Complete Object Locator for %s\n", name.c_str());
    return nullptr;
}
#else
void* GetVirtualTable(CModule* module, const std::string& name)
{
    auto readOnlyData = module->GetSection(".rodata");
    auto readOnlyRelocations = module->GetSection(".data.rel.ro");

    if (!readOnlyData || !readOnlyRelocations)
    {
        Warning("Failed to find .rodata or .data.rel.ro section\n");
        return nullptr;
    }

    // Linux RTTI format: "17CNavPhysicsInterface" etc.
    std::string decoratedTableName = std::to_string(name.length()) + name;

    SignatureIterator sigIt(readOnlyData->m_pBase, readOnlyData->m_iSize,
                                                         (const byte*)decoratedTableName.c_str(),
                                                         decoratedTableName.size() + 1);
    void* classNameString = sigIt.FindNext(false);
    if (!classNameString)
    {
        Warning("Failed to find type descriptor for %s\n", name.c_str());
        return nullptr;
    }

    // Find relocation referencing classNameString
    SignatureIterator sigIt2(readOnlyRelocations->m_pBase, readOnlyRelocations->m_iSize,
                                                          (const byte*)&classNameString, sizeof(void*));
    void* typeName = sigIt2.FindNext(false);
    if (!typeName)
    {
        Warning("Failed to find type name for %s\n", name.c_str());
        return nullptr;
    }

    void* typeInfo = (void*)((uintptr_t)typeName - 0x8);

    // Check both local/global relocation tables
    for (const auto& sectionName : { std::string_view(".data.rel.ro"), std::string_view(".data.rel.ro.local") })
    {
        auto section = module->GetSection(sectionName);
        if (!section)
            continue;

        SignatureIterator sigIt3(section->m_pBase, section->m_iSize,
                                                              (const byte*)&typeInfo, sizeof(void*));

        while (void* vtable = sigIt3.FindNext(false))
        {
            // Verify offset-to-top == 0
            if (*(int64_t*)((uintptr_t)vtable - 0x8) == 0)
            {
                // Return start of actual virtual method table
                // (vtable + 0x10) = skip RTTI + offset-to-top
                return (void*)((uintptr_t)vtable + 0x10);
            }
        }
    }

    CSSHARP_CORE_ERROR("Failed to find vtable for {}", name);
    return nullptr;
}
#endif

// ---------------------------------------------------------------------
// Wrapper for CounterStrikeSharp NativeAPI call
// ---------------------------------------------------------------------
void* FindVirtualTable(const char* moduleName, const char* vtableName)
{
    auto module = GetModuleByName(moduleName);
    if (module == nullptr)
        return nullptr;

    return GetVirtualTable(module, vtableName);
}
