#pragma once


#include "core/globals.h"
#include "core/memory_module.h"
#include <KeyValues.h>

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
    const char* GetLibrary(const std::string& name);
    const char* GetSignature(const std::string& name);
    const char* GetSymbol(const char* name);
    const char* GetPatch(const std::string& name);
    int GetOffset(const std::string& name);
    void* GetAddress(const std::string& name, void* engine, void* server, char* error, int maxlen);
    modules::CModule** GetModule(const char* name);
    bool IsSymbol(const char* name);
    void* ResolveSignature(const char* name);

    static std::string GetDirectoryName(const std::string& directoryPathInput);
    static std::vector<int16_t> HexToByte(std::string_view src);

  private:
    std::string m_sPath;
    // use Valve KeyValues in the future.
    // since we'd better make '\' easier.
    json m_json;
    std::unordered_map<std::string, int> m_umOffsets;
    std::unordered_map<std::string, std::string> m_umSignatures;
    std::unordered_map<std::string, void*> m_umAddresses;
    std::unordered_map<std::string, std::string> m_umLibraries;
    std::unordered_map<std::string, std::string> m_umPatches;
};

} // namespace counterstrikesharp
