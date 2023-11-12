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

#include <cstdlib>
#include <cstring>
#if __linux__
#include <dlfcn.h>
#include <elf.h>
#include <link.h>
#else
#include <Windows.h>
#include <Psapi.h>
#endif

#include <cstdio>

#include "memory_module.h"
#include "metamod_oslink.h"
#include "wchartypes.h"

#if __linux__
struct ModuleInfo {
    const char *path;  // in
    uint8_t *base;     // out
    uint size;         // out
};

#define PAGE_SIZE 4096
#define PAGE_ALIGN_UP(x) ((x + PAGE_SIZE - 1) & ~(PAGE_SIZE - 1))

// https://github.com/alliedmodders/sourcemod/blob/master/core/logic/MemoryUtils.cpp#L502-L587
int GetModuleInformation(void *hModule, void **base, size_t *length) {
    struct link_map *dlmap = (struct link_map *)hModule;
    Dl_info info;
    Elf64_Ehdr *file;
    Elf64_Phdr *phdr;
    uint16_t phdrCount;

    if (!dladdr((void *)dlmap->l_addr, &info)) {
        return 1;
    }

    if (!info.dli_fbase || !info.dli_fname) {
        return 2;
    }

    /* This is for our insane sanity checks :o */
    uintptr_t baseAddr = reinterpret_cast<uintptr_t>(info.dli_fbase);
    file = reinterpret_cast<Elf64_Ehdr *>(baseAddr);

    /* Check ELF magic */
    if (memcmp(ELFMAG, file->e_ident, SELFMAG) != 0) {
        return 3;
    }

    /* Check ELF version */
    if (file->e_ident[EI_VERSION] != EV_CURRENT) {
        return 4;
    }

    /* Check ELF endianness */
    if (file->e_ident[EI_DATA] != ELFDATA2LSB) {
        return 5;
    }

    /* Check ELF architecture */
    if (file->e_ident[EI_CLASS] != ELFCLASS64 || file->e_machine != EM_X86_64) {
        return 6;
    }

    /* For our purposes, this must be a dynamic library/shared object */
    if (file->e_type != ET_DYN) {
        return 7;
    }

    phdrCount = file->e_phnum;
    phdr = reinterpret_cast<Elf64_Phdr *>(baseAddr + file->e_phoff);

    for (uint16_t i = 0; i < phdrCount; i++) {
        Elf64_Phdr &hdr = phdr[i];

        /* We only really care about the segment with executable code */
        if (hdr.p_type == PT_LOAD && hdr.p_flags == (PF_X | PF_R)) {
            /* From glibc, elf/dl-load.c:
             * c->mapend = ((ph->p_vaddr + ph->p_filesz + GLRO(dl_pagesize) - 1)
             *             & ~(GLRO(dl_pagesize) - 1));
             *
             * In glibc, the segment file size is aligned up to the nearest page size and
             * added to the virtual address of the segment. We just want the size here.
             */
            // lib.memorySize = PAGE_ALIGN_UP(hdr.p_filesz);
            *length = PAGE_ALIGN_UP(hdr.p_filesz);
            *base = (void *)(baseAddr + hdr.p_paddr);

            break;
        }
    }

    return 0;
}
#endif

byte *ConvertToByteArray(const char *str, size_t *outLength) {
    size_t len = strlen(str) / 4;  // Every byte is represented as \xHH
    byte *result = (byte *)malloc(len);

    for (size_t i = 0, j = 0; i < len; ++i, j += 4) {
        sscanf(str + j, "\\x%2hhx", &result[i]);
    }

    *outLength = len;
    return result;
}


void* FindSignature(const char* moduleName, const char* bytesStr) {
    size_t iSigLength;
    auto sigBytes = ConvertToByteArray(bytesStr, &iSigLength);

    auto module = dlmount(moduleName);
    if (module == nullptr) {
        return nullptr;
    }

    void *moduleBase;
    size_t moduleSize;
#if __linux__
    if (GetModuleInformation(module, &moduleBase, &moduleSize) != 0) {
        return nullptr;
    }
#else
    MODULEINFO m_hModuleInfo;
    GetModuleInformation(GetCurrentProcess(), module, &m_hModuleInfo, sizeof(m_hModuleInfo));

    moduleBase = (void*)m_hModuleInfo.lpBaseOfDll;
    moduleSize = m_hModuleInfo.SizeOfImage;
#endif

    unsigned char *pMemory;
    void *returnAddr = nullptr;

    pMemory = (byte *)moduleBase;

    for (size_t i = 0; i < moduleSize; i++) {
        size_t matches = 0;
        while (*(pMemory + i + matches) == sigBytes[matches] || sigBytes[matches] == '\x2A') {
            matches++;
            if (matches == iSigLength) {
                returnAddr = (void *)(pMemory + i);
            }
        }
    }

    if (returnAddr == nullptr) {
        return nullptr;
    }

    return returnAddr;
}