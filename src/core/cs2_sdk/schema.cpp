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

#include "schema.h"

#include "interfaces/cs2_interfaces.h"
#include "core/globals.h"
#include "core/memory.h"
#include "core/log.h"

#include "tier1/utlmap.h"
#include <schemasystem.h>
#include <entity2/entitysystem.h>
#include <entity2/entityclass.h>
#include <networksystem/inetworkserializer.h>

// memdbgon must be the last include file in a .cpp file!!!
#include "tier0/memdbgon.h"

using SchemaKeyValueMap_t = CUtlMap<uint32_t, SchemaKey>;
using SchemaTableMap_t = CUtlMap<uint32_t, SchemaKeyValueMap_t*>;

static CNetworkSerializerCodeGenDatabase* GetNetworkSerializerDatabase()
{
    if (!GameEntitySystem()) return nullptr;

    CEntityClass* pEntityClass = GameEntitySystem()->FindClassByName("CBaseEntity");
    if (!pEntityClass || !pEntityClass->m_NetworkSerializerInfo) return nullptr;

    return pEntityClass->m_NetworkSerializerInfo->m_pDatabase;
}

static CNetworkSerializerClassInfo* FindNetworkSerializerClassInfo(const char* className)
{
    CNetworkSerializerCodeGenDatabase* pDatabase = GetNetworkSerializerDatabase();
    if (!pDatabase) return nullptr;

    auto index = pDatabase->m_ClassInfos.Find(className);
    if (index == pDatabase->m_ClassInfos.InvalidIndex()) return nullptr;

    return pDatabase->m_ClassInfos[index];
}

static bool IsFieldNetworked(CNetworkSerializerClassInfo* pNetworkClassInfo, SchemaClassFieldData_t& field)
{
    if (!pNetworkClassInfo) return false;
    return pNetworkClassInfo->FindField(field.m_pszName) != nullptr;
}

static bool InitSchemaFieldsForClass(SchemaTableMap_t* tableMap, const char* className, uint32_t classKey)
{
    CSchemaSystemTypeScope* pType = counterstrikesharp::globals::schemaSystem->FindTypeScopeForModule(MODULE_PREFIX "server" MODULE_EXT);

    if (!pType) return false;

    SchemaClassInfoData_t* pClassInfo = pType->FindDeclaredClass(className).Get();

    if (!pClassInfo)
    {
        SchemaKeyValueMap_t* map = new SchemaKeyValueMap_t(0, 0, DefLessFunc(uint32_t));
        tableMap->Insert(classKey, map);

        Warning("InitSchemaFieldsForClass(): '%s' was not found!\n", className);
        return false;
    }

    short fieldsSize = pClassInfo->m_nFieldCount;
    SchemaClassFieldData_t* pFields = pClassInfo->m_pFields;

    CNetworkSerializerClassInfo* pNetworkClassInfo = FindNetworkSerializerClassInfo(className);

    SchemaKeyValueMap_t* keyValueMap = new SchemaKeyValueMap_t(0, 0, DefLessFunc(uint32_t));
    keyValueMap->EnsureCapacity(fieldsSize);
    tableMap->Insert(classKey, keyValueMap);

    for (int i = 0; i < fieldsSize; ++i)
    {
        SchemaClassFieldData_t& field = pFields[i];

        keyValueMap->Insert(hash_32_fnv1a_const(field.m_pszName),
                            { field.m_nSingleInheritanceOffset, IsFieldNetworked(pNetworkClassInfo, field) });
    }

    return true;
}

int16_t schema::FindChainOffset(const char* className)
{
    CSchemaSystemTypeScope* pType = counterstrikesharp::globals::schemaSystem->FindTypeScopeForModule(MODULE_PREFIX "server" MODULE_EXT);

    if (!pType) return false;

    auto* pClassInfo = pType->FindDeclaredClass(className).Get();

    do
    {
        SchemaClassFieldData_t* pFields = pClassInfo->m_pFields;
        short fieldsSize = pClassInfo->m_nFieldCount;
        for (int i = 0; i < fieldsSize; ++i)
        {
            SchemaClassFieldData_t& field = pFields[i];

            if (V_strcmp(field.m_pszName, "__m_pChainEntity") == 0)
            {
                return field.m_nSingleInheritanceOffset;
            }
        }
    } while ((pClassInfo = pClassInfo->m_pBaseClasses->m_pClass) != nullptr);

    return 0;
}

SchemaKey schema::GetOffset(const char* className, uint32_t classKey, const char* memberName, uint32_t memberKey)
{
    static SchemaTableMap_t schemaTableMap(0, 0, DefLessFunc(uint32_t));
    int16_t tableMapIndex = schemaTableMap.Find(classKey);
    if (!schemaTableMap.IsValidIndex(tableMapIndex))
    {
        if (InitSchemaFieldsForClass(&schemaTableMap, className, classKey)) return GetOffset(className, classKey, memberName, memberKey);

        return { 0, 0 };
    }

    SchemaKeyValueMap_t* tableMap = schemaTableMap[tableMapIndex];
    int16_t memberIndex = tableMap->Find(memberKey);
    if (!tableMap->IsValidIndex(memberIndex))
    {
        return { 0, 0 };
    }

    return tableMap->Element(memberIndex);
}

void NetworkStateChanged(uintptr_t chainEntity, uint32_t offset, uint32_t nArrayIndex, uint32_t nPathIndex)
{
    CNetworkStateChangedInfo info(offset, nArrayIndex, nPathIndex);

    if (counterstrikesharp::globals::NetworkStateChanged)
        counterstrikesharp::globals::NetworkStateChanged(reinterpret_cast<void*>(chainEntity), info);
}

void SetStateChanged(uintptr_t pEntity, uint32_t offset, uint32_t nArrayIndex, uint32_t nPathIndex)
{
    CNetworkStateChangedInfo info(offset, nArrayIndex, nPathIndex);

    static auto fnOffset = counterstrikesharp::globals::gameConfig->GetOffset("SetStateChanged");
    CALL_VIRTUAL(void, fnOffset, (void*)pEntity, &info);
}
