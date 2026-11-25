#pragma once

// Reading CS2 builds from the shared build archive (~/dev/cs2-builds/<build-id>/linux/lib<module>.so or .img; see the
// archive's README). A module is loaded the way the dynamic loader maps it: PT_LOAD segments at their virtual
// addresses in one buffer, relocations that fill vtables and typeinfo applied against that buffer, so RTTI lookup,
// vtable reads and fingerprints work on it exactly as on the live server. Offline only (generator and real-build tests),
// never part of the plugin. Valve binaries are never committed.

#include <cstdint>
#include <filesystem>
#include <optional>
#include <string>
#include <string_view>
#include <vector>

#include "core/binding_resolver.h"
#include "core/elf_relocations.h"
#include "core/rtti_vtable.h"
#include "core/vtable_fingerprint.h"

namespace counterstrikesharp::generator {

// The archive root: $CSSHARP_BUILD_ARCHIVE, else ~/dev/cs2-builds.
std::filesystem::path DefaultArchiveRoot();

// <root>/<build>/linux/lib<module>.so, else .img. library is the Gamedata library name ("server", "engine" for
// libengine2, "tier0"). nullopt if the archive has neither.
std::optional<std::filesystem::path> FindArchivedModule(const std::filesystem::path& root, std::string_view build, std::string_view library);

class LoadedModule
{
  public:
    // A real ELF (.so) or a flat VA image (.img, file offset == virtual address). Throws std::runtime_error with the
    // file name when it cannot be read or is not a 64-bit ELF image.
    static LoadedModule Load(const std::filesystem::path& file);

    // Loads library of build from the archive; throws std::runtime_error naming the build and module when missing.
    static LoadedModule FromArchive(const std::filesystem::path& root, std::string_view build, std::string_view library);

    LoadedModule(LoadedModule&&) = default;
    LoadedModule& operator=(LoadedModule&&) = default;
    LoadedModule(const LoadedModule&) = delete;
    LoadedModule& operator=(const LoadedModule&) = delete;

    // The module as the resolver sees a loaded one: code segments, RTTI lookup, vtable slots, pure/deleted virtuals.
    [[nodiscard]] bindings::ModuleImage Image() const;

    [[nodiscard]] rtti::VtableLookup FindVtable(std::string_view className) const;

    // Fingerprints of every slot of the class's primary vtable; throws std::runtime_error unless exactly one is found.
    [[nodiscard]] std::vector<vtable::Fingerprint> Fingerprints(std::string_view className) const;

    // A vtable slot's target as the loader relocated it (nullopt where no relocation fills the word).
    [[nodiscard]] std::optional<std::uintptr_t> ReadVtableSlot(std::uintptr_t vtable, std::size_t index) const;

    [[nodiscard]] const std::vector<vtable::CodeSegment>& Code() const { return m_code; }

  private:
    LoadedModule() = default;

    std::string m_name;
    std::vector<std::uint64_t> m_memory; // the mapped image, 8-byte aligned like the loader's mapping
    std::vector<rtti::MemoryRegion> m_names; // read-only data (typeinfo names)
    std::vector<rtti::MemoryRegion> m_data; // relocated read-only data (typeinfo, vtables)
    std::vector<vtable::CodeSegment> m_code;
    elf::RelocatedWords m_relocations;
    std::uintptr_t m_pureVirtual = 0;
    std::uintptr_t m_deletedVirtual = 0;
};

} // namespace counterstrikesharp::generator
