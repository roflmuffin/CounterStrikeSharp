#include "core/memory_module.h"
#include "core/globals.h"
#include "platform.h"
#include <algorithm>
#include <cstddef>
#include <cstdint>
#include <filesystem>
#include <memory>
#include <string_view>
#include <algorithm>

#if _WIN32
#include <Psapi.h>
#include <winternl.h>
#else
#include <link.h>
#include <elf.h>
#endif

#include "dbg.h"
#include "log.h"
#include "core/gameconfig.h"
#include "core/memory.h"
#include "metamod_oslink.h"

namespace counterstrikesharp::modules {
void Initialize()
{
    if (!moduleList.empty())
        return;

#ifdef _WIN32
    // walk through peb to get modules
    const auto pteb = reinterpret_cast<PTEB>(
        __readgsqword(reinterpret_cast<DWORD_PTR>(&static_cast<NT_TIB*>(nullptr)->Self)));
    const auto peb = pteb->ProcessEnvironmentBlock;

    for (auto entry = peb->Ldr->InMemoryOrderModuleList.Flink;
         entry != &peb->Ldr->InMemoryOrderModuleList; entry = entry->Flink) {
        const auto module_entry =
            CONTAINING_RECORD(entry, LDR_DATA_TABLE_ENTRY, InMemoryOrderLinks);

        std::wstring_view w_name = module_entry->FullDllName.Buffer;

        // a hack way to do so
        std::string name(w_name.begin(), w_name.end());

        std::ranges::replace(name, '\\', '/');

        // check for extension first
        if (!name.ends_with(MODULE_EXT))
            continue;

        // no addons
        if (name.find(R"(csgo/addons/)") != std::string::npos)
            continue;

        // we need only modules from ROOTBIN and GAMEBIN
        bool isFromRootBin = name.find(ROOTBIN) != std::string::npos;
        bool isFromGameBin = name.find(GAMEBIN) != std::string::npos;
        if (!isFromGameBin && !isFromRootBin)
            continue;

        auto mod = std::make_unique<CModule>(
            name, reinterpret_cast<std::uintptr_t>(module_entry->DllBase));
        // it will delete itself after going out of scope
        if (!mod->IsInitialized())
            continue;

        moduleList.emplace_back(std::move(mod));
    }
#else
    dl_iterate_phdr(
        [](struct dl_phdr_info* info, size_t, void*) {
            std::string name = info->dlpi_name;

            if (!name.ends_with(MODULE_EXT))
                return 0;

            if (name.find("csgo/addons") != std::string::npos)
                return 0;

            bool isFromRootBin = name.find(ROOTBIN) != std::string::npos;
            bool isFromGameBin = name.find(GAMEBIN) != std::string::npos;
            if (!isFromGameBin && !isFromRootBin)
                return 0;

            auto mod = std::make_unique<CModule>(name, info);
            if (!mod->IsInitialized())
                return 0;

            moduleList.emplace_back(std::move(mod));
            return 0;
        },
        nullptr);
#endif
}

CModule* GetModuleByName(std::string name)
{
#ifdef _WIN32
    // or add this in GetGameDirectory()?
    std::ranges::replace(name, '\\', '/');
#endif

    const auto it = std::ranges::find_if(moduleList, [name](const std::unique_ptr<CModule>& i) {
        return name.ends_with(i->m_pszModule);
    });

    if (it == moduleList.end()) {
        CSSHARP_CORE_ERROR("Cannot find module {}", name);

        return nullptr;
    }

    return it->get();
}

constexpr std::array modules_to_read_from_disk = {
    MODULE_PREFIX "engine2" MODULE_EXT,
    MODULE_PREFIX "server" MODULE_EXT,
};

#ifdef _WIN32
CModule::CModule(std::string_view path, std::uint64_t base)
{
    const auto dos_header = reinterpret_cast<PIMAGE_DOS_HEADER>(base);
    if (dos_header->e_magic != IMAGE_DOS_SIGNATURE) {
        return;
    }

    const auto nt_header = reinterpret_cast<PIMAGE_NT_HEADERS>(base + dos_header->e_lfanew);
    if (nt_header->Signature != IMAGE_NT_SIGNATURE) {
        return;
    }

    m_base = reinterpret_cast<std::uint8_t*>(base);
    m_pszModule = path.substr(path.find_last_of('/') + 1);
    m_pszPath = path;
    m_baseAddress = base;
    m_size = nt_header->OptionalHeader.SizeOfImage;

    const bool should_read_from_disk = std::ranges::any_of(modules_to_read_from_disk,
                            [&](const auto& i) { return m_pszModule == i; });

    std::vector<std::uint8_t> disk_data{};
    if (should_read_from_disk) {
        std::ifstream stream(m_pszPath, std::ios::in | std::ios::binary);
        if (!stream.good()) {
            CSSHARP_CORE_ERROR("Cannot open file {}", m_pszPath);
            return;
        }
        disk_data.reserve(std::filesystem::file_size(m_pszPath));
        disk_data.assign((std::istreambuf_iterator(stream)), std::istreambuf_iterator<char>());
    }

    auto section = IMAGE_FIRST_SECTION(nt_header);

    for (auto i = 0; i < nt_header->FileHeader.NumberOfSections; i++, section++) {
        const auto is_executable = (section->Characteristics & IMAGE_SCN_MEM_EXECUTE) != 0;
        const auto is_readable = (section->Characteristics & IMAGE_SCN_MEM_READ) != 0;

        if (is_executable && is_readable) {
            const auto start = this->m_baseAddress + section->VirtualAddress;
            const auto size = (std::min)(section->SizeOfRawData, section->Misc.VirtualSize);
            const auto data = reinterpret_cast<std::uint8_t*>(start);

            auto& segment = m_vecSegments.emplace_back();

            segment.address = start;
            segment.bytes.reserve(size);

            if (should_read_from_disk) {
                if (auto bytes = GetOriginalBytes(disk_data, start - m_baseAddress, size)) {
                    CSSHARP_CORE_INFO("Copying bytes from disk for {}", m_pszPath);
                    segment.bytes = bytes.value();
                    continue;
                }
                CSSHARP_CORE_ERROR("Cannot get original bytes for {}", m_pszPath);
                return;
            }

            segment.bytes.assign(&data[0], &data[size]);
        }
    }

    DumpSymbols();

    if (m_fnCreateInterface == nullptr)
        return;

    m_bInitialized = true;
}
#else
CModule::CModule(std::string_view path, dl_phdr_info* info)
{
    m_pszModule = path.substr(path.find_last_of('/') + 1);
    m_pszPath = path.data();
    m_baseAddress = info->dlpi_addr;

    const bool should_read_from_disk = std::ranges::any_of(modules_to_read_from_disk,
                            [&](const auto& i) { return m_pszModule == i; });

    std::vector<std::uint8_t> disk_data{};
    if (should_read_from_disk) {
        std::ifstream stream(m_pszPath, std::ios::in | std::ios::binary);
        if (!stream.good()) {
            CSSHARP_CORE_ERROR("Cannot open file {}", m_pszPath);
            return;
        }
        disk_data.reserve(std::filesystem::file_size(m_pszPath));
        disk_data.assign((std::istreambuf_iterator(stream)), std::istreambuf_iterator<char>());
    }

    for (auto i = 0; i < info->dlpi_phnum; i++) {
        auto address = m_baseAddress + info->dlpi_phdr[i].p_paddr;
        auto type = info->dlpi_phdr[i].p_type;
        auto is_dynamic_section = type == PT_DYNAMIC;
        if (is_dynamic_section) {
            DumpSymbols(reinterpret_cast<ElfW(Dyn)*>(address));
            continue;
        }

        if (type != PT_LOAD)
            continue;

        auto flags = info->dlpi_phdr[i].p_flags;

        auto is_executable = (flags & PF_X) != 0;
        auto is_readable = (flags & PF_R) != 0;

        if (!is_executable || !is_readable)
            continue;

        auto size = info->dlpi_phdr[i].p_filesz;
        auto* data = reinterpret_cast<std::uint8_t*>(address);

        auto& segment = m_vecSegments.emplace_back();

        segment.address = address;
        segment.bytes.reserve(size);

        if (should_read_from_disk) {
            if (auto bytes = GetOriginalBytes(disk_data, address - m_baseAddress, size)) {
                CSSHARP_CORE_INFO("Copying bytes from disk for {}", m_pszPath);
                segment.bytes = bytes.value();
                continue;
            }
            CSSHARP_CORE_ERROR("Cannot get original bytes for {}", m_pszPath);
            return;
        }

        segment.bytes.assign(&data[0], &data[size]);
    }

    if (m_fnCreateInterface == nullptr)
        return;

    m_bInitialized = true;
}
#endif

#ifdef _WIN32
void CModule::DumpSymbols()
{
    const auto dos_header = reinterpret_cast<PIMAGE_DOS_HEADER>(m_baseAddress);

    const auto nt_header = reinterpret_cast<PIMAGE_NT_HEADERS>(
        reinterpret_cast<std::uint8_t*>(m_baseAddress) + dos_header->e_lfanew);

    const auto [export_address_rva, export_size] =
        nt_header->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_EXPORT];
    if (export_size == 0 || export_address_rva == 0)
        return;

