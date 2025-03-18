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
// clang-format off

#include "core/ClientMessage.h"
#include "core/globals.h"
#include "core/log.h"
#include "core/managers/clientmessage_manager.h"
#include "igameeventsystem.h"
#include "scripting/autonative.h"

#include "core/recipientfilters.h"

// clang-format on

namespace counterstrikesharp {

#define GET_MESSAGE_OR_ERR()                                     \
    auto message = scriptContext.GetArgument<ClientMessage*>(0); \
    if (message == nullptr || message->GetMessageID() < 0)       \
    {                                                            \
        scriptContext.ThrowNativeError("Invalid message");       \
        return;                                                  \
    }

#define GET_FIELD_NAME_OR_ERR() const char* fieldName = scriptContext.GetArgument<const char*>(1);

std::vector<ClientMessage*> managed_clientmessages;

static void HookClientMessage(ScriptContext& script_context)
{
    auto messageId = script_context.GetArgument<int>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);
    auto mode = script_context.GetArgument<HookMode>(2);

    globals::clientMessageManager.HookClientMessage(messageId, callback, mode);
}

static void UnhookClientMessage(ScriptContext& script_context)
{
    auto messageId = script_context.GetArgument<int>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);
    auto mode = script_context.GetArgument<HookMode>(2);

    globals::clientMessageManager.UnhookClientMessage(messageId, callback, mode);
}

static void GetInt32OrUnsignedOrEnum(ScriptContext& script_context)
{
    auto message = script_context.GetArgument<ClientMessage*>(0);
    auto fieldName = script_context.GetArgument<const char*>(1);

    int returnValue;
    if (!message->GetInt32OrUnsignedOrEnum(fieldName, &returnValue))
    {
        script_context.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                        message->GetProtobufMessage()->GetTypeName().c_str());
        return;
    }

    script_context.SetResult(returnValue);
}

static void SetInt32OrUnsignedOrEnum(ScriptContext& script_context)
{
    auto message = script_context.GetArgument<ClientMessage*>(0);
    auto fieldName = script_context.GetArgument<const char*>(1);
    auto value = script_context.GetArgument<int>(2);

    if (!message->SetInt32OrUnsignedOrEnum(fieldName, value))
    {
        script_context.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                        message->GetProtobufMessage()->GetTypeName().c_str());
    }
}

static void PbReadInt(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    int returnValue;

    auto index = scriptContext.GetArgument<int>(2);

    if (index < 0)
    {
        if (!message->GetInt32OrUnsignedOrEnum(fieldName, &returnValue))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
            return;
        }
    }
    else
    {
        if (!message->GetRepeatedInt32OrUnsignedOrEnum(fieldName, index, &returnValue))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\"[%d] for message \"%s\"", fieldName, index,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
            return;
        }
    }

    scriptContext.SetResult(returnValue);
}

static void PbReadInt64(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    int64 returnValue;

    auto index = scriptContext.GetArgument<int>(2);

    if (index < 0)
    {
        if (!message->GetInt64OrUnsigned(fieldName, &returnValue))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
            return;
        }
    }
    else
    {
        if (!message->GetRepeatedInt64OrUnsigned(fieldName, index, &returnValue))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\"[%d] for message \"%s\"", fieldName, index,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
            return;
        }
    }

    scriptContext.SetResult(returnValue);
}

static void PbReadFloat(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    float returnValue;

    auto index = scriptContext.GetArgument<int>(2);

    if (index < 0)
    {
        if (!message->GetFloatOrDouble(fieldName, &returnValue))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
            return;
        }
    }
    else
    {
        if (!message->GetRepeatedFloatOrDouble(fieldName, index, &returnValue))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\"[%d] for message \"%s\"", fieldName, index,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
            return;
        }
    }

    scriptContext.SetResult(returnValue);
}

static void PbReadBool(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    bool returnValue;

    auto index = scriptContext.GetArgument<int>(2);

    if (index < 0)
    {
        if (!message->GetBool(fieldName, &returnValue))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
            return;
        }
    }
    else
    {
        if (!message->GetRepeatedBool(fieldName, index, &returnValue))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\"[%d] for message \"%s\"", fieldName, index,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
            return;
        }
    }

    scriptContext.SetResult(returnValue);
}

