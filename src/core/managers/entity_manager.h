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

class CCheckTransmitInfoHack
{
  public:
    CBitVec<16384>* m_pTransmitEntity;

  private:
    [[maybe_unused]] int8_t m_pad8[568];

  public:
    int32_t m_nPlayerSlot;
    bool m_bFullUpdate;
};

namespace counterstrikesharp {
class ScriptCallback;

typedef std::pair<std::string, std::string> OutputKey_t;

class CEntityListener : public IEntityListener
{
    void OnEntitySpawned(CEntityInstance* pEntity) override;
    void OnEntityCreated(CEntityInstance* pEntity) override;
    void OnEntityDeleted(CEntityInstance* pEntity) override;
    void OnEntityParentChanged(CEntityInstance* pEntity, CEntityInstance* pNewParent) override;
};

class CCheckTransmitInfoList
{
  public:
    CCheckTransmitInfoList(CCheckTransmitInfoHack** pInfoInfoList, int nInfoCount);

  private:
    CCheckTransmitInfoHack** infoList;
    int infoCount;
};

class EntityManager : public GlobalClass
{
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
    void CheckTransmit(ISource2GameEntities* pThis,
                       CCheckTransmitInfoHack** ppInfoList,
                       uint32_t infoCount,
                       CBitVec<16384>& unionTransmitEdicts1,
                       CBitVec<16384>& unionTransmitEdicts2,
                       const Entity2Networkable_t** pNetworkables,
                       const uint16* pEntityIndicies,
                       uint32_t nEntities);

    ScriptCallback* on_entity_spawned_callback;
    ScriptCallback* on_entity_created_callback;
    ScriptCallback* on_entity_deleted_callback;
    ScriptCallback* on_entity_parent_changed_callback;
    ScriptCallback* check_transmit;

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

typedef void (*FireOutputInternal)(
    CEntityIOOutput* const, CEntityInstance*, CEntityInstance*, const CVariant* const, float flDelay, void* unk1, char* unk2);

static void DetourFireOutputInternal(CEntityIOOutput* const pThis,
                                     CEntityInstance* pActivator,
                                     CEntityInstance* pCaller,
                                     const CVariant* const value,
                                     float flDelay,
                                     void* unk1,
                                     char* unk2);

static FireOutputInternal m_pFireOutputInternal = nullptr;

// Do it in here because i didn't found a good place to do this
inline void (*CEntityInstance_AcceptInput)(CEntityInstance* pThis,
                                           const char* pInputName,
                                           CEntityInstance* pActivator,
                                           CEntityInstance* pCaller,
                                           variant_t* value,
                                           int nOutputID,
                                           void*);

inline void (*CEntitySystem_AddEntityIOEvent)(CEntitySystem* pEntitySystem,
                                              CEntityInstance* pTarget,
                                              const char* pInputName,
                                              CEntityInstance* pActivator,
                                              CEntityInstance* pCaller,
                                              variant_t* value,
                                              float delay,
                                              int nOutputID,
                                              void*);

typedef uint32 SoundEventGuid_t;

enum gender_t : uint8
{
    GENDER_NONE = 0x0,
    GENDER_MALE = 0x1,
    GENDER_FEMALE = 0x2,
    GENDER_NAMVET = 0x3,
    GENDER_TEENGIRL = 0x4,
    GENDER_BIKER = 0x5,
    GENDER_MANAGER = 0x6,
    GENDER_GAMBLER = 0x7,
    GENDER_PRODUCER = 0x8,
    GENDER_COACH = 0x9,
    GENDER_MECHANIC = 0xA,
    GENDER_CEDA = 0xB,
    GENDER_CRAWLER = 0xC,
    GENDER_UNDISTRACTABLE = 0xD,
    GENDER_FALLEN = 0xE,
    GENDER_RIOT_CONTROL = 0xF,
    GENDER_CLOWN = 0x10,
    GENDER_JIMMY = 0x11,
    GENDER_HOSPITAL_PATIENT = 0x12,
    GENDER_BRIDE = 0x13,
    GENDER_LAST = 0x14,
};

struct EmitSound_t
{
    EmitSound_t()
        : m_nChannel(0), m_pSoundName(0), m_flVolume(VOL_NORM), m_SoundLevel(SNDLVL_NONE), m_nFlags(0), m_nPitch(PITCH_NORM), m_pOrigin(0),
          m_flSoundTime(0.0f), m_pflSoundDuration(0), m_bEmitCloseCaption(true), m_bWarnOnMissingCloseCaption(false),
          m_bWarnOnDirectWaveReference(false), m_nSpeakerEntity(-1), m_UtlVecSoundOrigin(), m_nForceGuid(0), m_SpeakerGender(GENDER_NONE)
    {
    }
    int m_nChannel;
    const char* m_pSoundName;
    float m_flVolume;
    soundlevel_t m_SoundLevel;
    int m_nFlags;
    int m_nPitch;
    const Vector* m_pOrigin;
    float m_flSoundTime;
    float* m_pflSoundDuration;
    bool m_bEmitCloseCaption;
    bool m_bWarnOnMissingCloseCaption;
    bool m_bWarnOnDirectWaveReference;
    CEntityIndex m_nSpeakerEntity;
    CUtlVector<Vector, CUtlMemory<Vector, int>> m_UtlVecSoundOrigin;
    SoundEventGuid_t m_nForceGuid;
    gender_t m_SpeakerGender;
};

struct SndOpEventGuid_t
{
    SoundEventGuid_t m_nGuid;
    uint64 m_hStackHash;
};

inline SndOpEventGuid_t(FASTCALL* CBaseEntity_EmitSoundFilter)(IRecipientFilter& filter, CEntityIndex ent, const EmitSound_t& params);

SndOpEventGuid_t
EntityEmitSoundFilter(IRecipientFilter& filter, uint32 ent, const char* pszSound, float flVolume = 1.0f, float flPitch = 1.0f);
} // namespace counterstrikesharp
