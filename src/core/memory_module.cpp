#include "core/memory_module.h"

#if _WIN32
#include <Psapi.h>
#endif

#include "dbg.h"
#include "core/gameconfig.h"
#include "core/memory.h"
#include "metamod_oslink.h"

namespace counterstrikesharp::modules {

CModule::CModule(const char* path, const char* module) : m_pszModule(module), m_pszPath(path)
{
    char szModule[MAX_PATH];

    V_snprintf(szModule, MAX_PATH, "%s%s%s%s%s", Plat_GetGameDirectory(), path, MODULE_PREFIX,
               m_pszModule, MODULE_EXT);

    m_hModule = dlmount(szModule);

    if (!m_hModule)
        Error("Could not find %s\n", szModule);

#ifdef _WIN32
    MODULEINFO m_hModuleInfo;
    GetModuleInformation(GetCurrentProcess(), m_hModule, &m_hModuleInfo, sizeof(m_hModuleInfo));

    m_base = (void*)m_hModuleInfo.lpBaseOfDll;
    m_size = m_hModuleInfo.SizeOfImage;
#else
    if (int e = GetModuleInformation(m_hModule, &m_base, &m_size))
        Error("Failed to get module info for %s, error %d\n", szModule, e);
#endif
}

void* CModule::FindSignature(const char* signature)
{
    if (signature == nullptr || strlen(signature) == 0) {
        return nullptr;
    }

    auto pData = CGameConfig::HexToByte(signature);
    if (pData.empty()) [[unlikely]]
        return nullptr;

    return this->FindSignature(pData);
}

void* CModule::FindSignature(const std::vector<int16_t>& sigBytes)
{
    const auto first_byte = sigBytes[0];

    auto pMemory = (std::uint8_t*)m_base;
    std::uint8_t* end = pMemory + m_size - sigBytes.size();

    for (std::uint8_t* current = pMemory; current <= end; ++current) {
        if (first_byte != -1)
            current = std::find(current, end, first_byte);

        if (current == end) {
            break;
        }

        if (std::equal(sigBytes.begin() + 1, sigBytes.end(), current + 1,
                       [](auto opt, auto byte) { return opt == -1 || opt == byte; })) {
            return current;
        }
    }

    return nullptr;
}

void* CModule::FindInterface(const char* name)
{
    CreateInterfaceFn fn = (CreateInterfaceFn)dlsym(m_hModule, "CreateInterface");

    if (!fn)
        Error("Could not find CreateInterface in %s\n", m_pszModule);

    void* pInterface = fn(name, nullptr);

    if (!pInterface)
        Error("Could not find %s in %s\n", name, m_pszModule);

    return pInterface;
}
}
