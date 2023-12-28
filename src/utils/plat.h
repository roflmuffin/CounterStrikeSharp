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
#include <cstdint>
#include "metamod_oslink.h"

#if defined(_WIN32)
#define FASTCALL __fastcall
#define THISCALL __thiscall
#else
#define FASTCALL __attribute__((fastcall))
#define THISCALL
#define strtok_s strtok_r
#endif

struct Module
{
#ifndef _WIN32
	void* pHandle;
#endif
	uint8_t* pBase;
	unsigned int nSize;
};

#ifndef _WIN32
int GetModuleInformation(HINSTANCE module, void** base, size_t* length);
#endif

#ifdef _WIN32
#define MODULE_PREFIX ""
#define MODULE_EXT ".dll"
#else
#define MODULE_PREFIX "lib"
#define MODULE_EXT ".so"
#endif

void Plat_WriteMemory(void* pPatchAddress, uint8_t *pPatch, int iPatchSize);