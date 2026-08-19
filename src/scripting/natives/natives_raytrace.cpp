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
 *  along with CounterStrikeSharp.  If not, see <https://www.gnu.org/licenses/>.
 */

#include "scripting/autonative.h"
#include "scripting/script_engine.h"
#include "core/cs2_sdk/interfaces/navphysicsinterface.h"
#include "core/cs2_sdk/schema.h"
#include "core/globals.h"

#include "gametrace.h"
#include "cmodel.h"
#include "mathlib/mathlib.h"
#include "entity2/entityinstance.h"
#include "entity2/entitysystem.h"
#include "entityhandle.h"

namespace counterstrikesharp {

#pragma pack(push, 1)
struct CSSTraceResult
{
    void* m_pSurfaceProperties;
    void* m_pEnt;
    void* m_pHitbox;
    void* m_pBody;
    void* m_pShape;

    uint64_t m_nContents;

    float m_flStartX, m_flStartY, m_flStartZ;
    float m_flEndX, m_flEndY, m_flEndZ;
    float m_flHitNormalX, m_flHitNormalY, m_flHitNormalZ;
    float m_flHitPointX, m_flHitPointY, m_flHitPointZ;

    float m_flHitOffset;
    float m_flFraction;

    int32_t m_nTriangle;
    int16_t m_nHitboxBoneIndex;
    uint8_t m_eRayType;
    uint8_t m_bStartInSolid;
    uint8_t m_bExactHitPoint;
};
#pragma pack(pop)

static void FillResult(CSSTraceResult* out, const CGameTrace& trace)
{
    out->m_pSurfaceProperties = const_cast<CPhysSurfaceProperties*>(trace.m_pSurfaceProperties);
    out->m_pEnt = trace.m_pEnt;
    out->m_pHitbox = const_cast<CHitBox*>(trace.m_pHitbox);
    out->m_pBody = trace.m_hBody;
    out->m_pBody = trace.m_hShape;

    out->m_nContents = trace.m_nContents;

    out->m_flStartX = trace.m_vStartPos.x;
    out->m_flStartY = trace.m_vStartPos.y;
    out->m_flStartZ = trace.m_vStartPos.z;
    out->m_flEndX = trace.m_vEndPos.x;
    out->m_flEndY = trace.m_vEndPos.y;
    out->m_flEndZ = trace.m_vEndPos.z;
    out->m_flHitNormalX = trace.m_vHitNormal.x;
    out->m_flHitNormalY = trace.m_vHitNormal.y;
    out->m_flHitNormalZ = trace.m_vHitNormal.z;
    out->m_flHitPointX = trace.m_vHitPoint.x;
    out->m_flHitPointY = trace.m_vHitPoint.y;
    out->m_flHitPointZ = trace.m_vHitPoint.z;

    out->m_flHitOffset = trace.m_flHitOffset;
    out->m_flFraction = trace.m_flFraction;
    out->m_nTriangle = trace.m_nTriangle;
    out->m_nHitboxBoneIndex = trace.m_nHitboxBoneIndex;
    out->m_eRayType = static_cast<uint8_t>(trace.m_eRayType);
    out->m_bStartInSolid = trace.m_bStartInSolid ? 1 : 0;
    out->m_bExactHitPoint = trace.m_bExactHitPoint ? 1 : 0;
}

static CTraceFilter BuildFilter(CEntityInstance* pIgnore, uint64_t interactsAs, uint64_t interactsWith, uint64_t interactsExclude)
{
    CEntityInstance* pOwner = nullptr;
    uint16 nHierarchy = static_cast<uint16>(0xFFFF);

    if (pIgnore)
    {
        static auto classKey = hash_32_fnv1a_const("CBaseEntity");
        static auto ownerKey = hash_32_fnv1a_const("m_hOwnerEntity");
        static auto collKey = hash_32_fnv1a_const("m_pCollision");
        static const auto ownerField = schema::GetOffset("CBaseEntity", classKey, "m_hOwnerEntity", ownerKey);
        static const auto collField = schema::GetOffset("CBaseEntity", classKey, "m_pCollision", collKey);

        auto ownerHandle = *reinterpret_cast<CEntityHandle*>((uintptr_t)pIgnore + ownerField.offset);
        pOwner = globals::entitySystem->GetEntityInstance(ownerHandle);

        auto* pColl = *reinterpret_cast<uint8_t**>((uintptr_t)pIgnore + collField.offset);
        if (pColl)
        {
            static auto collClassKey = hash_32_fnv1a_const("CCollisionComponent");
            static auto attrKey = hash_32_fnv1a_const("m_collisionAttribute");
            static const auto attrField = schema::GetOffset("CCollisionComponent", collClassKey, "m_collisionAttribute", attrKey);

            // m_nHierarchyId is at byte 32 inside RnCollisionAttr_t (after 3×uint64 + 2×uint32)
            nHierarchy = *reinterpret_cast<uint16*>(pColl + attrField.offset + 32);
        }
    }

    CTraceFilter filter(pIgnore, pOwner, nHierarchy, interactsWith, COLLISION_GROUP_DEFAULT, true);
    filter.m_nInteractsAs = interactsAs;
    filter.m_nInteractsExclude = interactsExclude;
    return filter;
}

static void NativeTraceShape(ScriptContext& script_context)
{
    auto* vecStart = script_context.GetArgument<Vector*>(0);
    auto* angAngles = script_context.GetArgument<QAngle*>(1);
    auto* pIgnore = script_context.GetArgument<CEntityInstance*>(2);
    uint64_t interactsAs = script_context.GetArgument<uint64_t>(3);
    uint64_t interactsWith = script_context.GetArgument<uint64_t>(4);
    uint64_t interactsExclude = script_context.GetArgument<uint64_t>(5);
    auto* out = script_context.GetArgument<CSSTraceResult*>(6);

    Vector forward;
    AngleVectors(*angAngles, &forward);
    Vector vecEnd{ vecStart->x + forward.x * 8192.f, vecStart->y + forward.y * 8192.f, vecStart->z + forward.z * 8192.f };

    CTraceFilter filter = BuildFilter(pIgnore, interactsAs, interactsWith, interactsExclude);
    Ray_t ray;
    CGameTrace trace;
    INavPhysicsInterface::TraceShape(ray, *vecStart, vecEnd, &filter, &trace);
    FillResult(out, trace);
}

static void NativeTraceEndShape(ScriptContext& script_context)
{
    auto* vecStart = script_context.GetArgument<Vector*>(0);
    auto* vecEnd = script_context.GetArgument<Vector*>(1);
    auto* pIgnore = script_context.GetArgument<CEntityInstance*>(2);
    uint64_t interactsAs = script_context.GetArgument<uint64_t>(3);
    uint64_t interactsWith = script_context.GetArgument<uint64_t>(4);
    uint64_t interactsExclude = script_context.GetArgument<uint64_t>(5);
    auto* out = script_context.GetArgument<CSSTraceResult*>(6);

    CTraceFilter filter = BuildFilter(pIgnore, interactsAs, interactsWith, interactsExclude);
    Ray_t ray;
    CGameTrace trace;
    INavPhysicsInterface::TraceShape(ray, *vecStart, *vecEnd, &filter, &trace);
    FillResult(out, trace);
}

static void NativeTraceHullShape(ScriptContext& script_context)
{
    auto* vecStart = script_context.GetArgument<Vector*>(0);
    auto* vecEnd = script_context.GetArgument<Vector*>(1);
    auto* vecMins = script_context.GetArgument<Vector*>(2);
    auto* vecMaxs = script_context.GetArgument<Vector*>(3);
    auto* pIgnore = script_context.GetArgument<CEntityInstance*>(4);
    uint64_t interactsAs = script_context.GetArgument<uint64_t>(5);
    uint64_t interactsWith = script_context.GetArgument<uint64_t>(6);
    uint64_t interactsExclude = script_context.GetArgument<uint64_t>(7);
    auto* out = script_context.GetArgument<CSSTraceResult*>(8);

    CTraceFilter filter = BuildFilter(pIgnore, interactsAs, interactsWith, interactsExclude);
    Ray_t ray(*vecMins, *vecMaxs);
    CGameTrace trace;
    INavPhysicsInterface::TraceShape(ray, *vecStart, *vecEnd, &filter, &trace);
    FillResult(out, trace);
}

static uint64_t NativePointContents(ScriptContext& script_context)
{
    auto* vPos = script_context.GetArgument<Vector*>(0);
    uint64_t contentsMask = script_context.GetArgument<uint64_t>(1);
    uint64_t result = INavPhysicsInterface::PointContents(vPos, contentsMask);
    script_context.SetResult<uint64_t>(result);
    return 0;
}

static bool NativeCheckAreaOverlappingEntity(ScriptContext& script_context)
{
    auto* rArea = script_context.GetArgument<const void* const>(0);
    auto* rEntity = script_context.GetArgument<const CBaseEntity* const>(1);
    auto bExtrudeHullHeight = script_context.GetArgument<bool>(2);
    bool result = INavPhysicsInterface::CheckAreaOverlappingEntity(rArea, rEntity, bExtrudeHullHeight);
    script_context.SetResult<bool>(result);
    return 0;
}

static void NativeGetEntityWorldSpaceAABB(ScriptContext& script_context)
{
    auto* rEntity = script_context.GetArgument<const CBaseEntity*>(0);
    auto* pMinsOut = script_context.GetArgument<Vector*>(1);
    auto* pMaxsOut = script_context.GetArgument<Vector*>(2);
    INavPhysicsInterface::GetEntityWorldSpaceAABB(rEntity, pMinsOut, pMaxsOut);
}

REGISTER_NATIVES(raytrace, {
    ScriptEngine::RegisterNativeHandler("TRACE_SHAPE", NativeTraceShape);
    ScriptEngine::RegisterNativeHandler("TRACE_END_SHAPE", NativeTraceEndShape);
    ScriptEngine::RegisterNativeHandler("TRACE_HULL_SHAPE", NativeTraceHullShape);
    ScriptEngine::RegisterNativeHandler("POINT_CONTENTS", NativePointContents);
    ScriptEngine::RegisterNativeHandler("CHECK_AREA_OVERLAPPING_ENTITY", NativeCheckAreaOverlappingEntity);
    ScriptEngine::RegisterNativeHandler("GET_ENTITY_WORLD_SPACE_AABB", NativeGetEntityWorldSpaceAABB);
})

} // namespace counterstrikesharp
