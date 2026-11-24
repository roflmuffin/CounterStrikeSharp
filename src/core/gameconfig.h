#pragma once

#include "core/binding_resolver.h"
#include "core/globals.h"
#include "core/memory_module.h"
#include <tier0/keyvalues.h>

#include <string>
#include <unordered_map>

#undef snprintf
#include <nlohmann/json.hpp>

namespace counterstrikesharp {

class CGameConfig
{
  public:
    using json = nlohmann::json;
    CGameConfig(const std::string& path);
    ~CGameConfig();

    bool Init(char* conf_error, int conf_error_size);
    const std::string GetPath();
    // Gamedata as the Binding resolver reads it, for this OS. Core code never looks keys up itself: it asks the
    // Resolution Report (core/core_bindings.h).
    const bindings::Gamedata& GetGamedata() const { return m_gamedata; }

    static std::string GetDirectoryName(const std::string& directoryPathInput);
    static std::vector<int16_t> HexToByte(std::string_view src);

  private:
    std::string m_sPath;
    // use Valve KeyValues in the future.
    // since we'd better make '\' easier.
    json m_json;
    bindings::Gamedata m_gamedata;
};

} // namespace counterstrikesharp
