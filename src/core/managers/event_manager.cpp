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
#include "vprof.h"

namespace counterstrikesharp {

EventManager::EventManager() = default;

EventManager::~EventManager() = default;

void EventManager::OnStartup() {}

void EventManager::OnGameLoopInitialized()
{
    while (!m_PendingHooks.empty())
    {
        const auto& pendingHook = m_PendingHooks.top();
        HookEvent(pendingHook.m_Name.c_str(), pendingHook.m_fnCallback, pendingHook.m_bPost);
        m_PendingHooks.pop();
    }
}

void EventManager::OnAllInitialized() {}

void EventManager::OnAllInitialized_Post()
{
    m_hooks.Clear();
    m_hooks.Add(&IGameEventManager2::FireEvent, globals::gameEventManager, this, &EventManager::OnFireEvent, nullptr);
}

void EventManager::OnShutdown()
{
    m_hooks.Clear();

    if (globals::gameEventManager) globals::gameEventManager->RemoveListener(this);
}

void EventManager::FireGameEvent(IGameEvent* pEvent) {}

bool EventManager::HookEvent(const char* szName, CallbackT fnCallback, bool bPost)
{
    EventHook* pHook;

    // Plugin load is called before game loop (and thus events file is loaded)
    // So we defer hooking until game loop is initialized
    if (!globals::gameLoopInitialized)
    {
        const PendingEventHook pendingHook{ szName, fnCallback, bPost };
        m_PendingHooks.push(pendingHook);
        return true;
    }

    CSSHARP_CORE_TRACE("[EventManager] Hooking event: {0} with callback pointer: {1}", szName, (void*)fnCallback);

    if (!globals::gameEventManager->FindListener(this, szName))
    {
        globals::gameEventManager->AddListener(this, szName, true);
    }

    auto search = m_hooksMap.find(szName);
    // If hook struct is not found
    if (search == m_hooksMap.end())
    {
        pHook = new EventHook();

        if (bPost)
        {
            pHook->m_pPostHook = globals::callbackManager.CreateCallback(szName);
            pHook->m_pPostHook->AddListener(fnCallback);
        }
        else
        {
            pHook->m_pPreHook = globals::callbackManager.CreateCallback(szName);
            pHook->m_pPreHook->AddListener(fnCallback);
        }

        pHook->m_Name = std::string(szName);

        m_hooksMap[szName] = pHook;

        return true;
    }
    else
    {
        pHook = search->second;
    }

    if (bPost)
    {
        if (!pHook->m_pPostHook)
        {
            pHook->m_pPostHook = globals::callbackManager.CreateCallback("");
        }

        pHook->m_pPostHook->AddListener(fnCallback);
    }
    else
    {
        if (!pHook->m_pPreHook)
        {
            pHook->m_pPreHook = globals::callbackManager.CreateCallback("");
        }

        pHook->m_pPreHook->AddListener(fnCallback);
    }

    return true;
}

bool EventManager::UnhookEvent(const char* szName, CallbackT fnCallback, bool bPost)
{
    EventHook* pHook;
    ScriptCallback* pCallback;

    auto search = m_hooksMap.find(szName);
    if (search == m_hooksMap.end())
    {
        return false;
    }

    pHook = search->second;

    if (bPost)
    {
        pCallback = pHook->m_pPostHook;
    }
    else
    {
        pCallback = pHook->m_pPreHook;
    }

    pCallback->RemoveListener(fnCallback);

    if (pCallback->GetFunctionCount() == 0)
    {
        globals::callbackManager.ReleaseCallback(pCallback);

        if (bPost)
        {
            pHook->m_pPostHook = nullptr;
        }
        else
        {
            pHook->m_pPreHook = nullptr;
        }
    }

    CSSHARP_CORE_TRACE("Unhooking event: {0} with callback pointer: {1}", szName, (void*)fnCallback);

    return true;
}

KHook::Return<bool> EventManager::OnFireEvent(IGameEventManager2* hookThis, IGameEvent* pEvent, bool bDontBroadcast)
{
    if (!pEvent) return { KHook::Action::Ignore };
    auto it = m_hooksMap.find(pEvent->GetName());
    if (it == m_hooksMap.end()) return { KHook::Action::Ignore };

    auto* eventHook = it->second;
    EventOverride override = { bDontBroadcast };
    bool blocked = false;
    if (auto* callback = eventHook->m_pPreHook)
    {
        // Stack-local contexts survive a managed callback recursively firing the
        // same event, or unregistering itself while it runs.
        auto functions = callback->GetFunctions();
        for (auto function : functions)
        {
            if (!function) continue;
            fxNativeContext raw{};
            ScriptContextRaw context(raw);
            context.Reset();
            context.Push(pEvent);
            context.Push(&override);
            function(&raw);
            if (context.GetResult<HookResult>() >= HookResult::Handled)
            {
                blocked = true;
                break;
            }
        }
    }

    auto freeEvent = [hookThis](IGameEvent* event) {
        if (event) hookThis->FreeEvent(event);
    };
    std::unique_ptr<IGameEvent, decltype(freeEvent)> copy(hookThis->DuplicateEvent(pEvent), freeEvent);
    // A suppressed event is still visible to downstream hooks. Release it only
    // after the rest of the shared hook chain has finished using it.
    std::unique_ptr<IGameEvent, decltype(freeEvent)> suppressed(blocked ? pEvent : nullptr, freeEvent);
    auto action = blocked ? KHook::Action::Supersede : KHook::Action::Ignore;
    auto result =
        KHook::Recall(&IGameEventManager2::FireEvent, KHook::Return<bool>{ action, false }, hookThis, pEvent, override.m_bDontBroadcast);

    if (copy && eventHook->m_pPostHook)
    {
        auto functions = eventHook->m_pPostHook->GetFunctions();
        for (auto function : functions)
        {
            if (!function) continue;
            fxNativeContext raw{};
            ScriptContextRaw context(raw);
            context.Reset();
            context.Push(copy.get());
            context.Push(&override);
            function(&raw);
        }
    }
    return result;
}
} // namespace counterstrikesharp
