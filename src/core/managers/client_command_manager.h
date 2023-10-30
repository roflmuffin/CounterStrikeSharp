#pragma once

#include <map>
#include <vector>

#include "core/globals.h"
#include "core/global_listener.h"
#include "scripting/script_engine.h"
#include <string>
#include "playerslot.h"

namespace counterstrikesharp {
class ScriptCallback;

class ClientCommandInfo {
   friend class ClientCommandManager;

 public:
   ClientCommandInfo() {}

 private:
   std::string command;
   ScriptCallback* callback_pre;
   ScriptCallback* callback_post;
};

class ClientCommandManager : public GlobalClass {


 public:
   ClientCommandManager();
   ~ClientCommandManager();
   void OnAllInitialized() override;
   void OnShutdown() override;
   bool DispatchClientCommand(CPlayerSlot slot, const char* cmd, const CCommand* args);
   void AddCommandListener(const char* cmd, CallbackT callback, bool bPost);
   void RemoveCommandListener(const char* cmd, CallbackT callback, bool bPost);

 private:
   std::vector<ClientCommandInfo*> m_cmd_list;
   std::map<std::string, ClientCommandInfo*> m_cmd_lookup;
   ClientCommandInfo m_global_cmd;
};

}  // namespace counterstrikesharp