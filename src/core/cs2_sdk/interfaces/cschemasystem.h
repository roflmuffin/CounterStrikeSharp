#pragma once

#include <vector>
#include <utils/virtual.h>
#include <string_view>
#include <array>
#include "core/cs2_sdk/interfaces/CUtlTSHash.h"
#define CS2

#define CSCHEMATYPE_GETSIZES_INDEX 3
#define SCHEMASYSTEM_TYPE_SCOPES_OFFSET 0x190
#define SCHEMASYSTEMTYPESCOPE_OFF1 0x47E
#define SCHEMASYSTEMTYPESCOPE_OFF2 0x2808
#define SCHEMASYSTEM_FIND_DECLARED_CLASS_TYPE 2

class CSchemaClassInfo;
class CSchemaSystemTypeScope;
class CSchemaType;

struct SchemaMetadataEntryData_t;
struct SchemaMetadataSetData_t;
struct SchemaClassInfoData_t;

using SchemaString_t = const char*;

template <typename T> struct SchemaArray
{
  public:
    T* begin() const { return m_data; }

    T* end() const { return m_data + m_size; }

    T* m_data;
    unsigned int m_size;
};

struct CSchemaNetworkValue
{
    union
    {
        const char* m_sz_value;
        int m_n_value;
        float m_f_value;
        std::uintptr_t m_p_value;
    };
};

struct CSchemaClassBinding
{
    CSchemaClassBinding* parent;
    const char* m_binary_name; // ex: C_World
    const char* m_module_name; // ex: libclient.so
    const char* m_class_name;  // ex: client
    void* m_class_info_old_synthesized;
    void* m_class_info;
    void* m_this_module_binding_pointer;
    CSchemaType* m_schema_type;
};

struct SchemaEnumeratorInfoData_t
{
    SchemaString_t m_name;

    union
    {
        unsigned char m_value_char;
        unsigned short m_value_short;
        unsigned int m_value_int;
        unsigned long long m_value;
    };

    char pad_0x0010[0x10]; // 0x0010
};

class CSchemaEnumInfo
{
  public:
    SchemaEnumeratorInfoData_t m_field_;
};

class CSchemaEnumBinding
{
  public:
    virtual const char* GetBindingName() = 0;
    virtual CSchemaClassBinding* AsClassBinding() = 0;
    virtual CSchemaEnumBinding* AsEnumBinding() = 0;
    virtual const char* GetBinaryName() = 0;
    virtual const char* GetProjectName() = 0;

  public:
    char* m_binding_name_; // 0x0008
    char* m_dll_name_;     // 0x0010
    std::int8_t m_align_;  // 0x0018
    char pad_0x0019[0x3];  // 0x0019
    std::int16_t m_size_;  // 0x001C
    std::int16_t m_flags_; // 0x001E
    SchemaEnumeratorInfoData_t* m_enum_info_;
    char pad_0x0028[0x8];                  // 0x0028
    CSchemaSystemTypeScope* m_type_scope_; // 0x0030
    char pad_0x0038[0x8];                  // 0x0038
    std::int32_t m_i_unk1_;                // 0x0040
};

enum SchemaClassFlags_t
{
    SCHEMA_CLASS_HAS_VIRTUAL_MEMBERS = 1,
    SCHEMA_CLASS_IS_ABSTRACT = 2,
    SCHEMA_CLASS_HAS_TRIVIAL_CONSTRUCTOR = 4,
    SCHEMA_CLASS_HAS_TRIVIAL_DESTRUCTOR = 8,
    SCHEMA_CLASS_TEMP_HACK_HAS_NOSCHEMA_MEMBERS = 16,
    SCHEMA_CLASS_TEMP_HACK_HAS_CONSTRUCTOR_LIKE_METHODS = 32,
    SCHEMA_CLASS_TEMP_HACK_HAS_DESTRUCTOR_LIKE_METHODS = 64,
    SCHEMA_CLASS_IS_NOSCHEMA_CLASS = 128,
};

enum ETypeCategory
{
    Schema_Builtin = 0,
    Schema_Ptr = 1,
    Schema_Bitfield = 2,
    Schema_FixedArray = 3,
    Schema_Atomic = 4,
    Schema_DeclaredClass = 5,
    Schema_DeclaredEnum = 6,
    Schema_None = 7
};