    auto export_directory =
        reinterpret_cast<IMAGE_EXPORT_DIRECTORY*>(m_baseAddress + export_address_rva);
    const auto names =
        reinterpret_cast<uint32_t*>(m_baseAddress + export_directory->AddressOfNames);
    const auto addresses =
        reinterpret_cast<uint32_t*>(m_baseAddress + export_directory->AddressOfFunctions);
    const auto ordinals =
        reinterpret_cast<std::uint16_t*>(m_baseAddress + export_directory->AddressOfNameOrdinals);

    for (auto i = 0ull; i < export_directory->NumberOfNames; i++) {
        const auto export_name = reinterpret_cast<const char*>(m_baseAddress + names[i]);
        const auto address = m_baseAddress + addresses[ordinals[i]];

        if (address >= reinterpret_cast<uintptr_t>(export_directory) &&
            address < reinterpret_cast<uintptr_t>(export_directory) + export_size)
            continue;

        if (std::string_view(export_name) == "CreateInterface") {
            m_fnCreateInterface = reinterpret_cast<fnCreateInterface>(address);
        }

        _symbols[export_name] = address;
    }
}
#else
void CModule::DumpSymbols(ElfW(Dyn) * dyn)
{
    // thanks to https://stackoverflow.com/a/57099317
    auto GetNumberOfSymbolsFromGnuHash = [](ElfW(Addr) gnuHashAddress) {
        // See https://flapenguin.me/2017/05/10/elf-lookup-dt-gnu-hash/ and
        // https://sourceware.org/ml/binutils/2006-10/msg00377.html
        struct Header
        {
            uint32_t nbuckets;
            uint32_t symoffset;
            uint32_t bloom_size;
            uint32_t bloom_shift;
        };

        auto header = (Header*)gnuHashAddress;
        const auto bucketsAddress =
            gnuHashAddress + sizeof(Header) + (sizeof(std::uintptr_t) * header->bloom_size);

        // Locate the chain that handles the largest index bucket.
        uint32_t lastSymbol = 0;
        auto bucketAddress = (uint32_t*)bucketsAddress;
        for (uint32_t i = 0; i < header->nbuckets; ++i) {
            uint32_t bucket = *bucketAddress;
            if (lastSymbol < bucket) {
                lastSymbol = bucket;
            }
            bucketAddress++;
        }

        if (lastSymbol < header->symoffset) {
            return header->symoffset;
        }

        // Walk the bucket's chain to add the chain length to the total.
        const auto chainBaseAddress = bucketsAddress + (sizeof(uint32_t) * header->nbuckets);
        for (;;) {
            auto chainEntry =
                (uint32_t*)(chainBaseAddress + (lastSymbol - header->symoffset) * sizeof(uint32_t));
            lastSymbol++;

            // If the low bit is set, this entry is the end of the chain.
            if (*chainEntry & 1) {
                break;
            }
        }

        return lastSymbol;
    };

    ElfW(Sym) * symbols{};
    ElfW(Word) * hash_ptr{};

    char* string_table{};
    std::size_t symbol_count{};

    while (dyn->d_tag != DT_NULL) {
        if (dyn->d_tag == DT_HASH) {
            hash_ptr = reinterpret_cast<ElfW(Word)*>(dyn->d_un.d_ptr);
            symbol_count = hash_ptr[1];
        } else if (dyn->d_tag == DT_STRTAB) {
            string_table = reinterpret_cast<char*>(dyn->d_un.d_ptr);
        } else if (!symbol_count && dyn->d_tag == DT_GNU_HASH) {
            symbol_count = GetNumberOfSymbolsFromGnuHash(dyn->d_un.d_ptr);
        } else if (dyn->d_tag == DT_SYMTAB) {
            symbols = reinterpret_cast<ElfW(Sym)*>(dyn->d_un.d_ptr);

            for (auto i = 0; i < symbol_count; i++) {
                if (!symbols[i].st_name) {
                    continue;
                }

                if (symbols[i].st_other != 0) {
                    continue;
                }

                auto address = symbols[i].st_value + m_baseAddress;
                std::string_view name = &string_table[symbols[i].st_name];

                if (name == "CreateInterface") {
                    m_fnCreateInterface = reinterpret_cast<fnCreateInterface>(address);
                }

                _symbols.insert({name.data(), address});
            }
        }

        dyn++;
    }
}
#endif

