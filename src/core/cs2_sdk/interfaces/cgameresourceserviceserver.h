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
#include <platform.h>
#include "interfaces/interfaces.h"
#include <cstdint>

class CGameEntitySystem;

class CGameResourceService
{
public:
	CGameEntitySystem *GetGameEntitySystem()
	{
		return *reinterpret_cast<CGameEntitySystem **>((uintptr_t)(this) + 0x50);
	}
};