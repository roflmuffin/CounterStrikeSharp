/**
 * vim: set ts=4 sw=4 tw=99 noet:
 * =============================================================================
 * Source2Toolkit
 * Copyright (C) 2025-2026 Michal "Slynx (˙·٠● S l y n x ●٠·˙)" Přikryl,
 * AlliedModders LLC. All rights reserved.
 * =============================================================================
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License, version 3.0, as published by the
 * Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU General Public License for more
 * details.
 *
 * You should have received a copy of the GNU General Public License along with
 * this program. If not, see <http://www.gnu.org/licenses/>.
 *
 * As a special exception, Michal "Slynx (˙·٠● S l y n x ●٠·˙)" Přikryl and
 * AlliedModders LLC give you permission to link the code of this program
 * (as well as its derivative works) to "Counter-Strike 2," "Source 2,"
 * "Steam," and any Game MODs or server software running on software by
 * Valve Corporation. You must obey the GNU General Public License in all
 * respects for all other code used.
 *
 * Additionally, this exception applies to all derivative works unless
 * otherwise stated in LICENSE.txt.
 *
 * Authors:
 *   - Michal "Slynx (˙·٠● S l y n x ●٠·˙)" Přikryl
 *   - AlliedModders LLC
 *
 * Project: Source2Toolkit
 */

#include "navphysicsinterface.h"

#include "core/globals.h"
#include "core/memory_module.h"

#include <cassert>

class CBaseEntity;

void INavPhysicsInterface::TraceLine(const Vector& vStart,
                                     const Vector& vEnd,
                                     CBaseEntity* pIgnore,
                                     uint64 nInteractsWith,
                                     uint8 nCollisionGroup,
                                     uint8 nObjectSetMask,
                                     CGameTrace* trace)
{
    if (!vTable)
    {
        vTable = reinterpret_cast<void**>(counterstrikesharp::modules::server->FindVirtualTable("CNavPhysicsInterface"));
    }

    assert(vTable);
    auto* iface = reinterpret_cast<INavPhysicsInterface*>(&vTable);
    iface->Nav_TraceLine(vStart, vEnd, pIgnore, nInteractsWith, nCollisionGroup, nObjectSetMask, trace);
}

void INavPhysicsInterface::TraceLine(const Vector& vStart, const Vector& vEnd, CTraceFilter* pFilter, CGameTrace* trace)
{
    if (!vTable)
    {
        vTable = reinterpret_cast<void**>(counterstrikesharp::modules::server->FindVirtualTable("CNavPhysicsInterface"));
    }

    assert(vTable);
    auto* iface = reinterpret_cast<INavPhysicsInterface*>(&vTable);
    iface->Nav_TraceLine(vStart, vEnd, pFilter, trace);
}

void INavPhysicsInterface::TraceShape(const Ray_t& ray,
                                      const Vector& vStart,
                                      const Vector& vEnd,
                                      CBaseEntity* pIgnore,
                                      uint64 nInteractsWith,
                                      uint8 nCollisionGroup,
                                      uint8 nObjectSetMask,
                                      CGameTrace* trace)
{
    if (!vTable)
    {
        vTable = reinterpret_cast<void**>(counterstrikesharp::modules::server->FindVirtualTable("CNavPhysicsInterface"));
    }

    assert(vTable);
    auto* iface = reinterpret_cast<INavPhysicsInterface*>(&vTable);
    iface->Nav_TraceShape(ray, vStart, vEnd, pIgnore, nInteractsWith, nCollisionGroup, nObjectSetMask, trace);
}

void INavPhysicsInterface::TraceShape(const Ray_t& ray, const Vector& vStart, const Vector& vEnd, CTraceFilter* pFilter, CGameTrace* trace)
{
    if (!vTable)
    {
        vTable = reinterpret_cast<void**>(counterstrikesharp::modules::server->FindVirtualTable("CNavPhysicsInterface"));
    }

    assert(vTable);
    auto* iface = reinterpret_cast<INavPhysicsInterface*>(&vTable);
    iface->Nav_TraceShape(ray, vStart, vEnd, pFilter, trace);
}

uint64 INavPhysicsInterface::PointContents(const Vector* const vTestPos, uint64 nContentsMask)
{
    if (!vTable)
    {
        vTable = reinterpret_cast<void**>(counterstrikesharp::modules::server->FindVirtualTable("CNavPhysicsInterface"));
    }

    assert(vTable);
    auto* iface = reinterpret_cast<INavPhysicsInterface*>(&vTable);
    return iface->Nav_PointContents(vTestPos, nContentsMask);
}

bool INavPhysicsInterface::CheckAreaOverlappingEntity(const void* const rArea, const CBaseEntity* const rEntity, bool bExtrudeHullHeight)
{
    if (!vTable)
    {
        vTable = reinterpret_cast<void**>(counterstrikesharp::modules::server->FindVirtualTable("CNavPhysicsInterface"));
    }

    assert(vTable);
    auto* iface = reinterpret_cast<INavPhysicsInterface*>(&vTable);
    return iface->Nav_CheckAreaOverlappingEntity(rArea, rEntity, bExtrudeHullHeight);
}

void INavPhysicsInterface::GetEntityWorldSpaceAABB(const CBaseEntity* const rEntity, Vector* pMinsOut, Vector* pMaxsOut)
{
    if (!vTable)
    {
        vTable = reinterpret_cast<void**>(counterstrikesharp::modules::server->FindVirtualTable("CNavPhysicsInterface"));
    }

    assert(vTable);
    auto* iface = reinterpret_cast<INavPhysicsInterface*>(&vTable);
    iface->Nav_GetEntityWorldSpaceAABB(rEntity, pMinsOut, pMaxsOut);
}
