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

#include <map>
#include <vector>

#include "core/globals.h"
#include "core/global_listener.h"
#include "scripting/script_engine.h"
#include <string>
#include "playerslot.h"

struct CaseInsensitiveComparator {
    bool operator()(const std::string& lhs, const std::string& rhs) const {
        return std::lexicographical_compare(
            lhs.begin(), lhs.end(),
            rhs.begin(), rhs.end(),
            [](char a, char b) { return std::tolower(a) < std::tolower(b); }
        );
    }
};

namespace counterstrikesharp {
class ScriptCallback;

class ConCommandInfo {
    friend class ConCommandManager;

public:
  ConCommandInfo();
    ~ConCommandInfo();

public:
    void HookChange(CallbackT callback, bool post);
    void UnhookChange(CallbackT callback, bool post);
    ScriptCallback* GetCallback() { return callback_pre; }

private:
    ConCommandRefAbstract p_cmd;
    ConCommand* command;
    ScriptCallback* callback_pre;
    ScriptCallback* callback_post;
    bool server_only;
};

class ConCommandManager : public GlobalClass {
    friend class ConCommandInfo;

public:
    ConCommandManager();
    ~ConCommandManager();
    void OnAllInitialized() override;
    void OnShutdown() override;

    void AddCommandListener(const char* name, CallbackT callback, HookMode mode);
    void RemoveCommandListener(const char* name, CallbackT callback, HookMode mode);
    bool IsValidValveCommand(const char* name);
    bool AddValveCommand(const char* name, const char* description, bool server_only, int flags);
    bool RemoveValveCommand(const char* name);
    void Hook_DispatchConCommand(ConCommandHandle cmd, const CCommandContext& ctx, const CCommand& args);
    void Hook_DispatchConCommand_Post(ConCommandHandle cmd, const CCommandContext& ctx, const CCommand& args);
    HookResult ExecuteCommandCallbacks(const char* name, const CCommandContext& ctx,
                                       const CCommand& args, HookMode mode);

private:
    std::vector<ConCommandInfo*> m_cmd_list;
    std::map<std::string, ConCommandInfo*, CaseInsensitiveComparator> m_cmd_lookup;
};

}  // namespace counterstrikesharp