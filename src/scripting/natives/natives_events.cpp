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

#include "core/managers/event_manager.h"
#include "scripting/autonative.h"
#include "igameevents.h"


namespace counterstrikesharp {

std::vector<IGameEvent *> managed_game_events;

static void HookEvent(ScriptContext &script_context) {
    const char *name = script_context.GetArgument<const char *>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);
    auto post = script_context.GetArgument<bool>(2);

    globals::eventManager.HookEvent(name, callback, post);
}

static void UnhookEvent(ScriptContext &script_context) {
    const char *name = script_context.GetArgument<const char *>(0);
    auto callback = script_context.GetArgument<CallbackT>(1);
    auto post = script_context.GetArgument<bool>(2);

    globals::eventManager.UnhookEvent(name, callback, post);
}

static IGameEvent *CreateEvent(ScriptContext &script_context) {
    auto name = script_context.GetArgument<const char *>(0);
    bool force = script_context.GetArgument<bool>(1);

    auto pEvent = globals::gameEventManager->CreateEvent(name, force);

    if (pEvent != nullptr) {
        managed_game_events.push_back(pEvent);
    }

    return pEvent;
}

static void FireEvent(ScriptContext &script_context) {
    auto game_event = script_context.GetArgument<IGameEvent *>(0);
    bool dont_broadcast = script_context.GetArgument<bool>(1);
    if (!game_event) {
        script_context.ThrowNativeError("Invalid game event");
    }

    globals::gameEventManager->FireEvent(game_event, dont_broadcast);
    managed_game_events.erase(std::remove(managed_game_events.begin(), managed_game_events.end(), game_event), managed_game_events.end());
}


static void FireEventToClient(ScriptContext& script_context) {
    auto game_event = script_context.GetArgument<IGameEvent*>(0);
    int entityIndex = script_context.GetArgument<int>(1);
    if (!game_event) {
        script_context.ThrowNativeError("Invalid game event");
    }

    IGameEventListener2* pListener = globals::GetLegacyGameEventListener(CPlayerSlot(entityIndex - 1));
    if (!pListener) {
        script_context.ThrowNativeError("Could not get player event listener");
    }

    pListener->FireGameEvent(game_event);
}

static void FreeEvent(ScriptContext& script_context) {
    auto game_event = script_context.GetArgument<IGameEvent*>(0);
    if (!game_event) {
            script_context.ThrowNativeError("Invalid game event");
    }

    globals::gameEventManager->FreeEvent(game_event);
    managed_game_events.erase(std::remove(managed_game_events.begin(), managed_game_events.end(), game_event), managed_game_events.end());
}

static const char *GetEventName(ScriptContext &script_context) {
    IGameEvent *game_event = script_context.GetArgument<IGameEvent *>(0);

    if (!game_event) {
        script_context.ThrowNativeError("Invalid game event");
        return nullptr;
    }

    return game_event->GetName();
}

static bool GetEventBool(ScriptContext &script_context) {
    IGameEvent *game_event = script_context.GetArgument<IGameEvent *>(0);
    const char *key_name = script_context.GetArgument<const char *>(1);

    if (!game_event) {
        script_context.ThrowNativeError("Invalid game event");
        return false;
    }

    return game_event->GetBool(key_name);
}

static int GetEventInt(ScriptContext &script_context) {
    IGameEvent *game_event = script_context.GetArgument<IGameEvent *>(0);
    const char *key_name = script_context.GetArgument<const char *>(1);

    if (!game_event) {
        script_context.ThrowNativeError("Invalid game event");
        return -1;
    }

    return game_event->GetInt(key_name);
}

static float GetEventFloat(ScriptContext &script_context) {
    IGameEvent *game_event = script_context.GetArgument<IGameEvent *>(0);
    const char *key_name = script_context.GetArgument<const char *>(1);

    if (!game_event) {
        script_context.ThrowNativeError("Invalid game event");
        return -1;
    }

    return game_event->GetFloat(key_name);
}

static const char *GetEventString(ScriptContext &script_context) {
    IGameEvent *game_event = script_context.GetArgument<IGameEvent *>(0);
    const char *key_name = script_context.GetArgument<const char *>(1);

    if (!game_event) {
        script_context.ThrowNativeError("Invalid game event");
        return nullptr;
    }

    return game_event->GetString(key_name);
}

static void SetEventBool(ScriptContext &script_context) {
    IGameEvent *game_event = script_context.GetArgument<IGameEvent *>(0);
    const char *key_name = script_context.GetArgument<const char *>(1);
    const bool value = script_context.GetArgument<bool>(2);

    if (game_event) {
        game_event->SetBool(key_name, value);
    }
}

static void SetEventInt(ScriptContext &script_context) {
    IGameEvent *game_event = script_context.GetArgument<IGameEvent *>(0);
    const char *key_name = script_context.GetArgument<const char *>(1);
    const int value = script_context.GetArgument<int>(2);

    if (game_event) {
        game_event->SetInt(key_name, value);
    }
}

static void SetEventFloat(ScriptContext &script_context) {
    IGameEvent *game_event = script_context.GetArgument<IGameEvent *>(0);
    const char *key_name = script_context.GetArgument<const char *>(1);
    const float value = script_context.GetArgument<float>(2);

    if (game_event) {
        game_event->SetFloat(key_name, value);
    }
}

static void SetEventString(ScriptContext &script_context) {
    IGameEvent *game_event = script_context.GetArgument<IGameEvent *>(0);
    const char *key_name = script_context.GetArgument<const char *>(1);
    const char *value = script_context.GetArgument<const char *>(2);

    if (game_event) {
        game_event->SetString(key_name, value);
    }
}