static void PbReadString(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    std::string returnValue;

    auto index = scriptContext.GetArgument<int>(2);

    if (index < 0)
    {
        if (!message->GetString(fieldName, returnValue))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
            return;
        }
    }
    else
    {
        if (!message->GetRepeatedString(fieldName, index, returnValue))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\"[%d] for message \"%s\"", fieldName, index,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
            return;
        }
    }

    scriptContext.SetResult(returnValue.c_str());
}

static void PbReadBytes(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    std::string returnValue;

    auto index = scriptContext.GetArgument<int>(2);

    if (index < 0)
    {
        if (!message->GetString(fieldName, returnValue))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
            return;
        }
    }
    else
    {
        if (!message->GetRepeatedString(fieldName, index, returnValue))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\"[%d] for message \"%s\"", fieldName, index,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
            return;
        }
    }

    int len = returnValue.length();
    char* lenBuf = reinterpret_cast<char*>(&len);
    std::string lenStr = std::string(lenBuf, 4);
    returnValue = lenStr + returnValue;

    scriptContext.SetResult(returnValue.c_str());
}

static void PbGetRepeatedFieldCount(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    auto count = message->GetRepeatedFieldCount(fieldName);

    if (count == -1)
    {
        return scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                              message->GetProtobufMessage()->GetTypeName().c_str());
    }

    scriptContext.SetResult(count);
}

static void PbHasField(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    scriptContext.SetResult(message->HasField(fieldName));
}

static void PbSetInt(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    auto value = scriptContext.GetArgument<int>(2);
    auto index = scriptContext.GetArgument<int>(3);

    if (index < 0)
    {
        if (!message->SetInt32OrUnsignedOrEnum(fieldName, value))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
        }
    }
    else
    {
        if (!message->SetRepeatedInt32OrUnsignedOrEnum(fieldName, index, value))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\"[%d] for message \"%s\"", fieldName, index,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
        }
    }
}

static void PbSetInt64(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    auto value = scriptContext.GetArgument<int64>(2);
    auto index = scriptContext.GetArgument<int>(3);

    if (index < 0)
    {
        if (!message->SetInt64OrUnsigned(fieldName, value))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
        }
    }
    else
    {
        if (!message->SetRepeatedInt64OrUnsigned(fieldName, index, value))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\"[%d] for message \"%s\"", fieldName, index,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
        }
    }
}

static void PbSetFloat(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    auto value = scriptContext.GetArgument<float>(2);
    auto index = scriptContext.GetArgument<int>(3);

    if (index < 0)
    {
        if (!message->SetFloatOrDouble(fieldName, value))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
        }
    }
    else
    {
        if (!message->SetRepeatedFloatOrDouble(fieldName, index, value))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\"[%d] for message \"%s\"", fieldName, index,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
        }
    }
}

static void PbSetBool(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    auto value = scriptContext.GetArgument<bool>(2);
    auto index = scriptContext.GetArgument<int>(3);

    if (index < 0)
    {
        if (!message->SetBool(fieldName, value))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
        }
    }
    else
    {
        if (!message->SetRepeatedBool(fieldName, index, value))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\"[%d] for message \"%s\"", fieldName, index,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
        }
    }
}

static void PbSetString(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    auto value = strdup(std::string(scriptContext.GetArgument<const char*>(2)).c_str());
    auto index = scriptContext.GetArgument<int>(3);

    if (index < 0)
    {
        if (!message->SetString(fieldName, value))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
        }
    }
    else
    {
        if (!message->SetRepeatedString(fieldName, index, value))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\"[%d] for message \"%s\"", fieldName, index,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
        }
    }
}

static void PbSetBytes(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    auto len = *(int*)scriptContext.GetArgument<const char*>(2);
    auto value = std::string(scriptContext.GetArgument<const char*>(2) + 4, len);
    auto index = scriptContext.GetArgument<int>(3);

    if (index < 0)
    {
        if (!message->SetString(fieldName, value))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
        }
    }
    else
    {
        if (!message->SetRepeatedString(fieldName, index, value))
        {
            scriptContext.ThrowNativeError("Invalid field \"%s\"[%d] for message \"%s\"", fieldName, index,
                                           message->GetProtobufMessage()->GetTypeName().c_str());
        }
    }
}

