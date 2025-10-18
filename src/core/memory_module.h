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
#include <cstdint>
#include <unordered_map>
#include <optional>
#include <string>
#include <string_view>
#include <vector>

#if __linux__
#include <link.h>
#endif

#include "interface.h"
#include "strtools.h"
#undef snprintf

namespace counterstrikesharp::modules {

struct Section
{
    std::string m_szName;
    void* m_pBase;
    size_t m_iSize;
};

class SignatureIterator
{
  public:
    SignatureIterator(void* pBase, size_t iSize, const byte* pSignature, size_t iSigLength)
        : m_pBase((byte*)pBase), m_iSize(iSize), m_pSignature(pSignature), m_iSigLength(iSigLength)
    {
        m_pCurrent = m_pBase;
    }

    void* FindNext(bool allowWildcard)
    {
        for (size_t i = 0; i < m_iSize; i++)
        {
            size_t Matches = 0;
            while (*(m_pCurrent + i + Matches) == m_pSignature[Matches] || (allowWildcard && m_pSignature[Matches] == '\x2A'))
            {
                Matches++;
                if (Matches == m_iSigLength)
                {
                    m_pCurrent += i + 1;
                    return m_pCurrent - 1;
                }
            }
        }

        return nullptr;
    }

  private:
    byte* m_pBase;
    size_t m_iSize;
    const byte* m_pSignature;
    size_t m_iSigLength;
    byte* m_pCurrent;
};

struct Segments
{
    Segments() = default;

    Segments(const Segments&) = default;
    Segments(Segments&&) = default;
    Segments& operator=(const Segments&) = default;
    Segments& operator=(Segments&&) = default;

    std::uintptr_t address{};
    std::vector<std::uint8_t> bytes{};
};

class CModule
{
  public:
#ifdef _WIN32
    CModule(std::string_view path, std::uint64_t base);
#else
    CModule(std::string_view path, struct dl_phdr_info* info);
#endif

    void* FindSignature(const char* signature);

    void* FindInterface(std::string_view name);

    void* FindSymbol(const std::string& name);

    void* FindVirtualTable(const std::string& name);

    Section* GetSection(const std::string_view name)
    {
        for (auto& section : m_sections)
            if (section.m_szName == name) return &section;

        return nullptr;
    }

    [[nodiscard]] bool IsInitialized() const { return m_bInitialized; }

    std::string m_pszModule{};
    std::string m_pszPath{};
    void* m_base{};
    size_t m_size{};

  private:
    bool m_bInitialized{};
    std::vector<Segments> m_vecSegments{};
    std::vector<Section> m_sections{};
    std::uintptr_t m_baseAddress{};
    std::unordered_map<std::string, std::uintptr_t> _symbols{};
    std::unordered_map<std::string, std::uintptr_t> _interfaces{};
    using fnCreateInterface = void* (*)(const char*);
    fnCreateInterface m_fnCreateInterface{};

#ifdef _WIN32
    void DumpSymbols();
#else
    void DumpSymbols(ElfW(Dyn) * dyn);
#endif

    std::optional<std::vector<std::uint8_t>>
    GetOriginalBytes(const std::vector<std::uint8_t>& disk_data, std::uintptr_t rva, std::size_t size);

    void* FindSignature(const std::vector<int16_t>& sigBytes);
    void* FindSignatureAlternative(const std::vector<int16_t>& sigBytes);
};

} // namespace counterstrikesharp::modules
