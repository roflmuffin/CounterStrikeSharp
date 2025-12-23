#pragma once

#include "core/log.h"
#include "pch.h"
#include "dynohook/core.h"
#include "dynohook/manager.h"

#include "core/globals.h"
#include "core/managers/entity_manager.h"

#include "scripting/script_engine.h"
namespace counterstrikesharp {

inline HookResult OnTakeDamageProxy(HookMode mode, dyno::Hook& hook)
{
    auto* pThis = reinterpret_cast<CBaseEntity*>(hook.getArgument<void*>(0));
    auto* pInfo = reinterpret_cast<CTakeDamageInfo*>(hook.getArgument<void*>(1));
    auto* pResult = reinterpret_cast<CTakeDamageResult*>(hook.getArgument<void*>(2));

    if (mode == Pre)
    {
        if (!globals::entityManager.Hook_OnTakeDamage_Alive_Pre(pThis, pInfo, pResult))
        {
            hook.setReturnValue(1);
            return HookResult::Handled;
        }
    }
    else
    {
        globals::entityManager.Hook_OnTakeDamage_Alive_Post(pThis, pInfo, pResult);
    }

    return HookResult::Continue;
}
} // namespace counterstrikesharp