std::optional<std::vector<std::uint8_t>>
CModule::GetOriginalBytes(const std::vector<std::uint8_t>& disk_data, std::uintptr_t rva,
                          std::size_t size)
{
    auto get_file_ptr_from_rva = [](std::uint8_t* data,
                                    std::uintptr_t address) -> std::optional<std::uintptr_t> {
#ifdef _WIN32
        // thank you praydog
        // https://github.com/cursey/kananlib/blob/b0323a0b005fc9e3944e0ea36dcc98eda4b84eea/src/Module.cpp#L176

        const auto dos_header = reinterpret_cast<PIMAGE_DOS_HEADER>(data);
        const auto nt_header = reinterpret_cast<PIMAGE_NT_HEADERS>(&data[dos_header->e_lfanew]);
        auto section = IMAGE_FIRST_SECTION(nt_header);
        for (auto i = 0; i < nt_header->FileHeader.NumberOfSections; i++, section++) {
            auto section_size = section->Misc.VirtualSize;
            if (section_size == 0) {
                section_size = section->SizeOfRawData;
            }

            if (address >= section->VirtualAddress &&
                address < static_cast<uintptr_t>(section->VirtualAddress) + section_size) {
                const auto delta = section->VirtualAddress - section->PointerToRawData;

                return reinterpret_cast<std::uintptr_t>(data + (address - delta));
            }
        }
        return std::nullopt;
#else
        // on linux you can just read from rva
        return reinterpret_cast<std::uintptr_t>(data + address);
#endif
    };

    const auto disk_ptr = get_file_ptr_from_rva(const_cast<std::uint8_t*>(disk_data.data()), rva);
    if (!disk_ptr)
        return std::nullopt;

    const auto disk_bytes = reinterpret_cast<std::uint8_t*>(*disk_ptr);
    std::vector<std::uint8_t> result{&disk_bytes[0], &disk_bytes[size]};

    return result;
}

