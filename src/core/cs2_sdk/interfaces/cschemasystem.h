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
#include "../../utils/virtual.h"
#include <platform.h>

struct CSchemaNetworkValue {
	union {
		const char* m_sz_value;
		int m_n_value;
		float m_f_value;
		uintptr_t m_p_value;
	};
};

struct SchemaMetadataEntryData_t {
	const char *m_name;
	CSchemaNetworkValue* m_value;
};


struct SchemaClassFieldData_t
{
	const char *m_name;
	char pad0[0x8];
	short m_offset;
	int32_t m_metadata_size;
	SchemaMetadataEntryData_t* m_metadata;
};

class SchemaClassInfoData_t;

struct SchemaBaseClassInfoData_t
{
	unsigned int m_offset;
	SchemaClassInfoData_t *m_class;
};

class SchemaClassInfoData_t
{
public:
	auto GetName()
	{
		return m_name;
	}

	auto GetFieldsSize()
	{
		return m_align;
	}

	auto GetFields()
	{
		return m_fields;
	}

	SchemaClassInfoData_t* GetParent()
	{
		if (!m_schema_parent)
			return nullptr;

		return m_schema_parent->m_class;
	}

private:
	char pad_0x0000[0x8];						// 0x0000

	const char *m_name;							// 0x0008
	char *m_module;								// 0x0010

	int m_size;									// 0x0018
	int16_t m_align;							// 0x001C

	int16_t m_static_size;						// 0x001E
	int16_t m_metadata_size;					// 0x0020
	int16_t m_i_unk1;							// 0x0022
	int16_t m_i_unk2;							// 0x0024
	int16_t m_i_unk3;							// 0x0026

	SchemaClassFieldData_t *m_fields;			// 0x0028

	char pad_0x0030[0x8];						// 0x0030
	SchemaBaseClassInfoData_t *m_schema_parent; // 0x0038

	char pad_0x0038[0x10];						// 0x0038
};

class CSchemaSystemTypeScope
{
public:
	SchemaClassInfoData_t* FindDeclaredClass(const char *pClass)
	{
#ifdef _WIN32
		SchemaClassInfoData_t *rv = nullptr;
		CALL_VIRTUAL(void, 2, this, &rv, pClass);
		return rv;
#else
		return CALL_VIRTUAL(SchemaClassInfoData_t*, 2, this, pClass);
#endif
	}
};

class CSchemaSystem
{
public:
	auto FindTypeScopeForModule(const char *module)
	{
		return CALL_VIRTUAL(CSchemaSystemTypeScope *, 13, this, module, nullptr);
	}
};