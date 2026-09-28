#include "core/dynamic_hook.h"
#pragma once

#include "core/log.h"
#include <cassert>
#include <thread>

#include "core/globals.h"
#include "core/managers/entity_manager.h"

#include "scripting/script_engine.h"
namespace counterstrikesharp {

inline HookResult OnTakeDamageProxy(HookMode mode, DynamicHookContext& hook)
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