static void PbAddInt(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    auto value = scriptContext.GetArgument<int>(2);

    if (!message->AddInt32OrUnsignedOrEnum(fieldName, value))
    {
        scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                       message->GetProtobufMessage()->GetTypeName().c_str());
    }
}

static void PbAddInt64(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    auto value = scriptContext.GetArgument<int64>(2);

    if (!message->AddInt64OrUnsigned(fieldName, value))
    {
        scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                       message->GetProtobufMessage()->GetTypeName().c_str());
    }
}

static void PbAddFloat(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    auto value = scriptContext.GetArgument<float>(2);

    if (!message->AddFloatOrDouble(fieldName, value))
    {
        scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                       message->GetProtobufMessage()->GetTypeName().c_str());
    }
}

static void PbAddBool(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    auto value = scriptContext.GetArgument<bool>(2);

    if (!message->AddBool(fieldName, value))
    {
        scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                       message->GetProtobufMessage()->GetTypeName().c_str());
    }
}

static void PbAddString(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    auto value = scriptContext.GetArgument<const char*>(2);

    if (!message->AddString(fieldName, value))
    {
        scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
                                       message->GetProtobufMessage()->GetTypeName().c_str());
    }
}

static void PbRemoveRepeatedFieldValue(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();
    GET_FIELD_NAME_OR_ERR();

    auto index = scriptContext.GetArgument<int>(2);

    if (!message->RemoveRepeatedFieldValue(fieldName, index))
    {
        scriptContext.ThrowNativeError("Invalid field \"%s\"[%d] for message \"%s\"", fieldName, index,
                                       message->GetProtobufMessage()->GetTypeName().c_str());
    }
}

// static void PbReadMessage(ScriptContext& scriptContext)
//{
//     GET_MESSAGE_OR_ERR();
//     GET_FIELD_NAME_OR_ERR();
//
//     google::protobuf::Message* subMessage = nullptr;
//
//     if (!message->GetMessage(fieldName, &subMessage))
//     {
//         scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
//                                        message->GetProtobufMessage()->GetTypeName().c_str());
//         return;
//     }
//
//     auto subClientMessage = new ClientMessage(subMessage);
//     managed_clientmessages.push_back(subClientMessage);
//     scriptContext.SetResult(subClientMessage);
// }
//
// static void PbReadRepeatedMessage(ScriptContext& scriptContext)
//{
//     GET_MESSAGE_OR_ERR();
//     GET_FIELD_NAME_OR_ERR();
//
//     const google::protobuf::Message* subMessage = nullptr;
//
//     auto index = scriptContext.GetArgument<int>(2);
//
//     if (!message->GetRepeatedMessage(fieldName, index, &subMessage))
//     {
//         scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
//                                        message->GetProtobufMessage()->GetTypeName().c_str());
//         return;
//     }
//
//     auto subClientMessage = new ClientMessage(const_cast<google::protobuf::Message*>(subMessage));
//     managed_clientmessages.push_back(subClientMessage);
//     scriptContext.SetResult(subClientMessage);
// }
//
// static void PbAddMessage(ScriptContext& scriptContext)
//{
//     GET_MESSAGE_OR_ERR();
//     GET_FIELD_NAME_OR_ERR();
//
//     google::protobuf::Message* subMessage;
//
//     if (!message->AddMessage(fieldName, &subMessage))
//     {
//         scriptContext.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName,
//                                        message->GetProtobufMessage()->GetTypeName().c_str());
//         return;
//     }
//
//     auto subClientMessage = new ClientMessage(subMessage);
//     managed_clientmessages.push_back(subClientMessage);
//     scriptContext.SetResult(subClientMessage);
// }

static void PbGetDebugString(ScriptContext& scriptContext)
{
    GET_MESSAGE_OR_ERR();

    scriptContext.SetResult(message->GetDebugString().c_str());
}

static void ClientMessageFindMessageIdByName(ScriptContext& scriptContext)
{
    auto messageName = scriptContext.GetArgument<const char*>(0);
    auto message = globals::networkMessages->FindNetworkMessagePartial(messageName);

    if (message == nullptr)
    {
        scriptContext.ThrowNativeError("Could not find client message: %s", messageName);
        return;
    }

    scriptContext.SetResult(message->GetNetMessageInfo()->m_MessageId);
}

