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

#include "../schema.h"

struct VPhysicsCollisionAttribute_t
{
	DECLARE_SCHEMA_CLASS_INLINE(VPhysicsCollisionAttribute_t)

	SCHEMA_FIELD(uint8, m_nCollisionGroup)
	SCHEMA_FIELD(uint64_t, m_nInteractsAs)
	SCHEMA_FIELD(uint64_t, m_nInteractsWith)
	SCHEMA_FIELD(uint64_t, m_nInteractsExclude)
};

class CCollisionProperty
{
public:
	DECLARE_SCHEMA_CLASS_INLINE(CCollisionProperty)

	SCHEMA_FIELD(VPhysicsCollisionAttribute_t, m_collisionAttribute)
	SCHEMA_FIELD(SolidType_t, m_nSolidType)
	SCHEMA_FIELD(uint8, m_usSolidFlags)
	SCHEMA_FIELD(uint8, m_CollisionGroup)
};