enum EAtomicCategory
{
    Atomic_Basic,
    Atomic_T,
    Atomic_CollectionOfT,
    Atomic_TT,
    Atomic_I,
    Atomic_None
};

#define __thiscall

class CSchemaType
{
  public:
    bool GetSizes(int* out_size1, uint8_t* unk_probably_not_size)
    {
        return reinterpret_cast<int(__thiscall*)(void*, int*, uint8_t*)>(
            _vtable[CSCHEMATYPE_GETSIZES_INDEX])(this, out_size1, unk_probably_not_size);
    }

  public:
    bool GetSize(int* out_size)
    {
        uint8_t smh = 0;
        return GetSizes(out_size, &smh);
    }

  public:
    uintptr_t* _vtable;  // 0x0000
    const char* m_name_; // 0x0008

    CSchemaSystemTypeScope* m_type_scope_; // 0x0010
    uint8_t type_category;                 // ETypeCategory 0x0018
    uint8_t atomic_category;               // EAtomicCategory 0x0019

    // find out to what class pointer points.
    CSchemaType* GetRefClass()
    {
        if (type_category != Schema_Ptr)
            return nullptr;

        auto ptr = m_schema_type_;
        while (ptr && ptr->type_category == ETypeCategory::Schema_Ptr)
            ptr = ptr->m_schema_type_;

        return ptr;
    }

    struct array_t
    {
        uint32_t array_size;
        uint32_t unknown;
        CSchemaType* element_type_;
    };

    struct generic_type_t
    {
        uint64_t unknown;
        const char* m_name_; // 0x0008
    };

    struct atomic_t
    { // same goes for CollectionOfT
        generic_type_t* generic_type;
        uint64_t unknown;
        CSchemaType* template_typename;
    };

    struct atomic_tt
    {
        uint64_t gap[2];
        CSchemaType* templates[2];
    };

    struct atomic_i
    {
        uint64_t gap[2];
        uint64_t integer;
    };

    // this union depends on CSchema implementation, all members above
    // is from base class ( CSchemaType )
    union // 0x020
    {
        CSchemaType* m_schema_type_;
        CSchemaClassInfo* m_class_info;
        CSchemaEnumBinding* m_enum_binding_;
        array_t m_array_;
        atomic_t m_atomic_t_;
        atomic_tt m_atomic_tt_;
        atomic_i m_atomic_i_;
    };
};
static_assert(offsetof(CSchemaType, m_schema_type_) == 0x20);

struct SchemaMetadataEntryData_t
{
    SchemaString_t m_name;
    CSchemaNetworkValue* m_value;
    // CSchemaType* m_pDataType;
    // void* unaccounted;
};

struct SchemaMetadataSetData_t
{
    SchemaMetadataEntryData_t m_static_entries;
};

struct SchemaClassFieldData_t
{
    SchemaString_t m_name;                    // 0x0000
    CSchemaType* m_type;                      // 0x0008
    std::int32_t m_single_inheritance_offset; // 0x0010
    std::int32_t m_metadata_size;             // 0x0014
    SchemaMetadataEntryData_t* m_metadata;    // 0x0018
};

struct SchemaStaticFieldData_t
{
    const char* name;      // 0x0000
    CSchemaType* m_type;   // 0x0008
    void* m_instance;      // 0x0010
    char pad_0x0018[0x10]; // 0x0018
};

struct SchemaBaseClassInfoData_t
{
    unsigned int m_offset;
    CSchemaClassInfo* m_class;
};

// Classes
struct SchemaClassInfoData_t
{
    char pad_0x0000[0x8]; // 0x0000

    const char* m_name; // 0x0008
    char* m_module;     // 0x0010

    int m_size;           // 0x0018
    std::int16_t m_align; // 0x001C

    std::int16_t m_static_size;   // 0x001E
    std::int16_t m_metadata_size; // 0x0020
    std::int16_t m_i_unk1;        // 0x0022
    std::int16_t m_i_unk2;        // 0x0024
    std::int16_t m_i_unk3;        // 0x0026

    SchemaClassFieldData_t* m_fields; // 0x0028

