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

#ifdef __linux__
#include "plat.h"
#include <dlfcn.h>
#include <libgen.h>
#include <stdio.h>
#include <string.h>
#include "sys/mman.h"
#include <locale>
#include <elf.h>
#include <link.h>
#include "dbg.h"

#include "tier0/memdbgon.h"

#define PAGE_SIZE			4096
#define PAGE_ALIGN_UP(x)	((x + PAGE_SIZE - 1) & ~(PAGE_SIZE - 1))

struct ModuleInfo
{
	const char* path; // in
	uint8_t* base; // out
	uint size; // out
};

static int parse_prot(const char* s)
{
	int prot = 0;

	for (; *s; s++)
	{
		switch (*s)
		{
		case '-':
			break;
		case 'r':
			prot |= PROT_READ;
			break;
		case 'w':
			prot |= PROT_WRITE;
			break;
		case 'x':
			prot |= PROT_EXEC;
			break;
		case 's':
			break;
		case 'p':
			break;
		default:
			break;
		}
	}

	return prot;
}

static int get_prot(void* pAddr, size_t nSize)
{
	FILE* f = fopen("/proc/self/maps", "r");

	uintptr_t nAddr = (uintptr_t)pAddr;

	char line[512];
	while (fgets(line, sizeof(line), f))
	{
		char start[16];
		char end[16];
		char prot[16];

		const char* src = line;

		char* dst = start;
		while (*src != '-')
			*dst++ = *src++;
		*dst = 0;

		src++; // skip "-""

		dst = end;
		while (!isspace(*src))
			*dst++ = *src++;
		*dst = 0;

		src++; // skip space

		dst = prot;
		while (!isspace(*src))
			*dst++ = *src++;
		*dst = 0;

		uintptr_t nStart = (uintptr_t)strtoul(start, nullptr, 16);
		uintptr_t nEnd = (uintptr_t)strtoul(end, nullptr, 16);

		if (nStart < nAddr && nEnd >(nAddr + nSize))
		{
			fclose(f);
			return parse_prot(prot);
		}
	}

	fclose(f);
	return 0;
}

void Plat_WriteMemory(void* pPatchAddress, uint8_t* pPatch, int iPatchSize)
{
	int old_prot = get_prot(pPatchAddress, iPatchSize);

	uintptr_t page_size = sysconf(_SC_PAGESIZE);
	uint8_t* align_addr = (uint8_t*)((uintptr_t)pPatchAddress & ~(page_size - 1));

	uint8_t* end = (uint8_t*)pPatchAddress + iPatchSize;
	uintptr_t align_size = end - align_addr;

	int result = mprotect(align_addr, align_size, PROT_READ | PROT_WRITE);

	memcpy(pPatchAddress, pPatch, iPatchSize);

	result = mprotect(align_addr, align_size, old_prot);
}
#endif