static void ClientMessageCreate(ScriptContext& scriptContext)
{
    auto messageName = scriptContext.GetArgument<const char*>(0);
    auto message = new ClientMessage(messageName);

    if (message->GetSerializableMessage() == nullptr)
    {
        scriptContext.ThrowNativeError("Failed to create client message: %s", messageName);
        return;
    }

    managed_clientmessages.push_back(message);

    scriptContext.SetResult(message);
}

static void ClientMessageCreateById(ScriptContext& scriptContext)
{
    auto messageId = scriptContext.GetArgument<int>(0);
    auto message = new ClientMessage(messageId);

    if (message->GetSerializableMessage() == nullptr)
    {
        scriptContext.ThrowNativeError("Failed to create client message: %d", messageId);
        return;
    }

    managed_clientmessages.push_back(message);

    scriptContext.SetResult(message);
}

static void ClientMessageGetRecipients(ScriptContext& scriptContext)
{
    auto message = scriptContext.GetArgument<ClientMessage*>(0);

    if (message == nullptr)
    {
        scriptContext.ThrowNativeError("Invalid message");
        return;
    }

    scriptContext.SetResult(message->GetRecipientMask() ? *message->GetRecipientMask() : 0);
}

static void ClientMessageSetRecipients(ScriptContext& scriptContext)
{
    auto message = scriptContext.GetArgument<ClientMessage*>(0);
    auto recipientMask = scriptContext.GetArgument<uint64>(1);

    *message->GetRecipientMask() = recipientMask;
}

static void ClientMessageSend(ScriptContext& scriptContext)
{
    auto message = scriptContext.GetArgument<ClientMessage*>(0);

    CRecipientFilter filter{};
    filter.AddRecipientsFromMask(message->GetRecipientMask() ? *message->GetRecipientMask() : 0);

    globals::gameEventSystem->PostEventAbstract(0, false, &filter, message->GetSerializableMessage(), message->GetProtobufMessage(), 0);
}

static void ClientMessageDelete(ScriptContext& scriptContext)
{
    auto message = scriptContext.GetArgument<ClientMessage*>(0);

    auto it = std::find(managed_clientmessages.begin(), managed_clientmessages.end(), message);
    if (it != managed_clientmessages.end())
    {
        managed_clientmessages.erase(it);
        delete message;
    }
}

static void ClientMessageGetMessageId(ScriptContext& scriptContext)
{
    auto message = scriptContext.GetArgument<ClientMessage*>(0);

    if (message == nullptr || message->GetSerializableMessage() == nullptr)
    {
        scriptContext.ThrowNativeError("Invalid message");
        return;
    }

    scriptContext.SetResult(message->GetMessageID());
}

static void ClientMessageGetMessageName(ScriptContext& scriptContext)
{
    auto message = scriptContext.GetArgument<ClientMessage*>(0);

    if (message == nullptr || message->GetSerializableMessage() == nullptr)
    {
        scriptContext.ThrowNativeError("Invalid message");
        return;
    }

    scriptContext.SetResult(message->GetSerializableMessage()->GetUnscopedName());
}

static void ClientMessageGetMessageSender(ScriptContext& scriptContext)
{
    auto message = scriptContext.GetArgument<ClientMessage*>(0);

    if (message == nullptr || message->GetSerializableMessage() == nullptr)
    {
        scriptContext.ThrowNativeError("Invalid message");
        return;
    }

    scriptContext.SetResult(message->GetSender());
}

static void ClientMessageGetMessageTypeName(ScriptContext& scriptContext)
{
    auto message = scriptContext.GetArgument<ClientMessage*>(0);

    if (message == nullptr || message->GetProtobufMessage() == nullptr)
    {
        scriptContext.ThrowNativeError("Invalid message");
        return;
    }

    scriptContext.SetResult(message->GetProtobufMessage()->GetTypeName().c_str());
}

