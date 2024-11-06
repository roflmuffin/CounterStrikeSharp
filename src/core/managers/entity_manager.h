/*
*  This file is part of CounterStrikeSharp.
*  CounterStrikeSharp is free software: you can redistribute it and/or modify
*  it under the terms of the GNU General Public License as published by
*  the Free Software Foundation, either version 3 of the License, or
*  (at your option) any later version.
*
*  CounterStrikeSharp is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU General Public License for more details.
*
*  You should have received a copy of the GNU General Public License
*  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>. *
*/

#pragma once

#include <map>
#include <vector>

#include "core/globals.h"
#include "core/global_listener.h"
#include "scripting/script_engine.h"
#include "entitysystem.h"
#include "scripting/callback_manager.h"

#include <variant.h>

#include "vprof.h"

namespace counterstrikesharp {
class ScriptCallback;

typedef std::pair<std::string, std::string> OutputKey_t;

class CEntityListener : public IEntityListener {
    void OnEntitySpawned(CEntityInstance *pEntity) override;
    void OnEntityCreated(CEntityInstance *pEntity) override;
    void OnEntityDeleted(CEntityInstance *pEntity) override;
    void OnEntityParentChanged(CEntityInstance *pEntity, CEntityInstance *pNewParent) override;
};

class CCheckTransmitInfoList {
public:
    CCheckTransmitInfoList(CCheckTransmitInfo** pInfoInfoList, int nInfoCount);
private:
    CCheckTransmitInfo** infoList;
    int infoCount;
};

class EntityManager : public GlobalClass {
    friend CEntityListener;
public:
    EntityManager();
    ~EntityManager();
    void OnAllInitialized() override;
    void OnShutdown() override;
    void HookEntityOutput(const char* szClassname, const char* szOutput, CallbackT fnCallback, HookMode mode);
    void UnhookEntityOutput(const char* szClassname, const char* szOutput, CallbackT fnCallback, HookMode mode);
    CEntityListener entityListener;
    std::map<OutputKey_t, CallbackPair*> m_pHookMap;
private:
    void CheckTransmit(CCheckTransmitInfo** pInfoInfoList, int nInfoCount, CBitVec<16384>& unionTransmitEdicts, const Entity2Networkable_t** pNetworkables, const uint16* pEntityIndicies, int nEntityIndices, bool bEnablePVSBits);

    ScriptCallback *on_entity_spawned_callback;
    ScriptCallback *on_entity_created_callback;
    ScriptCallback *on_entity_deleted_callback;
    ScriptCallback *on_entity_parent_changed_callback;
    ScriptCallback *check_transmit;

    std::string m_profile_name;
};


enum EntityIOTargetType_t
{
    ENTITY_IO_TARGET_INVALID = 0xFFFFFFFF,
    ENTITY_IO_TARGET_CLASSNAME = 0x0,
    ENTITY_IO_TARGET_CLASSNAME_DERIVES_FROM = 0x1,
    ENTITY_IO_TARGET_ENTITYNAME = 0x2,
    ENTITY_IO_TARGET_CONTAINS_COMPONENT = 0x3,
    ENTITY_IO_TARGET_SPECIAL_ACTIVATOR = 0x4,
    ENTITY_IO_TARGET_SPECIAL_CALLER = 0x5,
    ENTITY_IO_TARGET_EHANDLE = 0x6,
    ENTITY_IO_TARGET_ENTITYNAME_OR_CLASSNAME = 0x7,
};

struct EntityIOConnectionDesc_t
{
    string_t m_targetDesc;
    string_t m_targetInput;
    string_t m_valueOverride;
    CEntityHandle m_hTarget;
    EntityIOTargetType_t m_nTargetType;
    int32 m_nTimesToFire;
    float m_flDelay;
};

struct EntityIOConnection_t : EntityIOConnectionDesc_t
{
    bool m_bMarkedForRemoval;
    EntityIOConnection_t* m_pNext;
};

struct EntityIOOutputDesc_t
{
    const char* m_pName;
    uint32 m_nFlags;
    uint32 m_nOutputOffset;
};

class CEntityIOOutput
{
  public:
    void* vtable;
    EntityIOConnection_t* m_pConnections;
    EntityIOOutputDesc_t* m_pDesc;
};

typedef void (*FireOutputInternal)(CEntityIOOutput* const, CEntityInstance*, CEntityInstance*,
                                   const CVariant* const, float);

static void DetourFireOutputInternal(CEntityIOOutput* const pThis, CEntityInstance* pActivator,
                                     CEntityInstance* pCaller, const CVariant* const value, float flDelay);

static FireOutputInternal m_pFireOutputInternal = nullptr;

// Do it in here because i didn't found a good place to do this
inline void (*CEntityInstance_AcceptInput)(CEntityInstance* pThis, const char* pInputName, CEntityInstance* pActivator, CEntityInstance* pCaller, variant_t* value, int nOutputID);

inline void (*CEntitySystem_AddEntityIOEvent)(CEntitySystem* pEntitySystem,
                                              CEntityInstance* pTarget,
                                              const char* pInputName,
                                              CEntityInstance* pActivator,
                                              CEntityInstance* pCaller,
                                              variant_t* value,
                                              float delay,
                                              int nOutputID);
}  // namespace counterstrikesharp
