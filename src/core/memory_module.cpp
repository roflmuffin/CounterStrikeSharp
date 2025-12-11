#include "core/memory_module.h"

#include <algorithm>
#include <cstddef>
#include <cstdint>
#include <filesystem>
#include <memory>
#include <string_view>

#include "core/globals.h"
#include "platform.h"

#if _WIN32
#include <Psapi.h>
#include <winternl.h>
#else
#include <elf.h>
#include <link.h>
#endif

#include "core/gameconfig.h"
#include "core/memory.h"
#include "dbg.h"
#include "log.h"
#include "metamod_oslink.h"

namespace counterstrikesharp::modules {
void Initialize()
{
    if (!moduleList.empty()) return;

#ifdef _WIN32
    // walk through peb to get modules
    const auto pteb = reinterpret_cast<PTEB>(__readgsqword(reinterpret_cast<DWORD_PTR>(&static_cast<NT_TIB*>(nullptr)->Self)));
    const auto peb = pteb->ProcessEnvironmentBlock;

    for (auto entry = peb->Ldr->InMemoryOrderModuleList.Flink; entry != &peb->Ldr->InMemoryOrderModuleList; entry = entry->Flink)
    {
        const auto module_entry = CONTAINING_RECORD(entry, LDR_DATA_TABLE_ENTRY, InMemoryOrderLinks);

        std::wstring_view w_name = module_entry->FullDllName.Buffer;

        // a hack way to do so
        std::string name(w_name.begin(), w_name.end());

        std::replace(name.begin(), name.end(), '\\', '/');

        // check for extension first
        if (name.rfind(MODULE_EXT) != name.length() - strlen(MODULE_EXT)) continue;

        // no addons
        if (name.find(R"(csgo/addons/)") != std::string::npos) continue;

        // we need only modules from ROOTBIN and GAMEBIN
        bool isFromRootBin = name.find(ROOTBIN) != std::string::npos;
        bool isFromGameBin = name.find(GAMEBIN) != std::string::npos;
        if (!isFromGameBin && !isFromRootBin) continue;

        auto mod = std::make_unique<CModule>(name, reinterpret_cast<std::uintptr_t>(module_entry->DllBase));
        // it will delete itself after going out of scope
        if (!mod->IsInitialized()) continue;

        moduleList.emplace_back(std::move(mod));
    }
#else
    dl_iterate_phdr([](struct dl_phdr_info* info, size_t, void*) {
        std::string name = info->dlpi_name;

        if (name.rfind(MODULE_EXT) != name.length() - strlen(MODULE_EXT)) return 0;

        if (name.find("csgo/addons") != std::string::npos) return 0;

        bool isFromRootBin = name.find(ROOTBIN) != std::string::npos;
        bool isFromGameBin = name.find(GAMEBIN) != std::string::npos;
        if (!isFromGameBin && !isFromRootBin) return 0;

        auto mod = std::make_unique<CModule>(name, info);
        if (!mod->IsInitialized()) return 0;

        moduleList.emplace_back(std::move(mod));
        return 0;
    }, nullptr);
#endif
}

CModule* GetModuleByName(std::string name)
{
#ifdef _WIN32
    // or add this in GetGameDirectory()?
    std::replace(name.begin(), name.end(), '\\', '/');
#endif

    const auto it = std::find_if(moduleList.begin(), moduleList.end(), [&name](const std::unique_ptr<CModule>& i) {
        return !i->m_pszModule.empty() && name.size() >= i->m_pszModule.size() &&
               name.substr(name.size() - i->m_pszModule.size()) == i->m_pszModule;
    });

    if (it == moduleList.end())
    {
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
    if (dos_header->e_magic != IMAGE_DOS_SIGNATURE)
    {
        return;
    }

    const auto nt_header = reinterpret_cast<PIMAGE_NT_HEADERS>(base + dos_header->e_lfanew);
    if (nt_header->Signature != IMAGE_NT_SIGNATURE)
    {
        return;
    }

    m_base = reinterpret_cast<std::uint8_t*>(base);
    m_pszModule = path.substr(path.find_last_of('/') + 1);
    m_pszPath = path;
    m_baseAddress = base;
    m_size = nt_header->OptionalHeader.SizeOfImage;

    const bool should_read_from_disk = std::any_of(modules_to_read_from_disk.begin(), modules_to_read_from_disk.end(), [&](const auto& i) {
        return m_pszModule == i;
    });

    std::vector<std::uint8_t> disk_data{};
    if (should_read_from_disk)
    {
        std::ifstream stream(m_pszPath, std::ios::in | std::ios::binary);
        if (!stream.good())
        {
            CSSHARP_CORE_ERROR("Cannot open file {}", m_pszPath);
            return;
        }
        disk_data.reserve(std::filesystem::file_size(m_pszPath));
        disk_data.assign((std::istreambuf_iterator(stream)), std::istreambuf_iterator<char>());
    }

    auto section = IMAGE_FIRST_SECTION(nt_header);

    for (auto i = 0; i < nt_header->FileHeader.NumberOfSections; i++, section++)
    {
        const auto is_executable = (section->Characteristics & IMAGE_SCN_MEM_EXECUTE) != 0;
        const auto is_readable = (section->Characteristics & IMAGE_SCN_MEM_READ) != 0;

        if (is_executable && is_readable)
        {
            const auto start = this->m_baseAddress + section->VirtualAddress;
            const auto size = (std::min)(section->SizeOfRawData, section->Misc.VirtualSize);
            const auto data = reinterpret_cast<std::uint8_t*>(start);

            auto& segment = m_vecSegments.emplace_back();

            segment.address = start;
            segment.bytes.reserve(size);

            if (should_read_from_disk)
            {
                if (auto bytes = GetOriginalBytes(disk_data, start - m_baseAddress, size))
                {
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

    // Load all sections for FindVirtualTable
    section = IMAGE_FIRST_SECTION(nt_header);
    for (auto i = 0; i < nt_header->FileHeader.NumberOfSections; i++, section++)
    {
        const auto is_readable = (section->Characteristics & IMAGE_SCN_MEM_READ) != 0;

        if (is_readable)
        {
            const auto start = this->m_baseAddress + section->VirtualAddress;
            const auto size = (std::min)(section->SizeOfRawData, section->Misc.VirtualSize);

            std::string section_name(reinterpret_cast<const char*>(section->Name),
                                     strnlen(reinterpret_cast<const char*>(section->Name), IMAGE_SIZEOF_SHORT_NAME));

            auto& sec = m_sections.emplace_back();
            sec.m_szName = section_name;
            sec.m_pBase = reinterpret_cast<void*>(start);
            sec.m_iSize = size;
        }
    }

    DumpSymbols();

    if (m_fnCreateInterface == nullptr) return;

    m_bInitialized = true;
}
#else
#ifdef __linux__
#include "dbg.h"
#include "sys/mman.h"
#include <dlfcn.h>
#include <elf.h>
#include <fcntl.h>
#include <libgen.h>
#include <link.h>
#include <locale>
#include <stdio.h>
#include <string.h>
#include <sys/stat.h>
#include <sys/types.h>

#include "tier0/memdbgon.h"
#endif

// Credits:
// https://github.com/alliedmodders/sourcemod/blob/master/core/logic/MemoryUtils.cpp#L502-L587
// https://github.com/komashchenko/DynLibUtils/blob/5eb95475170becfcc64fd5d32d14ec2b76dcb6d4/module_linux.cpp#L95
// https://github.com/Source2ZE/CS2Fixes/blob/e1a7aebee8b846b9c6be514dba890646b04a7792/src/utils/plat_unix.cpp#L53
int GetModuleInformation(HINSTANCE hModule, void** base, size_t* length, std::vector<Section>& m_sections)
{
    link_map* lmap;
    if (dlinfo(hModule, RTLD_DI_LINKMAP, &lmap) != 0)
    {
        dlclose(hModule);
        return 1;
    }

    int fd = open(lmap->l_name, O_RDONLY);
    if (fd == -1)
    {
        dlclose(hModule);
        return 2;
    }

    struct stat st;
    if (fstat(fd, &st) == 0)
    {
        void* map = mmap(nullptr, st.st_size, PROT_READ, MAP_PRIVATE, fd, 0);
        if (map != MAP_FAILED)
        {
            ElfW(Ehdr)* ehdr = static_cast<ElfW(Ehdr)*>(map);
            ElfW(Shdr)* shdrs = reinterpret_cast<ElfW(Shdr)*>(reinterpret_cast<uintptr_t>(ehdr) + ehdr->e_shoff);
            const char* strTab = reinterpret_cast<const char*>(reinterpret_cast<uintptr_t>(ehdr) + shdrs[ehdr->e_shstrndx].sh_offset);

            for (auto i = 0; i < ehdr->e_phnum; ++i)
            {
                ElfW(Phdr)* phdr = reinterpret_cast<ElfW(Phdr)*>(reinterpret_cast<uintptr_t>(ehdr) + ehdr->e_phoff + i * ehdr->e_phentsize);
                if (phdr->p_type == PT_LOAD && phdr->p_flags & PF_X)
                {
                    *base = reinterpret_cast<void*>(lmap->l_addr + phdr->p_vaddr);
                    *length = phdr->p_filesz;
                    break;
                }
            }

            for (auto i = 0; i < ehdr->e_shnum; ++i)
            {
                ElfW(Shdr)* shdr = reinterpret_cast<ElfW(Shdr)*>(reinterpret_cast<uintptr_t>(shdrs) + i * ehdr->e_shentsize);
                if (*(strTab + shdr->sh_name) == '\0') continue;

                Section section;
                section.m_szName = strTab + shdr->sh_name;
                section.m_pBase = reinterpret_cast<void*>(lmap->l_addr + shdr->sh_addr);
                section.m_iSize = shdr->sh_size;
                m_sections.push_back(section);
            }

            munmap(map, st.st_size);
        }
    }

    close(fd);

    return 0;
}

CModule::CModule(std::string_view path, dl_phdr_info* info)
{
    m_pszModule = path.substr(path.find_last_of('/') + 1);
    m_pszPath = path.data();
    m_baseAddress = info->dlpi_addr;

    auto module = dlmount(m_pszModule.c_str());
    GetModuleInformation(module, &m_base, &m_size, m_sections);

    const bool should_read_from_disk = std::any_of(modules_to_read_from_disk.begin(), modules_to_read_from_disk.end(), [&](const auto& i) {
        return m_pszModule == i;
    });

    std::vector<std::uint8_t> disk_data{};
    if (should_read_from_disk)
    {
        std::ifstream stream(m_pszPath, std::ios::in | std::ios::binary);
        if (!stream.good())
        {
            CSSHARP_CORE_ERROR("Cannot open file {}", m_pszPath);
            return;
        }
        disk_data.reserve(std::filesystem::file_size(m_pszPath));
        disk_data.assign((std::istreambuf_iterator(stream)), std::istreambuf_iterator<char>());
    }

    for (auto i = 0; i < info->dlpi_phnum; i++)
    {
        auto address = m_baseAddress + info->dlpi_phdr[i].p_paddr;
        auto type = info->dlpi_phdr[i].p_type;
        auto is_dynamic_section = type == PT_DYNAMIC;
        if (is_dynamic_section)
        {
            DumpSymbols(reinterpret_cast<ElfW(Dyn)*>(address));
            continue;
        }

        if (type != PT_LOAD) continue;

        auto flags = info->dlpi_phdr[i].p_flags;

        auto is_executable = (flags & PF_X) != 0;
        auto is_readable = (flags & PF_R) != 0;

        if (!is_executable || !is_readable) continue;

        auto size = info->dlpi_phdr[i].p_filesz;
        auto* data = reinterpret_cast<std::uint8_t*>(address);

        auto& segment = m_vecSegments.emplace_back();

        segment.address = address;
        segment.bytes.reserve(size);

        if (should_read_from_disk)
        {
            if (auto bytes = GetOriginalBytes(disk_data, address - m_baseAddress, size))
            {
                CSSHARP_CORE_INFO("Copying bytes from disk for {}", m_pszPath);
                segment.bytes = bytes.value();
                continue;
            }
            CSSHARP_CORE_ERROR("Cannot get original bytes for {}", m_pszPath);
            return;
        }

        segment.bytes.assign(&data[0], &data[size]);
    }

    if (m_fnCreateInterface == nullptr) return;

    m_bInitialized = true;
}
#endif

#ifdef _WIN32
void CModule::DumpSymbols()
{
    const auto dos_header = reinterpret_cast<PIMAGE_DOS_HEADER>(m_baseAddress);

    const auto nt_header = reinterpret_cast<PIMAGE_NT_HEADERS>(reinterpret_cast<std::uint8_t*>(m_baseAddress) + dos_header->e_lfanew);

    const auto [export_address_rva, export_size] = nt_header->OptionalHeader.DataDirectory[IMAGE_DIRECTORY_ENTRY_EXPORT];
    if (export_size == 0 || export_address_rva == 0) return;

    auto export_directory = reinterpret_cast<IMAGE_EXPORT_DIRECTORY*>(m_baseAddress + export_address_rva);
    const auto names = reinterpret_cast<uint32_t*>(m_baseAddress + export_directory->AddressOfNames);
    const auto addresses = reinterpret_cast<uint32_t*>(m_baseAddress + export_directory->AddressOfFunctions);
    const auto ordinals = reinterpret_cast<std::uint16_t*>(m_baseAddress + export_directory->AddressOfNameOrdinals);

    for (auto i = 0ull; i < export_directory->NumberOfNames; i++)
    {
        const auto export_name = reinterpret_cast<const char*>(m_baseAddress + names[i]);
        const auto address = m_baseAddress + addresses[ordinals[i]];

        if (address >= reinterpret_cast<uintptr_t>(export_directory) &&
            address < reinterpret_cast<uintptr_t>(export_directory) + export_size)
            continue;

        if (std::string_view(export_name) == "CreateInterface")
        {
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
        const auto bucketsAddress = gnuHashAddress + sizeof(Header) + (sizeof(std::uintptr_t) * header->bloom_size);

        // Locate the chain that handles the largest index bucket.
        uint32_t lastSymbol = 0;
        auto bucketAddress = (uint32_t*)bucketsAddress;
        for (uint32_t i = 0; i < header->nbuckets; ++i)
        {
            uint32_t bucket = *bucketAddress;
            if (lastSymbol < bucket)
            {
                lastSymbol = bucket;
            }
            bucketAddress++;
        }

        if (lastSymbol < header->symoffset)
        {
            return header->symoffset;
        }

        // Walk the bucket's chain to add the chain length to the total.
        const auto chainBaseAddress = bucketsAddress + (sizeof(uint32_t) * header->nbuckets);
        for (;;)
        {
            auto chainEntry = (uint32_t*)(chainBaseAddress + (lastSymbol - header->symoffset) * sizeof(uint32_t));
            lastSymbol++;

            // If the low bit is set, this entry is the end of the chain.
            if (*chainEntry & 1)
            {
                break;
            }
        }

        return lastSymbol;
    };

    ElfW(Sym) * symbols{};
    ElfW(Word) * hash_ptr{};

    char* string_table{};
    std::size_t symbol_count{};

    while (dyn->d_tag != DT_NULL)
    {
        if (dyn->d_tag == DT_HASH)
        {
            hash_ptr = reinterpret_cast<ElfW(Word)*>(dyn->d_un.d_ptr);
            symbol_count = hash_ptr[1];
        }
        else if (dyn->d_tag == DT_STRTAB)
        {
            string_table = reinterpret_cast<char*>(dyn->d_un.d_ptr);
        }
        else if (!symbol_count && dyn->d_tag == DT_GNU_HASH)
        {
            symbol_count = GetNumberOfSymbolsFromGnuHash(dyn->d_un.d_ptr);
        }
        else if (dyn->d_tag == DT_SYMTAB)
        {
            symbols = reinterpret_cast<ElfW(Sym)*>(dyn->d_un.d_ptr);
        }

        dyn++;
    }

    for (auto i = 0; i < symbol_count; i++)
    {
        if (!symbols[i].st_name)
        {
            continue;
        }

        if (symbols[i].st_other != 0)
        {
            continue;
        }

        auto address = symbols[i].st_value + m_baseAddress;
        std::string_view name = &string_table[symbols[i].st_name];

        if (name == "CreateInterface")
        {
            m_fnCreateInterface = reinterpret_cast<fnCreateInterface>(address);
        }

        _symbols.insert({ name.data(), address });
    }
}
#endif

std::optional<std::vector<std::uint8_t>>
CModule::GetOriginalBytes(const std::vector<std::uint8_t>& disk_data, std::uintptr_t rva, std::size_t size)
{
    auto get_file_ptr_from_rva = [](std::uint8_t* data, std::uintptr_t address) -> std::optional<std::uintptr_t> {
#ifdef _WIN32
        // thank you praydog
        // https://github.com/cursey/kananlib/blob/b0323a0b005fc9e3944e0ea36dcc98eda4b84eea/src/Module.cpp#L176

        const auto dos_header = reinterpret_cast<PIMAGE_DOS_HEADER>(data);
        const auto nt_header = reinterpret_cast<PIMAGE_NT_HEADERS>(&data[dos_header->e_lfanew]);
        auto section = IMAGE_FIRST_SECTION(nt_header);
        for (auto i = 0; i < nt_header->FileHeader.NumberOfSections; i++, section++)
        {
            auto section_size = section->Misc.VirtualSize;
            if (section_size == 0)
            {
                section_size = section->SizeOfRawData;
            }

            if (address >= section->VirtualAddress && address < static_cast<uintptr_t>(section->VirtualAddress) + section_size)
            {
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
    if (!disk_ptr) return std::nullopt;

    const auto disk_bytes = reinterpret_cast<std::uint8_t*>(*disk_ptr);
    std::vector<std::uint8_t> result{ &disk_bytes[0], &disk_bytes[size] };

    return result;
}

void* CModule::FindSignature(const char* signature)
{
    if (signature == nullptr || strlen(signature) == 0)
    {
        return nullptr;
    }

    auto pData = CGameConfig::HexToByte(signature);
    if (pData.empty()) [[unlikely]]
    {
        CSSHARP_CORE_ERROR("Cannot convert signture \"{}\" to bytes", signature);
        return nullptr;
    }

    auto pOld = this->FindSignature(pData);
    auto pNew = this->FindSignatureAlternative(pData);

    if (pOld != pNew)
    {
        CSSHARP_CORE_DEBUG(
            "Signature {} found different pointers using different signature scanning methods. Found old address: {}, new address: {}",
            signature, (void*)pOld, (void*)pNew);
    }

    if (pNew)
    {
        return pNew;
    }

    return pOld;
}

void* CModule::FindSignature(const std::vector<int16_t>& sigBytes)
{
    for (auto&& segment : m_vecSegments)
    {
        const auto size = segment.bytes.size();
        auto* data = segment.bytes.data();

        auto first_byte = sigBytes[0];
        std::uint8_t* end = data + size - sigBytes.size();

        for (std::uint8_t* current = data; current <= end; ++current)
        {
            if (first_byte != -1) current = std::find(current, end, first_byte);

            if (current == end)
            {
                break;
            }

            if (std::equal(sigBytes.begin() + 1, sigBytes.end(), current + 1, [](auto opt, auto byte) {
                return opt == -1 || opt == byte;
            }))
            {
                return reinterpret_cast<void*>(current - data + segment.address);
            }
        }
    }

    return nullptr;
}

void* CModule::FindSignatureAlternative(const std::vector<int16_t>& sigBytes)
{
    if (m_base == 0 || m_size == 0)
    {
        return nullptr;
    }

    auto* data = reinterpret_cast<std::uint8_t*>(m_base);
    const auto size = m_size;

    auto first_byte = sigBytes[0];
    std::uint8_t* end = data + size - sigBytes.size();

    for (std::uint8_t* current = data; current <= end; ++current)
    {
        if (first_byte != -1) current = std::find(current, end, first_byte);

        if (current == end)
        {
            break;
        }

        if (std::equal(sigBytes.begin() + 1, sigBytes.end(), current + 1, [](auto opt, auto byte) {
            return opt == -1 || opt == byte;
        }))
        {
            return reinterpret_cast<void*>(current - data + (std::uintptr_t)m_base);
        }
    }

    return nullptr;
}

void* CModule::FindInterface(std::string_view name)
{
    if (_interfaces.empty())
    {
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
        for (auto list = interface_reg; list != nullptr; list = list->pNext)
        {
            auto interface_addrss = list->fnCreate();
            if (const std::string_view interface_name = list->szName; interface_name == name) ret_interface = interface_addrss;

            _interfaces.insert({ list->szName, reinterpret_cast<uintptr_t>(interface_addrss) });
        }

        if (ret_interface == nullptr)
        {
            // Replace Error() from hl2sdk-cs2, it essentially calls Plat_ExitProcess
            CSSHARP_CORE_ERROR("Could not find interface {} in {}", name, m_pszModule);
            Plat_ExitProcess(1);
        }

        return ret_interface;
    }

    const auto it = _interfaces.find(name.data());

    if (it == _interfaces.end())
    {
        CSSHARP_CORE_ERROR("Could not find interface {} in {}", name, m_pszModule);
        Plat_ExitProcess(1);
    }

    return reinterpret_cast<void*>(it->second);
}

void* CModule::FindSymbol(const std::string& name)
{
    if (const auto it = _symbols.find(name); it != _symbols.end())
    {
        return reinterpret_cast<void*>(it->second);
    }

    CSSHARP_CORE_ERROR("Cannot find symbol {}", name);
    return nullptr;
}

#ifdef _WIN32
void* CModule::FindVirtualTable(const std::string& name)
{
    auto runTimeData = GetSection(".data");
    auto readOnlyData = GetSection(".rdata");

    if (!runTimeData || !readOnlyData)
    {
        Warning("Failed to find .data or .rdata section\n");
        return nullptr;
    }

    std::string decoratedTableName = ".?AV" + name + "@@";

    SignatureIterator sigIt(runTimeData->m_pBase, runTimeData->m_iSize, (const byte*)decoratedTableName.c_str(),
                            decoratedTableName.size() + 1);
    void* typeDescriptor = sigIt.FindNext(false);

    if (!typeDescriptor)
    {
        Warning("Failed to find type descriptor for %s\n", name.c_str());
        return nullptr;
    }

    typeDescriptor = (void*)((uintptr_t)typeDescriptor - 0x10);

    const uint32_t rttiTDRva = (uintptr_t)typeDescriptor - (uintptr_t)m_base;

    ConMsg("RTTI Type Descriptor RVA: 0x%p\n", rttiTDRva);

    SignatureIterator sigIt2(readOnlyData->m_pBase, readOnlyData->m_iSize, (const byte*)&rttiTDRva, sizeof(uint32_t));

    while (void* completeObjectLocator = sigIt2.FindNext(false))
    {
        auto completeObjectLocatorHeader = (uintptr_t)completeObjectLocator - 0xC;
        // check RTTI Complete Object Locator header, always 0x1
        if (*(int32_t*)(completeObjectLocatorHeader) != 1) continue;

        // check RTTI Complete Object Locator vtable offset
        if (*(int32_t*)((uintptr_t)completeObjectLocator - 0x8) != 0) continue;

        SignatureIterator sigIt3(readOnlyData->m_pBase, readOnlyData->m_iSize, (const byte*)&completeObjectLocatorHeader, sizeof(void*));
        void* vtable = sigIt3.FindNext(false);

        if (!vtable)
        {
            Warning("Failed to find vtable for %s\n", name.c_str());
            return nullptr;
        }

        return (void*)((uintptr_t)vtable + 0x8);
    }

    Warning("Failed to find RTTI Complete Object Locator for %s\n", name.c_str());
    return nullptr;
}
#endif

#ifndef _WIN32
void* CModule::FindVirtualTable(const std::string& name)
{
    auto readOnlyData = GetSection(".rodata");
    auto readOnlyRelocations = GetSection(".data.rel.ro");

    if (!readOnlyData || !readOnlyRelocations)
    {
        Warning("Failed to find .rodata or .data.rel.ro section\n");
        return nullptr;
    }

    std::string decoratedTableName = std::to_string(name.length()) + name;

    SignatureIterator sigIt(readOnlyData->m_pBase, readOnlyData->m_iSize, (const byte*)decoratedTableName.c_str(),
                            decoratedTableName.size() + 1);
    void* classNameString = sigIt.FindNext(false);

    if (!classNameString)
    {
        Warning("Failed to find type descriptor for %s\n", name.c_str());
        return nullptr;
    }

    SignatureIterator sigIt2(readOnlyRelocations->m_pBase, readOnlyRelocations->m_iSize, (const byte*)&classNameString, sizeof(void*));
    void* typeName = sigIt2.FindNext(false);

    if (!typeName)
    {
        Warning("Failed to find type name for %s\n", name.c_str());
        return nullptr;
    }

    void* typeInfo = (void*)((uintptr_t)typeName - 0x8);

    for (const auto& sectionName : { std::string_view(".data.rel.ro"), std::string_view(".data.rel.ro.local") })
    {
        auto section = GetSection(sectionName);
        if (!section) continue;

        SignatureIterator sigIt3(section->m_pBase, section->m_iSize, (const byte*)&typeInfo, sizeof(void*));

        while (void* vtable = sigIt3.FindNext(false))
            if (*(int64_t*)((uintptr_t)vtable - 0x8) == 0) return (void*)((uintptr_t)vtable + 0x8);
    }

    Warning("Failed to find vtable for %s\n", name.c_str());
    return nullptr;
}
#endif

} // namespace counterstrikesharp::modules
