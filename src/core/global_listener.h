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

#pragma once

namespace counterstrikesharp {
class GlobalClass {
public:
    virtual ~GlobalClass() = default;

    GlobalClass() {
        m_pGlobalClassNext = GlobalClass::head;
        GlobalClass::head = this;
    }

public:
    virtual void OnStartup() {}
    virtual void OnShutdown() {}
    virtual void OnAllInitialized() {}
    virtual void OnAllInitialized_Post() {}
    virtual void OnLevelChange(const char *mapName) {}
    virtual void OnLevelEnd() {}

public:
    GlobalClass *m_pGlobalClassNext;
    static GlobalClass *head;
};
}  // namespace counterstrikesharp

#define CALL_GLOBAL_LISTENER(func)          \
    GlobalClass *pBase = GlobalClass::head; \
    pBase = GlobalClass::head;              \
    while (pBase) {                         \
        pBase->func;                        \
        pBase = pBase->m_pGlobalClassNext;  \
    }
