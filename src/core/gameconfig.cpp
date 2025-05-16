#include "core/gameconfig.h"

#include <fstream>

#include "log.h"

namespace counterstrikesharp {

CGameConfig::CGameConfig(const std::string& path) { m_sPath = path; }

CGameConfig::~CGameConfig() = default;

bool CGameConfig::Init(char* conf_error, int conf_error_size)
{
    std::ifstream ifs(m_sPath);
    if (!ifs)
    {
        V_snprintf(conf_error, conf_error_size, "Gamedata file not found.");
        return false;
    }

    m_json = json::parse(ifs);

#if _WIN32
    constexpr auto platform = "windows";
#else
    constexpr auto platform = "linux";
#endif

    try
    {
        for (auto& [k, v] : m_json.items())
        {
            if (v.contains("signatures"))
            {
                if (auto library = v["signatures"]["library"]; library.is_string())
                {
                    m_umLibraries[k] = library.get<std::string>();
                }
                if (auto signature = v["signatures"][platform]; signature.is_string())
                {
                    m_umSignatures[k] = signature.get<std::string>();
                }
            }
            if (v.contains("offsets"))
            {
                if (auto offset = v["offsets"][platform]; offset.is_number_integer())
                {
                    m_umOffsets[k] = offset.get<std::int64_t>();
                }
            }
            if (v.contains("patches"))
            {
                if (auto patch = v["patches"][platform]; patch.is_string())
                {
                    m_umPatches[k] = patch.get<std::string>();
                }
            }
        }
    }
    catch (const std::exception& ex)
    {
        V_snprintf(conf_error, conf_error_size, "Failed to parse gamedata file: %s", ex.what());
        return false;
    }
    return true;
}

const std::string CGameConfig::GetPath() { return m_sPath; }

const char* CGameConfig::GetLibrary(const std::string& name)
{
    // My recommendation is switch to C++20.
    auto it = m_umLibraries.find(name);
    if (it == m_umLibraries.end())
    {
        return nullptr;
    }
    return it->second.c_str();
}

const char* CGameConfig::GetSignature(const std::string& name)
{
    auto it = m_umSignatures.find(name);
    if (it == m_umSignatures.end())
    {
        return nullptr;
    }
    return it->second.c_str();
}

const char* CGameConfig::GetSymbol(const char* name)
{
    const char* symbol = this->GetSignature(name);

    if (!symbol || strlen(symbol) <= 1)
    {
        CSSHARP_CORE_ERROR("Missing symbol: {}\n", name);
        return nullptr;
    }
    return symbol + 1;
}

const char* CGameConfig::GetPatch(const std::string& name)
{
    auto it = m_umPatches.find(name);
    if (it == m_umPatches.end())
    {
        return nullptr;
    }
    return it->second.c_str();
}

int CGameConfig::GetOffset(const std::string& name)
{
    auto it = m_umOffsets.find(name);
    if (it == m_umOffsets.end())
    {
        return -1;
    }
    return it->second;
}

void* CGameConfig::GetAddress(const std::string& name, void* engine, void* server, char* error, int maxlen)
{
    CSSHARP_CORE_ERROR("Not implemented.");
    return nullptr;
}

modules::CModule** CGameConfig::GetModule(const char* name)
{
    const char* library = this->GetLibrary(name);
    if (!library) return nullptr;

    if (strcmp(library, "engine") == 0) return &modules::engine;
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
    if (!sigOrSymbol || strlen(sigOrSymbol) <= 0)
    {
        CSSHARP_CORE_ERROR("Missing signature or symbol: {}\n", name);
        return false;
    }
    return sigOrSymbol[0] == '@';
}

void* CGameConfig::ResolveSignature(const char* name)
{
    modules::CModule** module = this->GetModule(name);
    if (!module || !(*module))
    {
        CSSHARP_CORE_ERROR("Invalid Module {}\n", name);
        return nullptr;
    }

    void* address = nullptr;
    if (this->IsSymbol(name))
    {
        const char* symbol = this->GetSymbol(name);
        if (!symbol)
        {
            CSSHARP_CORE_ERROR("Invalid symbol for {}\n", name);
            return nullptr;
        }
        address = (*module)->FindSymbol(symbol);
    }
    else
    {
        const char* signature = this->GetSignature(name);
        if (!signature)
        {
            CSSHARP_CORE_ERROR("Failed to find signature for {}\n", name);
            return nullptr;
        }

        address = (*module)->FindSignature(signature);
    }

    if (!address)
    {
        CSSHARP_CORE_ERROR("Failed to find address for {}\n", name);
        return nullptr;
    }
    return address;
}

std::string CGameConfig::GetDirectoryName(const std::string& directoryPathInput)
{
    std::string directoryPath = std::string(directoryPathInput);

    size_t found = std::string(directoryPath).find_last_of("/\\");
    if (found != std::string::npos)
    {
        return std::string(directoryPath, found + 1);
    }
    return "";
}

std::vector<int16_t> CGameConfig::HexToByte(std::string_view src)
{
    if (src.empty())
    {
        return {};
    }

    auto hex_char_to_byte = [](char c) -> int16_t {
        if (c >= '0' && c <= '9') return c - '0';
        if (c >= 'A' && c <= 'F') return c - 'A' + 10;
        if (c >= 'a' && c <= 'f') return c - 'a' + 10;

        // a valid hex char can never go up to 0xFF
        return -1;
    };

    std::vector<int16_t> result{};

    const bool is_code_style = src[0] == '\\';

    const std::string_view pattern = is_code_style ? R"(\x)" : " ";
    const std::string_view wildcard = is_code_style ? "2A" : "?";

    std::string::size_type pos = 0;

    while (pos < src.size())
    {
        std::string::size_type found = src.find(pattern, pos);
        if (found == std::string::npos)
        {
            found = src.size();
        }
        std::string_view str = src.substr(pos, found - pos);
        pos = found + pattern.size();

        if (str.empty()) continue;

        std::string byte(str.data(), str.size());

        if (byte.substr(0, wildcard.size()) == wildcard)
        {
            result.emplace_back(-1);
            continue;
        }

        if (byte.size() < 2)
        {
            return {};
        }

        const auto high = hex_char_to_byte(byte[0]);
        const auto low = hex_char_to_byte(byte[1]);

        if (high == 0xFF || low == 0xFF)
        {
            return {};
        }

        result.emplace_back((high << 4) | low);
    }

    return result;
}
} // namespace counterstrikesharp
