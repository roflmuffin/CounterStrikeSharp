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

// memdbgon must be the last include file in a .cpp file!!!
#include "tier0/memdbgon.h"

using SchemaKeyValueMap_t = CUtlMap<uint32_t, SchemaKey>;
using SchemaTableMap_t = CUtlMap<uint32_t, SchemaKeyValueMap_t*>;

bool IsFieldNetworked(SchemaClassFieldData_t& field) {
    for (int i = 0; i < field.m_metadata_size; i++) {
        static auto networkEnabled = hash_32_fnv1a_const("MNetworkEnable");
        if (networkEnabled == hash_32_fnv1a_const(field.m_metadata[i].m_name)) return true;
    }

    return false;
}

static bool InitSchemaFieldsForClass(SchemaTableMap_t* tableMap,
                                     const char* className,
                                     uint32_t classKey) {
    CSchemaSystemTypeScope* pType =
        counterstrikesharp::interfaces::pSchemaSystem->FindTypeScopeForModule(MODULE_PREFIX "server" MODULE_EXT);

    if (!pType) return false;

    SchemaClassInfoData_t* pClassInfo = pType->FindDeclaredClass(className);

    if (!pClassInfo) {
        SchemaKeyValueMap_t* map = new SchemaKeyValueMap_t(0, 0, DefLessFunc(uint32_t));
        tableMap->Insert(classKey, map);

        Warning("InitSchemaFieldsForClass(): '%s' was not found!\n", className);
        return false;
    }

    short fieldsSize = pClassInfo->m_align;
    SchemaClassFieldData_t* pFields = pClassInfo->m_fields;

    SchemaKeyValueMap_t* keyValueMap = new SchemaKeyValueMap_t(0, 0, DefLessFunc(uint32_t));
    keyValueMap->EnsureCapacity(fieldsSize);
    tableMap->Insert(classKey, keyValueMap);

    for (int i = 0; i < fieldsSize; ++i) {
        SchemaClassFieldData_t& field = pFields[i];

        keyValueMap->Insert(hash_32_fnv1a_const(field.m_name),
                            {field.m_single_inheritance_offset, IsFieldNetworked(field)});
    }

    return true;
}

int16_t schema::FindChainOffset(const char* className) {
    CSchemaSystemTypeScope* pType =
        counterstrikesharp::interfaces::pSchemaSystem->FindTypeScopeForModule(MODULE_PREFIX "server" MODULE_EXT);

    if (!pType) return false;

    auto* pClassInfo = pType->FindDeclaredClass(className);

    do {
        SchemaClassFieldData_t* pFields = pClassInfo->m_fields;
        short fieldsSize = pClassInfo->m_align;
        for (int i = 0; i < fieldsSize; ++i) {
            SchemaClassFieldData_t& field = pFields[i];

            if (V_strcmp(field.m_name, "__m_pChainEntity") == 0) {
                return field.m_single_inheritance_offset;
            }
        }
    } while ((pClassInfo = pClassInfo->GetParent()) != nullptr);

    return 0;
}

SchemaKey schema::GetOffset(const char* className,
                            uint32_t classKey,
                            const char* memberName,
                            uint32_t memberKey) {
    static SchemaTableMap_t schemaTableMap(0, 0, DefLessFunc(uint32_t));
    int16_t tableMapIndex = schemaTableMap.Find(classKey);
    if (!schemaTableMap.IsValidIndex(tableMapIndex)) {
        if (InitSchemaFieldsForClass(&schemaTableMap, className, classKey))
            return GetOffset(className, classKey, memberName, memberKey);

        return {0, 0};
    }

    SchemaKeyValueMap_t* tableMap = schemaTableMap[tableMapIndex];
    int16_t memberIndex = tableMap->Find(memberKey);
    if (!tableMap->IsValidIndex(memberIndex)) {
        return {0, 0};
    }

    return tableMap->Element(memberIndex);
}

void SetStateChanged(Z_CBaseEntity* pEntity, int offset) {
    // addresses::StateChanged(pEntity->m_NetworkTransmitComponent(), pEntity, offset, -1, -1);
    auto vars = counterstrikesharp::globals::getGlobalVars();

    // if (vars) pEntity->m_lastNetworkChange = vars->curtime;

    // pEntity->m_isSteadyState().ClearAll();
};
