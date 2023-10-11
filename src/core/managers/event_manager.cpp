/**
 * =============================================================================
 * SourceMod
 * Copyright (C) 2004-2016 AlliedModders LLC.  All rights reserved.
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
 *
 * As a special exception, AlliedModders LLC gives you permission to link the
 * code of this program (as well as its derivative works) to "Half-Life 2," the
 * "Source Engine," the "SourcePawn JIT," and any Game MODs that run on software
 * by the Valve Corporation.  You must obey the GNU General Public License in
 * all respects for all other code used.  Additionally, AlliedModders LLC grants
 * this exception to all derivative works.  AlliedModders LLC defines further
 * exceptions, found in LICENSE.txt (as of this writing, version JULY-31-2007),
 * or <http://www.sourcemod.net/license.php>.
 *
 * This file has been modified from its original form, under the GNU General
 * Public License, version 3.0.
 */

#include "core/managers/event_manager.h"

#include "core/log.h"
#include "scripting/callback_manager.h"

SH_DECL_HOOK2(IGameEventManager2, FireEvent, SH_NOATTRIB, 0, bool, IGameEvent *, bool);

namespace counterstrikesharp {

EventManager::EventManager() {}

EventManager::~EventManager() {}

void EventManager::OnStartup() {}

void EventManager::OnAllInitialized() {
    SH_ADD_HOOK(IGameEventManager2, FireEvent, globals::gameEventManager,
                SH_MEMBER(this, &EventManager::OnFireEvent), false);
    SH_ADD_HOOK(IGameEventManager2, FireEvent, globals::gameEventManager,
                SH_MEMBER(this, &EventManager::OnFireEvent_Post), true);
}

void EventManager::OnShutdown() {
    SH_REMOVE_HOOK(IGameEventManager2, FireEvent, globals::gameEventManager,
                   SH_MEMBER(this, &EventManager::OnFireEvent), false);
    SH_REMOVE_HOOK(IGameEventManager2, FireEvent, globals::gameEventManager,
                   SH_MEMBER(this, &EventManager::OnFireEvent_Post), true);

    globals::gameEventManager->RemoveListener(this);
}

void EventManager::FireGameEvent(IGameEvent *event) {}

bool EventManager::HookEvent(const char *name, CallbackT callback, bool post) {
    EventHook *p_hook;

    if (!globals::gameEventManager->FindListener(this, name)) {
        globals::gameEventManager->AddListener(this, name, true);
    }

    CSSHARP_CORE_INFO("Hooking event: {0} with callback pointer: {1}", name, (void *)callback);

    auto search = m_hooks.find(name);
    // If hook struct is not found
    if (search == m_hooks.end()) {
        p_hook = new EventHook();

        if (post) {
            p_hook->PostHook = globals::callbackManager.CreateCallback(name);
            p_hook->PostHook->AddListener(callback);
        } else {
            p_hook->PreHook = globals::callbackManager.CreateCallback(name);
            p_hook->PreHook->AddListener(callback);
        }

        p_hook->name = std::string(name);

        m_hooks[name] = p_hook;

        return true;
    } else {
        p_hook = search->second;
    }

    if (post) {
        if (!p_hook->PostHook) {
            p_hook->PostHook = globals::callbackManager.CreateCallback("");
        }

        p_hook->PostHook->AddListener(callback);
    } else {
        if (!p_hook->PreHook) {
            p_hook->PreHook = globals::callbackManager.CreateCallback("");
            ;
        }

        p_hook->PreHook->AddListener(callback);
    }

    return true;
}

bool EventManager::UnhookEvent(const char *name, CallbackT callback, bool post) {
    EventHook *p_hook;
    ScriptCallback *p_callback;

    auto search = m_hooks.find(name);
    if (search == m_hooks.end()) {
        return false;
    }

    p_hook = search->second;

    if (post) {
        p_callback = p_hook->PostHook;
    } else {
        p_callback = p_hook->PreHook;
    }

    // Remove from function list
    if (p_callback == nullptr) {
        return false;
    }

    p_callback = nullptr;
    if (post) {
        p_hook->PostHook = nullptr;
    } else {
        p_hook->PreHook = nullptr;
    }

    // TODO: Clean up callback if theres noone left attached.

    CSSHARP_CORE_INFO("Unhooking event: {0} with callback pointer: {1}", name, (void *)callback);

    return true;
}

bool EventManager::OnFireEvent(IGameEvent *pEvent, bool bDontBroadcast) {
    EventHook *p_hook;
    const char *name;

    if (!pEvent) {
        RETURN_META_VALUE(MRES_IGNORED, false);
    }

    name = pEvent->GetName();

    auto search = m_hooks.find(name);
    if (search != m_hooks.end()) {
        auto p_callback = search->second->PreHook;

        if (p_callback) {
            CSSHARP_CORE_INFO("Pushing event `{0}` pointer: {1}", name, (void *)pEvent);
            p_callback->ScriptContext().Reset();
            p_callback->ScriptContext().SetArgument(0, pEvent);
            p_callback->Execute();

            RETURN_META_VALUE(MRES_IGNORED, false);
        }
    }

    RETURN_META_VALUE(MRES_IGNORED, true);
}

bool EventManager::OnFireEvent_Post(IGameEvent *pEvent, bool bDontBroadcast) {
    RETURN_META_VALUE(MRES_IGNORED, true);
}
}  // namespace counterstrikesharp