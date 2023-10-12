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

// Required to access convar methods :(
#define protected public
#define private public
#include <tier1/convar.h>
#undef protected
#undef private

namespace counterstrikesharp {
class ScriptCallback;

class ConCommandInfo {
    friend class ConCommandManager;

public:
    ConCommandInfo() {}

public:
    void HookChange(CallbackT callback, bool post);
    void UnhookChange(CallbackT callback, bool post);
    ScriptCallback* GetCallback() { return callback_pre; }

private:
    bool sdn;
    ConCommandRefAbstract* p_cmd;
    ConCommand* command;
    ScriptCallback* callback_pre;
    ScriptCallback* callback_post;
    bool server_only;
};

class ConCommandManager : public GlobalClass {
    friend class ConCommandInfo;
    friend void CommandCallback(const CCommand& command);
    friend void CommandCallback_Post(const CCommand& command);

public:
    ConCommandManager();
    ~ConCommandManager();
    void OnAllInitialized() override;
    void OnShutdown() override;
    ConCommandInfo* AddOrFindCommand(const char* name,
                                     const char* description,
                                     bool server_only,
                                     int flags);
    bool DispatchClientCommand(CPlayerSlot slot, const char* cmd, const CCommand* args);

    bool InternalDispatch(CPlayerSlot slot, const CCommand* args);

    int GetCommandClient();

    bool InternalDispatch_Post(CPlayerSlot slot, const CCommand* args);

public:
    ConCommandInfo* AddCommand(
        const char* name, const char* description, bool server_only, int flags, CallbackT callback);
    bool RemoveCommand(const char* name, CallbackT callback);
    ConCommandInfo* FindCommand(const char* name);

private:
    void SetCommandClient(int client);

private:
    int last_command_client;
    std::vector<ConCommandInfo*> m_cmd_list;
    std::map<std::string, ConCommandInfo*> m_cmd_lookup;
};

}  // namespace counterstrikesharp