void* CModule::FindSignature(const char* signature)
{
    if (signature == nullptr || strlen(signature) == 0) {
        return nullptr;
    }

    auto pData = CGameConfig::HexToByte(signature);
    if (pData.empty()) [[unlikely]] {
        CSSHARP_CORE_ERROR("Cannot convert signture \"{}\" to bytes", signature);
        return nullptr;
    }

    return this->FindSignature(pData);
}

void* CModule::FindSignature(const std::vector<int16_t>& sigBytes)
{
    for (auto&& segment : m_vecSegments) {
        const auto size = segment.bytes.size();
        auto* data = segment.bytes.data();

        auto first_byte = sigBytes[0];
        std::uint8_t* end = data + size - sigBytes.size();

        for (std::uint8_t* current = data; current <= end; ++current) {
            if (first_byte != -1)
                current = std::find(current, end, first_byte);

            if (current == end) {
                break;
            }

            if (std::equal(sigBytes.begin() + 1, sigBytes.end(), current + 1,
                           [](auto opt, auto byte) {
                               return opt == -1 || opt == byte;
                           })) {
                return reinterpret_cast<void*>(current - data + segment.address);
            }
        }
    }

    return nullptr;
}

void* CModule::FindInterface(std::string_view name)
{
    if (_interfaces.empty()) {
        auto RelToAbs = [](std::uintptr_t address, int offset) {
            const auto displacement = *reinterpret_cast<int32_t*>(address + offset);
            return address + offset + displacement + sizeof(int32_t);
        };

        auto address = reinterpret_cast<std::uintptr_t>(m_fnCreateInterface);

#ifndef _WIN32
        // CreateInterface on linux starts with a jmp instruciton
        address = RelToAbs(address, 1);
        // skipping 16 bytes to mov rax, interfaceRegisterList
        address += 16;
#endif

        using InstantiateInterfaceFn_t = void* (*)();

        class CInterfaceRegister
        {
          public:
            InstantiateInterfaceFn_t fnCreate;
            const char* szName;
            CInterfaceRegister* pNext;
        };

        void* ret_interface{};

        const auto interface_reg = *reinterpret_cast<CInterfaceRegister**>(RelToAbs(address, 3));
        for (auto list = interface_reg; list != nullptr; list = list->pNext) {
            auto interface_addrss = list->fnCreate();
            if (const std::string_view interface_name = list->szName; interface_name == name)
                ret_interface = interface_addrss;

            _interfaces.insert({list->szName, reinterpret_cast<uintptr_t>(interface_addrss)});
        }

        if (ret_interface == nullptr) {
            // Replace Error() from hl2sdk-cs2, it essentially calls Plat_ExitProcess
            CSSHARP_CORE_ERROR("Could not find interface {} in {}", name, m_pszModule);
            Plat_ExitProcess(1);
        }

        return ret_interface;
    }
    
    const auto it = _interfaces.find(name.data());

    if (it == _interfaces.end()) {
        CSSHARP_CORE_ERROR("Could not find interface {} in {}", name, m_pszModule);
        Plat_ExitProcess(1);
    }

    return reinterpret_cast<void*>(it->second);
}

void* CModule::FindSymbol(const std::string& name)
{
    if (const auto it = _symbols.find(name); it != _symbols.end()) {
        return reinterpret_cast<void*>(it->second);
    }

    CSSHARP_CORE_ERROR("Cannot find symbol {}", name);
    return nullptr;
}
} // namespace counterstrikesharp::modules