    SchemaStaticFieldData_t* m_static_fields;   // 0x0030
    SchemaBaseClassInfoData_t* m_schema_parent; // 0x0038

    char pad_0x0038[0x10]; // 0x0038

    SchemaMetadataSetData_t* m_metadata;  // 0x0048
    CSchemaSystemTypeScope* m_type_scope; // 0x0050
    CSchemaType* m_shema_type;            // 0x0058
    SchemaClassFlags_t m_class_flags : 8; // 0x0060
};

class CSchemaClassInfo : public SchemaClassInfoData_t
{
  public:
    bool GetMetaStrings(const char* metaName, std::vector<const char**>& strings);

    unsigned int CalculateInheritanceDataSize(bool ignoreVirtuals = false);

    bool DependsOn(CSchemaClassInfo* other);
    bool InheritsFrom(CSchemaClassInfo* other);
    bool UsesClass(CSchemaClassInfo* other);
    bool InheritsVirtuals();

    void FillClassFieldsList(std::vector<SchemaClassFieldData_t*>& fields);
    void FillInheritanceList(std::vector<const char*>& inheritance);
    CSchemaClassInfo* GetParent()
    {
        if (!m_schema_parent)
            return nullptr;

        return m_schema_parent->m_class;
    }

  private:
};

class CSchemaSystemTypeScope
{
  public:
    CSchemaClassInfo* FindDeclaredClass(const char* class_name)
    {
#ifdef _WIN32
        CSchemaClassInfo* rv = nullptr;
        CALL_VIRTUAL(void, 2, this, &rv, class_name);
        return rv;
#else
        return CALL_VIRTUAL(CSchemaClassInfo*, 2, this, class_name);
#endif
    }

    CSchemaEnumBinding* FindDeclaredEnum(const char* name)
    {
#ifdef _WIN32
        CSchemaEnumBinding* rv = nullptr;
        CALL_VIRTUAL(void, 3, this, &rv, name);
        return rv;
#else
        return CALL_VIRTUAL(CSchemaEnumBinding*, 3, this, name);
#endif
    }

    CSchemaType* FindSchemaTypeByName(const char* name, std::uintptr_t* pSchema)
    {
        return CALL_VIRTUAL(CSchemaType*, 4, this, name, pSchema);
    }

    CSchemaType* FindTypeDeclaredClass(const char* name)
    {
        return CALL_VIRTUAL(CSchemaType*, 5, this, name);
    }

    CSchemaType* FindTypeDeclaredEnum(const char* name)
    {
        return CALL_VIRTUAL(CSchemaType*, 6, this, name);
    }

    CSchemaClassBinding* FindRawClassBinding(const char* name)
    {
        return CALL_VIRTUAL(CSchemaClassBinding*, 7, this, name);
    }

    CSchemaEnumBinding* FindRawEnumBinding(const char* name)
    {
        return CALL_VIRTUAL(CSchemaEnumBinding*, 8, this, name);
    }

    std::string_view GetScopeName() { return {m_name_.data()}; }

    [[nodiscard]] CUtlTSHash<CSchemaClassBinding*> GetClasses() const { return m_classes_; }

    [[nodiscard]] CUtlTSHash<CSchemaEnumBinding*> GetEnums() const { return m_enumes_; }

  private:
    char pad_0x0000[0x8]; // 0x0000
    std::array<char, 256> m_name_ = {};
    char pad_0x0108[SCHEMASYSTEMTYPESCOPE_OFF1]; // 0x0108
    CUtlTSHash<CSchemaClassBinding*> m_classes_; // 0x0588
    char pad_0x0594[SCHEMASYSTEMTYPESCOPE_OFF2]; // 0x05C8
    CUtlTSHash<CSchemaEnumBinding*> m_enumes_;   // 0x2DD0
  private:
    static constexpr unsigned int s_class_list = 0x580;
};

class CSchemaSystem
{
  public:
    CSchemaSystemTypeScope* GlobalTypeScope(void)
    {
        return CALL_VIRTUAL(CSchemaSystemTypeScope*, 11, this);
    }

    CSchemaSystemTypeScope* FindTypeScopeForModule(const char* m_module_name)
    {
        return CALL_VIRTUAL(CSchemaSystemTypeScope*, 13, this, m_module_name, nullptr);
    }
};
