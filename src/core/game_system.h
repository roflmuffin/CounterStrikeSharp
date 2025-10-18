/**
 * =============================================================================
 * CS2Fixes
 * Copyright (C) 2023-2024 Source2ZE
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

#include "core/log.h"
#include "entitysystem.h"
#include "igamesystemfactory.h"

bool InitGameSystems();

class CGameSystem : public CBaseGameSystem
{
  public:
    GS_EVENT(BuildGameSessionManifest);
    GS_EVENT(ServerPreEntityThink);
    GS_EVENT(ServerPostEntityThink);

    void Shutdown() override
    {
        CSSHARP_CORE_INFO("CGameSystem::Shutdown");
        delete sm_Factory;
    }

    void SetGameSystemGlobalPtrs(void* pValue) override
    {
        if (sm_Factory) sm_Factory->SetGlobalPtr(pValue);
    }

    bool DoesGameSystemReallocate() override { return sm_Factory->ShouldAutoAdd(); }

    static IGameSystemFactory* sm_Factory;
};

class IEntityResourceManifest
{
  public:
    virtual void AddResource(const char*) = 0;
    virtual void AddResource(const char*, void*) = 0;
    virtual void AddResource(const char*, void*, void*, void*) = 0;
    virtual void unk_04() = 0;
    virtual void unk_05() = 0;
    virtual void unk_06() = 0;
    virtual void unk_07() = 0;
    virtual void unk_08() = 0;
    virtual void unk_09() = 0;
    virtual void unk_10() = 0;
};

extern IEntityResourceManifest* m_exportResourceManifest;
