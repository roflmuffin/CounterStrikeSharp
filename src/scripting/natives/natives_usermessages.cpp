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
#include "core/globals.h"
#include "core/log.h"

#include "core/managers/usermessage_manager.h"
#include "scripting/autonative.h"
#include "igameevents.h"


namespace counterstrikesharp {

static void HookUserMessage(ScriptContext &script_context) {
   auto messageId = script_context.GetArgument<int>(0);
   auto callback = script_context.GetArgument<CallbackT>(1);
   auto mode = script_context.GetArgument<HookMode>(2);

   globals::userMessageManager.HookUserMessage(messageId, callback, mode);
}

static void UnhookUserMessage(ScriptContext &script_context) {
    auto messageId = script_context.GetArgument<int>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);
    auto mode = script_context.GetArgument<HookMode>(2);

    globals::userMessageManager.UnhookUserMessage(messageId, callback, mode);
}

static void HasField(ScriptContext &script_context) {
    auto message = script_context.GetArgument<UserMessage*>(0);
    auto fieldName = script_context.GetArgument<const char *>(1);

    script_context.SetResult(message->HasField(fieldName));
}

static void GetInt32OrUnsignedOrEnum(ScriptContext &script_context) {
    auto message = script_context.GetArgument<UserMessage*>(0);
    auto fieldName = script_context.GetArgument<const char *>(1);


    int returnValue;
    if (!message->GetInt32OrUnsignedOrEnum(fieldName, &returnValue)) {
        script_context.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName, message->GetProtobufMessage()->GetTypeName().c_str());
        return;
    }

    script_context.SetResult(returnValue);
}

static void SetInt32OrUnsignedOrEnum(ScriptContext &script_context) {
    auto message = script_context.GetArgument<UserMessage*>(0);
    auto fieldName = script_context.GetArgument<const char *>(1);
    auto value = script_context.GetArgument<int>(2);

    if (!message->SetInt32OrUnsignedOrEnum(fieldName, value)) {
        script_context.ThrowNativeError("Invalid field \"%s\" for message \"%s\"", fieldName, message->GetProtobufMessage()->GetTypeName().c_str());
    }
}

REGISTER_NATIVES(usermessages, {
   ScriptEngine::RegisterNativeHandler("HOOK_USERMESSAGE", HookUserMessage);
   ScriptEngine::RegisterNativeHandler("UNHOOK_USERMESSAGE", UnhookUserMessage);
   ScriptEngine::RegisterNativeHandler("USERMESSAGE_HASFIELD", HasField);
   ScriptEngine::RegisterNativeHandler("USERMESSAGE_HASFIELD", HasField);
   ScriptEngine::RegisterNativeHandler("USERMESSAGE_READINT", GetInt32OrUnsignedOrEnum);
   ScriptEngine::RegisterNativeHandler("USERMESSAGE_SETINT", SetInt32OrUnsignedOrEnum);
})
}  // namespace counterstrikesharp