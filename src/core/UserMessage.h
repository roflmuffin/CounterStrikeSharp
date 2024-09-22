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

#include <networksystem/inetworkmessages.h>

#include "core/globals.h"
#include "log.h"
#include "networkbasetypes.pb.h"

using namespace google;

#define GETCHECK_FIELD()                                                                          \
    const protobuf::FieldDescriptor* field = msg->GetDescriptor()->FindFieldByName(pszFieldName); \
    if (!field)                                                                                   \
    {                                                                                             \
        return false;                                                                             \
    }

#define CHECK_FIELD_TYPE(type)                                          \
    if (field->cpp_type() != protobuf::FieldDescriptor::CPPTYPE_##type) \
    {                                                                   \
        return false;                                                   \
    }

#define CHECK_FIELD_TYPE2(type1, type2)                                                                                     \
    protobuf::FieldDescriptor::CppType fieldType = field->cpp_type();                                                       \
    if (fieldType != protobuf::FieldDescriptor::CPPTYPE_##type1 && fieldType != protobuf::FieldDescriptor::CPPTYPE_##type2) \
    {                                                                                                                       \
        return false;                                                                                                       \
    }

#define CHECK_FIELD_TYPE3(type1, type2, type3)                                                                                \
    protobuf::FieldDescriptor::CppType fieldType = field->cpp_type();                                                         \
    if (fieldType != protobuf::FieldDescriptor::CPPTYPE_##type1 && fieldType != protobuf::FieldDescriptor::CPPTYPE_##type2 && \
        fieldType != protobuf::FieldDescriptor::CPPTYPE_##type3)                                                              \
    {                                                                                                                         \
        return false;                                                                                                         \
    }

#define CHECK_FIELD_REPEATED()                                       \
    if (field->label() != protobuf::FieldDescriptor::LABEL_REPEATED) \
    {                                                                \
        return false;                                                \
    }

#define CHECK_FIELD_NOT_REPEATED()                                   \
    if (field->label() == protobuf::FieldDescriptor::LABEL_REPEATED) \
    {                                                                \
        return false;                                                \
    }

#define CHECK_REPEATED_ELEMENT(idx)                               \
    int elemCount = msg->GetReflection()->FieldSize(*msg, field); \
    if (elemCount == 0 || idx >= elemCount || idx < 0)            \
    {                                                             \
        return false;                                             \
    };

class INetworkMessageInternal;
namespace google::protobuf {
class Message;
}

namespace counterstrikesharp {
class ScriptCallback;

static inline size_t SafeStrcpy(char* dest, size_t maxlength, const char* src)
{
    if (!dest || !maxlength) return 0;

    char* iter = dest;
    size_t count = maxlength;
    while (*src && --count)
        *iter++ = *src++;
    *iter = '\0';

    return iter - dest;
}

class UserMessage
{
  public:
    UserMessage(INetworkMessageInternal* msgSerializable, const CNetMessage* message, int nRecipientCount, uint64* recipientMask)
        : msgSerializable(msgSerializable), nRecipientCount(nRecipientCount), recipientMask(recipientMask),
          msg(const_cast<CNetMessage*>(message)->ToPB<protobuf::Message>())
    {
    }

    UserMessage(std::string msgName)
    {
        manuallyAllocated = true;
        this->msgSerializable = globals::networkMessages->FindNetworkMessagePartial(msgName.c_str());
        if (!this->msgSerializable) return;

        this->msg = this->msgSerializable->AllocateMessage()->ToPB<protobuf::Message>();
        this->recipientMask = new uint64(0);
    }

    UserMessage(int msgId)
    {
        manuallyAllocated = true;
        this->msgSerializable = globals::networkMessages->FindNetworkMessageById(msgId);
        if (!this->msgSerializable) return;

        this->msg = this->msgSerializable->AllocateMessage()->ToPB<protobuf::Message>();
        this->recipientMask = new uint64(0);
    }

    ~UserMessage()
    {
        if (manuallyAllocated) delete this->recipientMask;
    }

    std::string GetMessageName();
    int GetMessageID();
    bool HasField(std::string fieldName);
    const CNetMessagePB<google::protobuf::Message>* GetProtobufMessage();
    INetworkMessageInternal* GetSerializableMessage() { return msgSerializable; }
    uint64* GetRecipientMask() { return recipientMask; }
    bool IsManuallyAllocated() { return manuallyAllocated; }

  private:
    CNetMessagePB<google::protobuf::Message>* msg = nullptr;
    INetworkMessageInternal* msgSerializable = nullptr;
    int nRecipientCount = 0;
    uint64* recipientMask = nullptr;
    bool manuallyAllocated = false;

  public:
    inline bool HasField(const char* pszFieldName)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_NOT_REPEATED();
        return msg->GetReflection()->HasField(*msg, field);
    }

    inline bool GetInt32(const char* pszFieldName, int32* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(INT32);
        CHECK_FIELD_NOT_REPEATED();

        *out = msg->GetReflection()->GetInt32(*msg, field);
        return true;
    }

    inline bool SetInt32(const char* pszFieldName, int32 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(INT32);
        CHECK_FIELD_NOT_REPEATED();

        msg->GetReflection()->SetInt32(msg, field, value);
        return true;
    }

    inline bool GetRepeatedInt32(const char* pszFieldName, int index, int32* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(INT32);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        *out = msg->GetReflection()->GetRepeatedInt32(*msg, field, index);
        return true;
    }

    inline bool SetRepeatedInt32(const char* pszFieldName, int index, int32 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(INT32);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        msg->GetReflection()->SetRepeatedInt32(msg, field, index, value);
        return true;
    }

    inline bool AddInt32(const char* pszFieldName, int32 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(INT32);
        CHECK_FIELD_REPEATED();

        msg->GetReflection()->AddInt32(msg, field, value);
        return true;
    }

    inline bool GetInt64(const char* pszFieldName, int64* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(INT64);
        CHECK_FIELD_NOT_REPEATED();

        *out = msg->GetReflection()->GetInt64(*msg, field);
        return true;
    }

    inline bool SetInt64(const char* pszFieldName, int64 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(INT64);
        CHECK_FIELD_NOT_REPEATED();

        msg->GetReflection()->SetInt64(msg, field, value);
        return true;
    }

    inline bool GetRepeatedInt64(const char* pszFieldName, int index, int64* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(INT64);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        *out = msg->GetReflection()->GetRepeatedInt64(*msg, field, index);
        return true;
    }

    inline bool SetRepeatedInt64(const char* pszFieldName, int index, int64 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(INT64);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        msg->GetReflection()->SetRepeatedInt64(msg, field, index, value);
        return true;
    }

    inline bool AddInt64(const char* pszFieldName, int64 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(INT64);
        CHECK_FIELD_REPEATED();

        msg->GetReflection()->AddInt64(msg, field, value);
        return true;
    }

    inline bool GetUInt32(const char* pszFieldName, uint32* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(UINT32);
        CHECK_FIELD_NOT_REPEATED();

        *out = msg->GetReflection()->GetUInt32(*msg, field);
        return true;
    }

    inline bool SetUInt32(const char* pszFieldName, uint32 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(UINT32);
        CHECK_FIELD_NOT_REPEATED();

        msg->GetReflection()->SetUInt32(msg, field, value);
        return true;
    }

    inline bool GetRepeatedUInt32(const char* pszFieldName, int index, uint32* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(UINT32);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        *out = msg->GetReflection()->GetRepeatedUInt32(*msg, field, index);
        return true;
    }

    inline bool SetRepeatedUInt32(const char* pszFieldName, int index, uint32 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(UINT32);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        msg->GetReflection()->SetRepeatedUInt32(msg, field, index, value);
        return true;
    }

    inline bool AddUInt32(const char* pszFieldName, uint32 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(UINT32);
        CHECK_FIELD_REPEATED();

        msg->GetReflection()->AddUInt32(msg, field, value);
        return true;
    }

    inline bool GetUInt64(const char* pszFieldName, uint64* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(UINT64);
        CHECK_FIELD_NOT_REPEATED();

        *out = msg->GetReflection()->GetUInt64(*msg, field);
        return true;
    }

    inline bool SetUInt64(const char* pszFieldName, uint64 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(UINT64);
        CHECK_FIELD_NOT_REPEATED();

        msg->GetReflection()->SetUInt64(msg, field, value);
        return true;
    }

    inline bool GetRepeatedUInt64(const char* pszFieldName, int index, uint64* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(UINT64);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        *out = msg->GetReflection()->GetRepeatedUInt64(*msg, field, index);
        return true;
    }

    inline bool SetRepeatedUInt64(const char* pszFieldName, int index, uint64 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(UINT64);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        msg->GetReflection()->SetRepeatedUInt64(msg, field, index, value);
        return true;
    }

    inline bool AddUInt64(const char* pszFieldName, uint64 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(UINT64);
        CHECK_FIELD_REPEATED();

        msg->GetReflection()->AddUInt32(msg, field, value);
        return true;
    }

    inline bool GetInt32OrUnsignedOrEnum(const char* pszFieldName, int32* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE3(INT32, UINT32, ENUM);
        CHECK_FIELD_NOT_REPEATED();

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_UINT32) *out = (int32)msg->GetReflection()->GetUInt32(*msg, field);
        else if (fieldType == protobuf::FieldDescriptor::CPPTYPE_INT32)
            *out = msg->GetReflection()->GetInt32(*msg, field);
        else // CPPTYPE_ENUM
            *out = msg->GetReflection()->GetEnum(*msg, field)->number();

        return true;
    }

    inline bool GetInt64OrUnsigned(const char* pszFieldName, int64* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE2(INT64, UINT64);
        CHECK_FIELD_NOT_REPEATED();

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_UINT64) *out = (int64)msg->GetReflection()->GetUInt64(*msg, field);
        else
            *out = msg->GetReflection()->GetInt64(*msg, field);

        return true;
    }

    inline bool SetInt32OrUnsignedOrEnum(const char* pszFieldName, int32 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE3(INT32, UINT32, ENUM);
        CHECK_FIELD_NOT_REPEATED();

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_UINT32)
        {
            msg->GetReflection()->SetUInt32(msg, field, (uint32)value);
        }
        else if (fieldType == protobuf::FieldDescriptor::CPPTYPE_INT32)
        {
            msg->GetReflection()->SetInt32(msg, field, value);
        }
        else // CPPTYPE_ENUM
        {
            const protobuf::EnumValueDescriptor* pEnumValue = field->enum_type()->FindValueByNumber(value);
            if (!pEnumValue) return false;

            msg->GetReflection()->SetEnum(msg, field, pEnumValue);
        }

        return true;
    }

    inline bool SetInt64OrUnsigned(const char* pszFieldName, int64 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE2(INT64, UINT64);
        CHECK_FIELD_NOT_REPEATED();

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_UINT64)
        {
            msg->GetReflection()->SetUInt64(msg, field, (uint64)value);
        }
        else
        {
            msg->GetReflection()->SetInt64(msg, field, value);
        }

        return true;
    }

    inline bool GetRepeatedInt32OrUnsignedOrEnum(const char* pszFieldName, int index, int32* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE3(INT32, UINT32, ENUM);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_UINT32)
            *out = (int32)msg->GetReflection()->GetRepeatedUInt32(*msg, field, index);
        else if (fieldType == protobuf::FieldDescriptor::CPPTYPE_INT32)
            *out = msg->GetReflection()->GetRepeatedInt32(*msg, field, index);
        else // CPPTYPE_ENUM
            *out = msg->GetReflection()->GetRepeatedEnum(*msg, field, index)->number();

        return true;
    }

    inline bool GetRepeatedInt64OrUnsigned(const char* pszFieldName, int index, int64* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE2(INT64, UINT64);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_UINT64)
            *out = (int64)msg->GetReflection()->GetRepeatedUInt64(*msg, field, index);
        else
            *out = msg->GetReflection()->GetRepeatedInt64(*msg, field, index);

        return true;
    }

    inline bool SetRepeatedInt32OrUnsignedOrEnum(const char* pszFieldName, int index, int32 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE3(INT32, UINT32, ENUM);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_UINT32)
        {
            msg->GetReflection()->SetRepeatedUInt32(msg, field, index, (uint32)value);
        }
        else if (fieldType == protobuf::FieldDescriptor::CPPTYPE_INT32)
        {
            msg->GetReflection()->SetRepeatedInt32(msg, field, index, value);
        }
        else // CPPTYPE_ENUM
        {
            const protobuf::EnumValueDescriptor* pEnumValue = field->enum_type()->FindValueByNumber(value);
            if (!pEnumValue) return false;

            msg->GetReflection()->SetRepeatedEnum(msg, field, index, pEnumValue);
        }

        return true;
    }

    inline bool SetRepeatedInt64OrUnsigned(const char* pszFieldName, int index, int64 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE2(INT64, UINT64);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_UINT64)
        {
            msg->GetReflection()->SetRepeatedUInt64(msg, field, index, (uint64)value);
        }
        else
        {
            msg->GetReflection()->SetRepeatedInt64(msg, field, index, value);
        }

        return true;
    }

    inline bool AddInt32OrUnsignedOrEnum(const char* pszFieldName, int32 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE3(INT32, UINT32, ENUM);
        CHECK_FIELD_REPEATED();

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_UINT32)
        {
            msg->GetReflection()->AddUInt32(msg, field, (uint32)value);
        }
        else if (fieldType == protobuf::FieldDescriptor::CPPTYPE_INT32)
        {
            msg->GetReflection()->AddInt32(msg, field, value);
        }
        else // CPPTYPE_ENUM
        {
            const protobuf::EnumValueDescriptor* pEnumValue = field->enum_type()->FindValueByNumber(value);
            if (!pEnumValue) return false;

            msg->GetReflection()->AddEnum(msg, field, pEnumValue);
        }

        return true;
    }

    inline bool AddInt64OrUnsigned(const char* pszFieldName, int64 value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE2(INT64, UINT64);
        CHECK_FIELD_REPEATED();

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_UINT64)
        {
            msg->GetReflection()->AddUInt64(msg, field, (uint64)value);
        }
        else
        {
            msg->GetReflection()->AddInt64(msg, field, value);
        }

        return true;
    }

    inline bool GetBool(const char* pszFieldName, bool* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(BOOL);
        CHECK_FIELD_NOT_REPEATED();

        *out = msg->GetReflection()->GetBool(*msg, field);
        return true;
    }

    inline bool SetBool(const char* pszFieldName, bool value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(BOOL);
        CHECK_FIELD_NOT_REPEATED();

        msg->GetReflection()->SetBool(msg, field, value);
        return true;
    }

    inline bool GetRepeatedBool(const char* pszFieldName, int index, bool* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(BOOL);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        *out = msg->GetReflection()->GetRepeatedBool(*msg, field, index);
        return true;
    }

    inline bool SetRepeatedBool(const char* pszFieldName, int index, bool value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(BOOL);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        msg->GetReflection()->SetRepeatedBool(msg, field, index, value);
        return true;
    }

    inline bool AddBool(const char* pszFieldName, bool value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(BOOL);
        CHECK_FIELD_REPEATED();

        msg->GetReflection()->AddBool(msg, field, value);
        return true;
    }

    inline bool GetFloat(const char* pszFieldName, float* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(FLOAT);
        CHECK_FIELD_NOT_REPEATED();

        *out = msg->GetReflection()->GetFloat(*msg, field);
        return true;
    }

    inline bool SetFloat(const char* pszFieldName, float value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(FLOAT);
        CHECK_FIELD_NOT_REPEATED();

        msg->GetReflection()->SetFloat(msg, field, value);
        return true;
    }

    inline bool GetRepeatedFloat(const char* pszFieldName, int index, float* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(FLOAT);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        *out = msg->GetReflection()->GetRepeatedFloat(*msg, field, index);
        return true;
    }

    inline bool SetRepeatedFloat(const char* pszFieldName, int index, float value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(FLOAT);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        msg->GetReflection()->SetRepeatedFloat(msg, field, index, value);
        return true;
    }

    inline bool AddFloat(const char* pszFieldName, float value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(FLOAT);
        CHECK_FIELD_REPEATED();

        msg->GetReflection()->AddFloat(msg, field, value);
        return true;
    }

    inline bool GetDouble(const char* pszFieldName, double* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(DOUBLE);
        CHECK_FIELD_NOT_REPEATED();

        *out = msg->GetReflection()->GetDouble(*msg, field);
        return true;
    }

    inline bool SetDouble(const char* pszFieldName, double value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(DOUBLE);
        CHECK_FIELD_NOT_REPEATED();

        msg->GetReflection()->SetDouble(msg, field, value);
        return true;
    }

    inline bool GetRepeatedDouble(const char* pszFieldName, int index, double* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(DOUBLE);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        *out = msg->GetReflection()->GetRepeatedDouble(*msg, field, index);
        return true;
    }

    inline bool SetRepeatedDouble(const char* pszFieldName, int index, double value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(DOUBLE);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        msg->GetReflection()->SetRepeatedDouble(msg, field, index, value);
        return true;
    }

    inline bool AddDouble(const char* pszFieldName, double value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(DOUBLE);
        CHECK_FIELD_REPEATED();

        msg->GetReflection()->AddDouble(msg, field, value);
        return true;
    }

    inline bool GetFloatOrDouble(const char* pszFieldName, float* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE2(FLOAT, DOUBLE);
        CHECK_FIELD_NOT_REPEATED();

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_DOUBLE) *out = (float)msg->GetReflection()->GetDouble(*msg, field);
        else
            *out = msg->GetReflection()->GetFloat(*msg, field);

        return true;
    }

    inline bool SetFloatOrDouble(const char* pszFieldName, float value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE2(FLOAT, DOUBLE);
        CHECK_FIELD_NOT_REPEATED();

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_DOUBLE) msg->GetReflection()->SetDouble(msg, field, (double)value);
        else
            msg->GetReflection()->SetFloat(msg, field, value);

        return true;
    }

    inline bool GetRepeatedFloatOrDouble(const char* pszFieldName, int index, float* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE2(FLOAT, DOUBLE);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_DOUBLE)
            *out = (float)msg->GetReflection()->GetRepeatedDouble(*msg, field, index);
        else
            *out = msg->GetReflection()->GetRepeatedFloat(*msg, field, index);

        return true;
    }

    inline bool SetRepeatedFloatOrDouble(const char* pszFieldName, int index, float value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE2(FLOAT, DOUBLE);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_DOUBLE)
            msg->GetReflection()->SetRepeatedDouble(msg, field, index, (double)value);
        else
            msg->GetReflection()->SetRepeatedFloat(msg, field, index, value);

        return true;
    }

    inline bool AddFloatOrDouble(const char* pszFieldName, float value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE2(FLOAT, DOUBLE);
        CHECK_FIELD_REPEATED();

        if (fieldType == protobuf::FieldDescriptor::CPPTYPE_DOUBLE) msg->GetReflection()->AddDouble(msg, field, (double)value);
        else
            msg->GetReflection()->AddFloat(msg, field, value);

        return true;
    }

    inline bool GetString(const char* pszFieldName, std::string& out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(STRING);
        CHECK_FIELD_NOT_REPEATED();

        out = msg->GetReflection()->GetString(*msg, field);

        return true;
    }

    inline bool SetString(const char* pszFieldName, std::string value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(STRING);
        CHECK_FIELD_NOT_REPEATED();

        msg->GetReflection()->SetString(msg, field, value);
        return true;
    }

    inline bool GetRepeatedString(const char* pszFieldName, int index, std::string& out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(STRING);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        out = msg->GetReflection()->GetRepeatedString(*msg, field, index);

        return true;
    }

    inline bool SetRepeatedString(const char* pszFieldName, int index, std::string value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(STRING);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        msg->GetReflection()->SetRepeatedString(msg, field, index, value);

        return true;
    }

    inline bool AddString(const char* pszFieldName, const char* value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(STRING);
        CHECK_FIELD_REPEATED();

        msg->GetReflection()->AddString(msg, field, value);
        return true;
    }

    inline bool GetColor(const char* pszFieldName, Color* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_NOT_REPEATED();

        auto msgRGBA = *(CMsgRGBA*)msg->GetReflection()->MutableMessage(msg, field);
        out->SetColor(msgRGBA.r(), msgRGBA.g(), msgRGBA.b(), msgRGBA.a());

        return true;
    }

    inline bool SetColor(const char* pszFieldName, const Color& value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_NOT_REPEATED();

        CMsgRGBA* msgRGBA = (CMsgRGBA*)msg->GetReflection()->MutableMessage(msg, field);
        msgRGBA->set_r(value.r());
        msgRGBA->set_g(value.g());
        msgRGBA->set_b(value.b());
        msgRGBA->set_a(value.a());

        return true;
    }

    inline bool GetRepeatedColor(const char* pszFieldName, int index, Color* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        const CMsgRGBA& msgRGBA = (const CMsgRGBA&)msg->GetReflection()->GetRepeatedMessage(*msg, field, index);
        out->SetColor(msgRGBA.r(), msgRGBA.g(), msgRGBA.b(), msgRGBA.a());

        return true;
    }

    inline bool SetRepeatedColor(const char* pszFieldName, int index, const Color& value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        CMsgRGBA* msgRGBA = (CMsgRGBA*)msg->GetReflection()->MutableRepeatedMessage(msg, field, index);
        msgRGBA->set_r(value.r());
        msgRGBA->set_g(value.g());
        msgRGBA->set_b(value.b());
        msgRGBA->set_a(value.a());

        return true;
    }

    inline bool AddColor(const char* pszFieldName, const Color& value)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_REPEATED();

        CMsgRGBA* msgRGBA = (CMsgRGBA*)msg->GetReflection()->AddMessage(msg, field);
        msgRGBA->set_r(value.r());
        msgRGBA->set_g(value.g());
        msgRGBA->set_b(value.b());
        msgRGBA->set_a(value.a());

        return true;
    }

    inline bool GetVector2D(const char* pszFieldName, Vector2D* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_NOT_REPEATED();

        auto msgVec2d = *(CMsgVector2D*)msg->GetReflection()->MutableMessage(msg, field);
        out->Init(msgVec2d.x(), msgVec2d.y());

        return true;
    }

    inline bool SetVector2D(const char* pszFieldName, Vector2D& vec)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_NOT_REPEATED();

        CMsgVector2D* msgVec2d = (CMsgVector2D*)msg->GetReflection()->MutableMessage(msg, field);
        msgVec2d->set_x(vec.x);
        msgVec2d->set_y(vec.y);

        return true;
    }

    inline bool GetRepeatedVector2D(const char* pszFieldName, int index, Vector2D* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        const CMsgVector2D& msgVec2d = (const CMsgVector2D&)msg->GetReflection()->GetRepeatedMessage(*msg, field, index);
        out->Init(msgVec2d.x(), msgVec2d.y());

        return true;
    }

    inline bool SetRepeatedVector2D(const char* pszFieldName, int index, Vector2D& vec)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        CMsgVector2D* msgVec2d = (CMsgVector2D*)msg->GetReflection()->MutableRepeatedMessage(msg, field, index);
        msgVec2d->set_x(vec.x);
        msgVec2d->set_y(vec.y);

        return true;
    }

    inline bool AddVector2D(const char* pszFieldName, Vector2D& vec)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_REPEATED();

        CMsgVector2D* msgVec2d = (CMsgVector2D*)msg->GetReflection()->AddMessage(msg, field);
        msgVec2d->set_x(vec.x);
        msgVec2d->set_y(vec.y);

        return true;
    }

    inline bool GetVector(const char* pszFieldName, Vector* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_NOT_REPEATED();

        auto msgVec = *(CMsgVector*)msg->GetReflection()->MutableMessage(msg, field);
        out->Init(msgVec.x(), msgVec.y(), msgVec.z());

        return true;
    }

    inline bool SetVector(const char* pszFieldName, Vector& vec)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_NOT_REPEATED();

        CMsgVector* msgVec = (CMsgVector*)msg->GetReflection()->MutableMessage(msg, field);
        msgVec->set_x(vec.x);
        msgVec->set_y(vec.y);
        msgVec->set_z(vec.z);

        return true;
    }

    inline bool GetRepeatedVector(const char* pszFieldName, int index, Vector* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        const CMsgVector& msgVec = (const CMsgVector&)msg->GetReflection()->GetRepeatedMessage(*msg, field, index);
        out->Init(msgVec.x(), msgVec.y(), msgVec.z());

        return true;
    }

    inline bool SetRepeatedVector(const char* pszFieldName, int index, Vector& vec)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        CMsgVector* msgVec = (CMsgVector*)msg->GetReflection()->MutableRepeatedMessage(msg, field, index);
        msgVec->set_x(vec.x);
        msgVec->set_y(vec.y);
        msgVec->set_z(vec.z);

        return true;
    }

    inline bool AddVector(const char* pszFieldName, Vector& vec)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_REPEATED();

        CMsgVector* msgVec = (CMsgVector*)msg->GetReflection()->AddMessage(msg, field);
        msgVec->set_x(vec.x);
        msgVec->set_y(vec.y);
        msgVec->set_z(vec.z);

        return true;
    }

    inline bool GetQAngle(const char* pszFieldName, QAngle* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_NOT_REPEATED();

        auto msgAng = *(CMsgQAngle*)msg->GetReflection()->MutableMessage(msg, field);
        out->Init(msgAng.x(), msgAng.y(), msgAng.z());

        return true;
    }

    inline bool SetQAngle(const char* pszFieldName, QAngle& vec)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_NOT_REPEATED();

        CMsgQAngle* msgAng = (CMsgQAngle*)msg->GetReflection()->MutableMessage(msg, field);
        msgAng->set_x(vec.x);
        msgAng->set_y(vec.y);
        msgAng->set_z(vec.z);

        return true;
    }

    inline bool GetRepeatedQAngle(const char* pszFieldName, int index, QAngle* out)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        const CMsgQAngle& msgAng = (const CMsgQAngle&)msg->GetReflection()->GetRepeatedMessage(*msg, field, index);
        out->Init(msgAng.x(), msgAng.y(), msgAng.z());

        return true;
    }

    inline bool SetRepeatedQAngle(const char* pszFieldName, int index, QAngle& vec)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        CMsgQAngle* msgAng = (CMsgQAngle*)msg->GetReflection()->MutableRepeatedMessage(msg, field, index);
        msgAng->set_x(vec.x);
        msgAng->set_y(vec.y);
        msgAng->set_z(vec.z);

        return true;
    }

    inline bool AddQAngle(const char* pszFieldName, QAngle& vec)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_REPEATED();

        CMsgQAngle* msgAng = (CMsgQAngle*)msg->GetReflection()->AddMessage(msg, field);
        msgAng->set_x(vec.x);
        msgAng->set_y(vec.y);
        msgAng->set_z(vec.z);

        return true;
    }

    inline bool GetMessage(const char* pszFieldName, protobuf::Message** message)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_NOT_REPEATED();

        *message = msg->GetReflection()->MutableMessage(msg, field);

        return true;
    }

    inline bool GetRepeatedMessage(const char* pszFieldName, int index, const protobuf::Message** message)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        const protobuf::Message* m = &msg->GetReflection()->GetRepeatedMessage(*msg, field, index);
        *message = m;

        return true;
    }

    inline bool AddMessage(const char* pszFieldName, protobuf::Message** message)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_TYPE(MESSAGE);
        CHECK_FIELD_REPEATED();

        *message = msg->GetReflection()->AddMessage(msg, field);

        return true;
    }

    inline int GetRepeatedFieldCount(const char* pszFieldName)
    {
        const protobuf::FieldDescriptor* field = msg->GetDescriptor()->FindFieldByName(pszFieldName);
        if (!field) return -1;

        if (field->label() != protobuf::FieldDescriptor::LABEL_REPEATED) return -1;

        return msg->GetReflection()->FieldSize(*msg, field);
    }

    inline bool RemoveRepeatedFieldValue(const char* pszFieldName, int index)
    {
        GETCHECK_FIELD();
        CHECK_FIELD_REPEATED();
        CHECK_REPEATED_ELEMENT(index);

        // Protobuf guarantees that repeated field values will stay in order and so must we.
        const protobuf::Reflection* pReflection = msg->GetReflection();
        for (int i = index; i < elemCount - 1; ++i)
        {
            pReflection->SwapElements(msg, field, i, i + 1);
        }

        pReflection->RemoveLast(msg, field);

        return true;
    }

    inline std::string GetDebugString() { return msg->DebugString(); }
};

} // namespace counterstrikesharp
