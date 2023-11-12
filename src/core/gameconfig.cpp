#include "core/gameconfig.h"
#include <fstream>

#include "log.h"
#include "metamod_oslink.h"

namespace counterstrikesharp {

CGameConfig::CGameConfig(const std::string& path) { m_sPath = path; }

CGameConfig::~CGameConfig() = default;

bool CGameConfig::Init(char* conf_error, int conf_error_size)
{
    std::ifstream ifs(m_sPath);
    if (!ifs) {
        V_snprintf(conf_error, conf_error_size, "Gamedata file not found.");
        return false;
    }

    m_json = json::parse(ifs);

#if _WIN32
    constexpr auto platform = "windows";
#else
    constexpr auto platform = "linux";
#endif

    try {
        for (auto& [k, v] : m_json.items()) {
            if (v.contains("signatures")) {
                if (auto library = v["signatures"]["library"]; library.is_string()) {
                    m_umLibraries[k] = library.get<std::string>();
                }
                if (auto signature = v["signatures"][platform]; signature.is_string()) {
                    m_umSignatures[k] = signature.get<std::string>();
                }
            }
            if (v.contains("offsets")) {
                if (auto offset = v["offsets"][platform]; offset.is_number_integer()) {
                    m_umOffsets[k] = offset.get<std::int64_t>();
                }
            }
            if (v.contains("patches")) {
                if (auto patch = v["patches"][platform]; patch.is_string()) {
                    m_umPatches[k] = patch.get<std::string>();
                }
            }
        }
    } catch (const std::exception& ex) {
        V_snprintf(conf_error, conf_error_size, "Failed to parse gamedata file: %s", ex.what());
        return false;
    }
    return true;
}

const std::string CGameConfig::GetPath()
{
    return m_sPath;
}

const char* CGameConfig::GetLibrary(const std::string& name)
{
    // My recommendation is switch to C++20.
    auto it = m_umLibraries.find(name);
    if (it == m_umLibraries.end()) {
        return nullptr;
    }
    return it->second.c_str();
}

const char* CGameConfig::GetSignature(const std::string& name)
{
    auto it = m_umSignatures.find(name);
    if (it == m_umSignatures.end()) {
        return nullptr;
    }
    return it->second.c_str();
}

const char* CGameConfig::GetSymbol(const char* name)
{
    const char* symbol = this->GetSignature(name);

    if (!symbol || strlen(symbol) <= 1) {
        CSSHARP_CORE_ERROR("Missing symbol: {}\n", name);
        return nullptr;
    }
    return symbol + 1;
}

const char* CGameConfig::GetPatch(const std::string& name)
{
    auto it = m_umPatches.find(name);
    if (it == m_umPatches.end()) {
        return nullptr;
    }
    return it->second.c_str();
}

int CGameConfig::GetOffset(const std::string& name)
{
    auto it = m_umOffsets.find(name);
    if (it == m_umOffsets.end()) {
        return -1;
    }
    return it->second;
}

void* CGameConfig::GetAddress(const std::string& name, void* engine, void* server, char* error,
    int maxlen)
{
    CSSHARP_CORE_ERROR("Not implemented.");
    return nullptr;
}

modules::CModule** CGameConfig::GetModule(const char* name)
{
    const char* library = this->GetLibrary(name);
    if (!library)
        return nullptr;

    if (strcmp(library, "engine") == 0)
        return &modules::engine;
    else if (strcmp(library, "server") == 0)
        return &modules::server;
    else if (strcmp(library, "vscript") == 0)
        return &modules::vscript;
    else if (strcmp(library, "tier0") == 0)
        return &modules::tier0;

    return nullptr;
}

bool CGameConfig::IsSymbol(const char* name)
{
    const char* sigOrSymbol = this->GetSignature(name);
    if (!sigOrSymbol || strlen(sigOrSymbol) <= 0) {
        CSSHARP_CORE_ERROR("Missing signature or symbol: {}\n", name);
        return false;
    }
    return sigOrSymbol[0] == '@';
}

void* CGameConfig::ResolveSignature(const char* name)
{
    modules::CModule** module = this->GetModule(name);
    if (!module || !(*module)) {
        CSSHARP_CORE_ERROR("Invalid Module {}\n", name);
        return nullptr;
    }

    void* address = nullptr;
    if (this->IsSymbol(name)) {
        const char* symbol = this->GetSymbol(name);
        if (!symbol) {
            CSSHARP_CORE_ERROR("Invalid symbol for {}\n", name);
            return nullptr;
        }
        address = dlsym((*module)->m_hModule, symbol);
    } else {
        const char* signature = this->GetSignature(name);
        if (!signature) {
            CSSHARP_CORE_ERROR("Failed to find signature for {}\n", name);
            return nullptr;
        }
        size_t iLength = 0;
        byte* pSignature = HexToByte(signature, iLength);
        if (!pSignature) {
            return nullptr;
        }
        address = (*module)->FindSignature(pSignature, iLength);
    }

    if (!address) {
        CSSHARP_CORE_ERROR("Failed to find address for {}\n", name);
        return nullptr;
    }
    return address;
}

std::string CGameConfig::GetDirectoryName(const std::string& directoryPathInput)
{
    std::string directoryPath = std::string(directoryPathInput);

    size_t found = std::string(directoryPath).find_last_of("/\\");
    if (found != std::string::npos) {
        return std::string(directoryPath, found + 1);
    }
    return "";
}

int CGameConfig::HexStringToUint8Array(const char* hexString, uint8_t* byteArray, size_t maxBytes)
{
    if (!hexString) {
        printf("Invalid hex string.\n");
        return -1;
    }

    size_t hexStringLength = strlen(hexString);
    size_t byteCount = hexStringLength / 4; // Each "\\x" represents one byte.

    if (hexStringLength % 4 != 0 || byteCount == 0 || byteCount > maxBytes) {
        printf("Invalid hex string format or byte count.\n");
        return -1; // Return an error code.
    }

    for (size_t i = 0; i < hexStringLength; i += 4) {
        if (sscanf(hexString + i, "\\x%2hhX", &byteArray[i / 4]) != 1) {
            printf("Failed to parse hex string at position %zu.\n", i);
            return -1; // Return an error code.
        }
    }

    byteArray[byteCount] = '\0'; // Add a null-terminating character.

    return byteCount; // Return the number of bytes successfully converted.
}

byte* CGameConfig::HexToByte(const char* src, size_t& length)
{
    if (!src || strlen(src) <= 0) {
        CSSHARP_CORE_INFO("Invalid hex string\n");
        return nullptr;
    }

    length = strlen(src) / 4;
    uint8_t* dest = new uint8_t[length];
    int byteCount = HexStringToUint8Array(src, dest, length);
    if (byteCount <= 0) {
        CSSHARP_CORE_INFO("Invalid hex format %s\n", src);
        return nullptr;
    }
    return dest;
}
} // namespace counterstrikesharp