static void *GetPlayerController(ScriptContext &scriptContext) {
    IGameEvent *gameEvent = scriptContext.GetArgument<IGameEvent *>(0);
    const char *keyName = scriptContext.GetArgument<const char *>(1);

    if (gameEvent == nullptr) {
        scriptContext.ThrowNativeError("Invalid game event");
        return nullptr;
    }

    return gameEvent->GetPlayerController(keyName);
}

static void SetPlayerController(ScriptContext &scriptContext) {
    IGameEvent *gameEvent = scriptContext.GetArgument<IGameEvent *>(0);
    const char *keyName = scriptContext.GetArgument<const char *>(1);
    auto *value = scriptContext.GetArgument<CEntityInstance *>(2);

    if (gameEvent != nullptr) {
        gameEvent->SetPlayer(keyName, value);
    }
}

static void SetEntity(ScriptContext &scriptContext) {
    IGameEvent *gameEvent = scriptContext.GetArgument<IGameEvent *>(0);
    const char *keyName = scriptContext.GetArgument<const char *>(1);
    auto *value = scriptContext.GetArgument<CEntityInstance *>(2);

    if (gameEvent != nullptr) {
        gameEvent->SetEntity(keyName, value);
    }
}

static void SetEntityIndex(ScriptContext &scriptContext) {
    IGameEvent *gameEvent = scriptContext.GetArgument<IGameEvent *>(0);
    const char *keyName = scriptContext.GetArgument<const char *>(1);
    auto index = scriptContext.GetArgument<int>(2);

    if (gameEvent != nullptr) {
        gameEvent->SetEntity(keyName, CEntityIndex{ index });
    }
}

static void *GetPlayerPawn(ScriptContext& scriptContext) {
    IGameEvent *gameEvent = scriptContext.GetArgument<IGameEvent *>(0);
    const char *keyName = scriptContext.GetArgument<const char *>(1);

    if (gameEvent == nullptr) {
        scriptContext.ThrowNativeError("Invalid game event");
        return nullptr;
    }

    return gameEvent->GetPlayerPawn(keyName);
}

static uint64 GetUint64(ScriptContext &scriptContext) {
    IGameEvent *gameEvent = scriptContext.GetArgument<IGameEvent *>(0);
    const char *keyName = scriptContext.GetArgument<const char *>(1);

    if (gameEvent == nullptr) {
        scriptContext.ThrowNativeError("Invalid game event");
        return 0;
    }

    return gameEvent->GetUint64(keyName);
}

static void SetUint64(ScriptContext &scriptContext) {
    IGameEvent *gameEvent = scriptContext.GetArgument<IGameEvent *>(0);
    const char *keyName = scriptContext.GetArgument<const char *>(1);
    auto value = scriptContext.GetArgument<uint64>(2);

    if (gameEvent != nullptr) {
        gameEvent->SetUint64(keyName, value);
    }
}

static int LoadEventsFromFile(ScriptContext &script_context) {
    auto [path, searchAll] = script_context.GetArguments<const char *, bool>();

    return globals::gameEventManager->LoadEventsFromFile(path, searchAll);
}

REGISTER_NATIVES(events, {
    ScriptEngine::RegisterNativeHandler("HOOK_EVENT", HookEvent);
    ScriptEngine::RegisterNativeHandler("UNHOOK_EVENT", UnhookEvent);
    ScriptEngine::RegisterNativeHandler("CREATE_EVENT", CreateEvent);
    ScriptEngine::RegisterNativeHandler("FREE_EVENT", FreeEvent);
    ScriptEngine::RegisterNativeHandler("FIRE_EVENT", FireEvent);
    ScriptEngine::RegisterNativeHandler("FIRE_EVENT_TO_CLIENT", FireEventToClient);

    ScriptEngine::RegisterNativeHandler("GET_EVENT_NAME", GetEventName);
    ScriptEngine::RegisterNativeHandler("GET_EVENT_BOOL", GetEventBool);
    ScriptEngine::RegisterNativeHandler("GET_EVENT_FLOAT", GetEventFloat);
    ScriptEngine::RegisterNativeHandler("GET_EVENT_STRING", GetEventString);
    ScriptEngine::RegisterNativeHandler("GET_EVENT_INT", GetEventInt);

    ScriptEngine::RegisterNativeHandler("SET_EVENT_BOOL", SetEventBool);
    ScriptEngine::RegisterNativeHandler("SET_EVENT_FLOAT", SetEventFloat);
    ScriptEngine::RegisterNativeHandler("SET_EVENT_STRING", SetEventString);
    ScriptEngine::RegisterNativeHandler("SET_EVENT_INT", SetEventInt);

    ScriptEngine::RegisterNativeHandler("GET_EVENT_PLAYER_CONTROLLER", GetPlayerController);
    ScriptEngine::RegisterNativeHandler("SET_EVENT_PLAYER_CONTROLLER", SetPlayerController);
    ScriptEngine::RegisterNativeHandler("GET_EVENT_PLAYER_PAWN", GetPlayerPawn);
    ScriptEngine::RegisterNativeHandler("SET_EVENT_ENTITY", SetEntity);
    ScriptEngine::RegisterNativeHandler("SET_EVENT_ENTITY_INDEX", SetEntityIndex);

    ScriptEngine::RegisterNativeHandler("GET_EVENT_UINT64", GetUint64);
    ScriptEngine::RegisterNativeHandler("SET_EVENT_UINT64", SetUint64);


    ScriptEngine::RegisterNativeHandler("LOAD_EVENTS_FROM_FILE", LoadEventsFromFile);
})

}  // namespace counterstrikesharp