REGISTER_NATIVES(clientmessages, {
    ScriptEngine::RegisterNativeHandler("HOOK_CLIENTMESSAGE", HookClientMessage);
    ScriptEngine::RegisterNativeHandler("UNHOOK_CLIENTMESSAGE", UnhookClientMessage);
    ScriptEngine::RegisterNativeHandler("CM_PB_HASFIELD", PbHasField);
    ScriptEngine::RegisterNativeHandler("CM_PB_READINT", PbReadInt);
    ScriptEngine::RegisterNativeHandler("CM_PB_READINT64", PbReadInt64);
    ScriptEngine::RegisterNativeHandler("CM_PB_READFLOAT", PbReadFloat);
    ScriptEngine::RegisterNativeHandler("CM_PB_READBOOL", PbReadBool);
    ScriptEngine::RegisterNativeHandler("CM_PB_READSTRING", PbReadString);
    ScriptEngine::RegisterNativeHandler("CM_PB_READBYTES", PbReadBytes);
    ScriptEngine::RegisterNativeHandler("CM_PB_GETREPEATEDFIELDCOUNT", PbGetRepeatedFieldCount);
    ScriptEngine::RegisterNativeHandler("CM_PB_SETINT", PbSetInt);
    ScriptEngine::RegisterNativeHandler("CM_PB_SETINT64", PbSetInt64);
    ScriptEngine::RegisterNativeHandler("CM_PB_SETFLOAT", PbSetFloat);
    ScriptEngine::RegisterNativeHandler("CM_PB_SETBOOL", PbSetBool);
    ScriptEngine::RegisterNativeHandler("CM_PB_SETSTRING", PbSetString);
    ScriptEngine::RegisterNativeHandler("CM_PB_SETBYTES", PbSetBytes);
    ScriptEngine::RegisterNativeHandler("CM_PB_ADDINT", PbAddInt);
    ScriptEngine::RegisterNativeHandler("CM_PB_ADDINT64", PbAddInt64);
    ScriptEngine::RegisterNativeHandler("CM_PB_ADDFLOAT", PbAddFloat);
    ScriptEngine::RegisterNativeHandler("CM_PB_ADDBOOL", PbAddBool);
    ScriptEngine::RegisterNativeHandler("CM_PB_ADDSTRING", PbAddString);
    ScriptEngine::RegisterNativeHandler("CM_PB_REMOVEREPEATEDFIELDVALUE", PbRemoveRepeatedFieldValue);
    //    ScriptEngine::RegisterNativeHandler("CM_PB_READMESSAGE", PbReadMessage);
    //    ScriptEngine::RegisterNativeHandler("CM_PB_READREPEATEDMESSAGE", PbReadRepeatedMessage);
    //    ScriptEngine::RegisterNativeHandler("CM_PB_ADDMESSAGE", PbAddMessage);
    ScriptEngine::RegisterNativeHandler("CM_PB_GETDEBUGSTRING", PbGetDebugString);
    ScriptEngine::RegisterNativeHandler("CLIENTMESSAGE_FINDMESSAGEIDBYNAME", ClientMessageFindMessageIdByName);
    ScriptEngine::RegisterNativeHandler("CLIENTMESSAGE_CREATE", ClientMessageCreate);
    ScriptEngine::RegisterNativeHandler("CLIENTMESSAGE_CREATEBYID", ClientMessageCreateById);
    ScriptEngine::RegisterNativeHandler("CLIENTMESSAGE_GETRECIPIENTS", ClientMessageGetRecipients);
    ScriptEngine::RegisterNativeHandler("CLIENTMESSAGE_SETRECIPIENTS", ClientMessageSetRecipients);
    ScriptEngine::RegisterNativeHandler("CLIENTMESSAGE_SEND", ClientMessageSend);
    ScriptEngine::RegisterNativeHandler("CLIENTMESSAGE_DELETE", ClientMessageDelete);
    ScriptEngine::RegisterNativeHandler("CLIENTMESSAGE_GETID", ClientMessageGetMessageId);
    ScriptEngine::RegisterNativeHandler("CLIENTMESSAGE_GETNAME", ClientMessageGetMessageName);
    ScriptEngine::RegisterNativeHandler("CLIENTMESSAGE_GETSENDER", ClientMessageGetMessageSender);
    ScriptEngine::RegisterNativeHandler("CLIENTMESSAGE_GETTYPE", ClientMessageGetMessageTypeName);
})
} // namespace counterstrikesharp
